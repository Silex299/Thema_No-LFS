using System.Collections;
using NPCs.Weapons;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NPCs.FlyingBot
{
    public class FlyingBot : MonoBehaviour
    {
        
        [BoxGroup("Movement")] public Transform[] surveillancePoints;
        [SerializeField, BoxGroup("Movement")] private Transform[] rotors;
        [SerializeField, BoxGroup("Movement"), Space(5)] private float rotorSpeed;
        [BoxGroup("Movement")]public float botSpeed;
        [SerializeField, BoxGroup("Movement")] private float rotationSmoothness;

        [BoxGroup("References")]public Animator animator;
        [BoxGroup("References")] public PowerUpWeapon weapon;
        
        [SerializeField, BoxGroup("States"), EnumToggleButtons, HideLabel] private GuardStateEnum state;
        
        private BotState _currentState;
        [SerializeField, BoxGroup("States")] private BotChaseState chaseState = new BotChaseState();
        [SerializeField, BoxGroup("States")] private BotSurveillanceState surveillanceState = new BotSurveillanceState();


        private bool _movingForward;
        private bool _movingUp;


        private void OnEnable()
        {
            ChangeState(state);
            _currentState.StateEnter(this);
            PlayerMovementController.Instance.player.Health.OnDeath += PlayerDeathCallback;
        }

        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath -= PlayerDeathCallback;
        }

        private void Update()
        {
            RotateRotors();
            
            if (enabled)
            {
                _currentState.StateUpdate(this);
            }
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

        public void ChangeState(int index)
        {
            //Change state according to index, 0 for guard and 1 for chase
            switch (index)
            {
                case 0:
                    ChangeState(GuardStateEnum.Guard);
                    break;
                case 1:
                    ChangeState(GuardStateEnum.Chase);
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

        private void PlayerDeathCallback()
        {
            enabled = false;
        }

    }

    //Class Called BotChaseState inherit from BotState abstract class
    [System.Serializable]
    public class BotChaseState : BotState
    {
       [Tooltip("The distance at which the bot can fire at the target.")]
       public float fireDistance;

       [Tooltip("The distance at which the bot will stop moving towards the target.")]
       public float stopDistance;

       [Tooltip("The distance at which the bot will start moving away from the target if inside stopDistance.")]
       public float safeDistance;

       [Tooltip("The default height at which the bot will hover.")]
       public float defaultY;

       [Tooltip("The frequency of the bot's vertical oscillation when moving.")]
       public float movementFrequency = 3;

       [Tooltip("The amplitude of the bot's vertical oscillation when moving.")]
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
            
            //TODO cleanup
            
            Transform target = PlayerMovementController.Instance.transform;
            Vector3 targetPoint = target.position;
            
            Vector3 botPosition = bot.transform.position;
            float botY = targetPoint.y + defaultY;
            targetPoint.y = botPosition.y;

            float targetDistance = Vector3.Distance(botPosition, targetPoint);

            //if the distance of the bot and target is less than stopDistance stop the bot and set animator Speed
            if (targetDistance < stopDistance)
            {
                targetPoint += (botPosition - targetPoint).normalized * (safeDistance);

                if (targetDistance > safeDistance)
                {
                    Debug.DrawLine(bot.transform.position, targetPoint, Color.green, 1f);
                    bot.animator.SetFloat(Speed, 0, 0.2f, Time.deltaTime);
                    //lerp the currentSpeed to 0 in 1/3 seconds
                    _currentSpeed = Mathf.Lerp(_currentSpeed, 0, 0.5f * Time.deltaTime);
                }
                else
                {
                    Debug.DrawLine(bot.transform.position, targetPoint, Color.red, 1f);
                    bot.animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                    _currentSpeed = Mathf.Lerp(_currentSpeed, bot.botSpeed, 3 * Time.deltaTime);
                }
            }
            else
            {
                targetPoint.y = botY + movementAmplitude * Mathf.Sin(movementFrequency * Time.time);

                bot.animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                _currentSpeed = Mathf.Lerp(_currentSpeed, bot.botSpeed, 3 * Time.deltaTime);
            }


            //If distance is less than firingDistance fire the weapon
            if (targetDistance < fireDistance)
            {
                bot.weapon.Fire(target.position, targetDistance < stopDistance ? -0.5f : 0.5f);
            }


            //Move the bot to target point
            bot.transform.position = Vector3.MoveTowards(botPosition, targetPoint, _currentSpeed * Time.deltaTime);
            

            //call rotate from bot
            bot.Rotate(_currentSpeed > 0, target.position);
            bot.weapon.LookForPlayer(target);
        }

        public override void StateExit(FlyingBot bot)
        {
        }

    }

    //Class called BotSurveillanceState inherit from BotState abstract class, the bot surveil  the points in the surveillancePoints array in loop, with a delay between each point
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
                
                _waitCoroutine ??= bot.StartCoroutine(Wait(bot));
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