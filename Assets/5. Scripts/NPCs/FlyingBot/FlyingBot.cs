using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace NPCs.FlyingBot
{
    public class FlyingBot : MonoBehaviour
    {
        public Transform[] surveillancePoints;
        [SerializeField] private Transform[] rotors;
        [SerializeField] private float rotorSpeed;
        public float botSpeed;
        [SerializeField] private float rotationSmoothness;

        public Animator animator;

        public Transform target;
        [SerializeField] private GuardStateEnum state;

        private BotState _currentState;
        [SerializeField] private BotChaseState chaseState = new BotChaseState();
        [SerializeField] private BotSurveillanceState surveillanceState = new BotSurveillanceState();


        private bool _movingForward;
        private bool _movingUp;


        private void Start()
        {
            ChangeState(state);
            _currentState.StateEnter(this);
        }

        private void Update()
        {
            RotateRotors();

            _currentState.StateUpdate(this);
        }


        //Function to change bot state
        public void ChangeState(GuardStateEnum newState)
        {
            _currentState?.StateExit(this);
            state = newState;

            switch (state)
            {
                case GuardStateEnum.Guard:
                    _currentState = surveillanceState;
                    _currentState.StateEnter(this);
                    break;
                case GuardStateEnum.Chase:
                    _currentState = chaseState;
                    _currentState.StateEnter(this);
                    break;
            }
        }

        //Function to rotate the rotors around their up with a speed
        private void RotateRotors()
        {
            foreach (Transform rotor in rotors)
            {
                rotor.Rotate(Vector3.up, rotorSpeed * Time.deltaTime, Space.Self);
            }
        }


        //Function to rotate the bot towards the target, ignoring the y axis
        //Use rotation smoothness
        //Rotate the forward vector
        public void Rotate(bool isMoving, Vector3 rotateTowards, float speedMultiplier = 1)
        {
            var transform1 = transform;
            Vector3 direction = rotateTowards - transform1.position;
            direction.y = -1f;

            transform.forward = Vector3.Lerp(transform1.forward, direction, rotationSmoothness * Time.deltaTime * speedMultiplier);
        }
    }

    //Class Called BotChaseState inherit from BotState abstract class
    [System.Serializable]
    public class BotChaseState : BotState
    {
        public float stopDistance;
        public float defaultY;
        public float movementFrequency = 3;
        public float movementAmplitude = 1;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private float _currentSpeed;

        public override void StateEnter(FlyingBot bot)
        {
            bot.animator.CrossFade("Go Forward", 0.2f);
            _currentSpeed = bot.botSpeed;
        }

        public override void StateUpdate(FlyingBot bot)
        {
            Vector3 targetPoint = bot.target.position;
            var position = bot.transform.position;

            //Change the target point y sinusoidally with amplitude 2 and frequency 1, position.y being the center
            targetPoint.y = defaultY + movementAmplitude * Mathf.Sin(movementFrequency * Time.time);

            //Move the bot to target point
            position = Vector3.MoveTowards(position, targetPoint, _currentSpeed * Time.deltaTime);
            bot.transform.position = position;


            //if the distance of the bot and target is less than stopDistance stop the bot and set animator Speed
            if (Vector3.Distance(bot.transform.position, bot.target.position) < stopDistance)
            {
                bot.animator.SetFloat(Speed, 0, 0.2f, Time.deltaTime);
                //Make the currentSpeed to 0 in 1 seconds
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0, 3 * Time.deltaTime);
            }
            else
            {
                bot.animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                //Make the currentSpeed to botSpeed in 1 seconds
                _currentSpeed = Mathf.Lerp(_currentSpeed, bot.botSpeed, 3 * Time.deltaTime);
            }

            //call rotate from bot
            bot.Rotate(_currentSpeed > 0, bot.target.position);
        }

        public override void StateExit(FlyingBot bot)
        {
        }
    }

    //Class called BotSurveillanceState inherit from BotState abstract class, the bot surveills the points in the surveillancePoints array in loop, with a delay between each point
    [System.Serializable]
    public class BotSurveillanceState : BotState
    {
        public int currentSurveillanceIndex;
        public float stoppingDistance = 2;
        public float waitTime;
        public float rotationMultiplier = 0.5f;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private float _currentSpeed;
        private bool _move;
        private Coroutine _waitCoroutine;

        public override void StateEnter(FlyingBot bot)
        {
            bot.animator.CrossFade("Move", 0.2f);
            _currentSpeed = bot.botSpeed;
            _move = true;
        }


        public override void StateUpdate(FlyingBot bot)
        {
            //If wait time is not passed, do nothing
            if (!_move)
            {
                return;
            }

            //Move the bot towards the surveillance point of current index
            bot.transform.position = Vector3.MoveTowards(bot.transform.position,
                bot.surveillancePoints[currentSurveillanceIndex].position, _currentSpeed * Time.deltaTime);


            if (Vector3.Distance(bot.transform.position, bot.surveillancePoints[currentSurveillanceIndex].position) <
                stoppingDistance)
            {
                
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0, 3 * Time.deltaTime);
                bot.animator.SetFloat(Speed, 0, 0.5f, Time.deltaTime);
                
                if (_waitCoroutine == null)
                {
                    // current speed to 0 in 3 seconds
                    _waitCoroutine = bot.StartCoroutine(Wait(bot));
                }
            }
            else
            {
                bot.animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                _currentSpeed = Mathf.Lerp(_currentSpeed, bot.botSpeed, 3 * Time.deltaTime);
            }


            

            bot.Rotate(_currentSpeed > 0, bot.surveillancePoints[currentSurveillanceIndex].position, _currentSpeed/bot.botSpeed * rotationMultiplier);
        }
        
        private IEnumerator Wait(FlyingBot bot)
        {
            float timeElapsed = 0;
            
            while (timeElapsed < waitTime)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            currentSurveillanceIndex = (currentSurveillanceIndex + 1) % bot.surveillancePoints.Length;
            _waitCoroutine = null;
        }

        public override void StateExit(FlyingBot bot)
        {
        }
    }


    //An abstract called BotState with abstract functions called SateEnter, StateUpdate, StateExit
    [System.Serializable]
    public abstract class BotState
    {
        public abstract void StateEnter(FlyingBot bot);
        public abstract void StateUpdate(FlyingBot bot);
        public abstract void StateExit(FlyingBot bot);
    }
}