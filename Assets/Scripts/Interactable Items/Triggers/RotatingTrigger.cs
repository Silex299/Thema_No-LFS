using System;
using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    public class RotatingTrigger : MonoBehaviour
    {

        [SerializeField, FoldoutGroup("Player Action")]
        private Transform playerDefaultTransform;

        [SerializeField, FoldoutGroup("Player Action")]
        private float playerMovementSmoothness = 20;

        [SerializeField, FoldoutGroup("Player Action")]
        private float playerRotationSmoothness = 200;



        [SerializeField, FoldoutGroup("Wheel Action")]
        private Animator wheelAnimator;

        [SerializeField, FoldoutGroup("Wheel Action")]
        private bool invertAxis;


        [SerializeField, FoldoutGroup("Values")]
        private float maximumTriggerValue;

        [SerializeField, FoldoutGroup("Values")]
        private float stepValue;


        [SerializeField, FoldoutGroup("Values"), ProgressBar(0, "maximumTriggerValue", Height = 20), Space(10)]
        private float currentTriggerValue;

        private float CurrentTriggerValue
        {
            get => currentTriggerValue;
            set
            {
                currentTriggerValue = value;
                onTriggerValueChanged?.Invoke(value / maximumTriggerValue);
                OnTriggerValueChangedInternal?.Invoke(value / maximumTriggerValue);
            }
        }

        [SerializeField, BoxGroup("Events"), Space(10)] private UnityEvent<float> onTriggerValueChanged;
        public event Action<float> OnTriggerValueChangedInternal;



        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = false;
            }
        }


        private float _lastAnimationNormalizedTime = 0f;
        /// <summary>
        /// If rotation is in clockwise direction
        /// </summary>
        private bool _lastRotation;
        private bool _animationTriggered;
        private bool _playerIsInTrigger;
        private bool _movePlayer;

        private void Update()
        {

            if (!_playerIsInTrigger) return;


            if (Input.GetAxis("Axis_Interaction") > 0)
            {
                if (CurrentTriggerValue > maximumTriggerValue)
                {
                    if (_animationTriggered)
                    {
                        StartCoroutine(StopWheelTrigger());
                    }
                    return;
                }

                if (!_animationTriggered)
                {
                    StartCoroutine(StartWheelTrigger(true));
                }
                else if (_animationTriggered && !_movePlayer)
                {
                    CurrentTriggerValue += Time.deltaTime * stepValue;
                }

            }
            else if (Input.GetAxis("Axis_Interaction") < 0)
            {

                if (CurrentTriggerValue < 0)
                {
                    if (_animationTriggered)
                    {
                        StartCoroutine(StopWheelTrigger());
                    }
                    return;
                }

                if (!_animationTriggered)
                {
                    StartCoroutine(StartWheelTrigger(false));
                }
                else if (_animationTriggered && !_movePlayer)
                {
                    CurrentTriggerValue -= Time.deltaTime * stepValue;
                }

            }
            else
            {
                if (_animationTriggered)
                {
                    StartCoroutine(StopWheelTrigger());
                }
            }



        }

        private void FixedUpdate()
        {
            if (!_playerIsInTrigger) return;

            if (!_movePlayer) return;

            var playerTransform = PlayerController.Instance.transform;

            var playerPos = playerTransform.position;
            var destinationPos = playerDefaultTransform.position;

            var playerRot = playerTransform.rotation;
            var destinationRot = playerDefaultTransform.rotation;

            playerTransform.position = Vector3.MoveTowards(playerPos, destinationPos, Time.fixedDeltaTime * playerMovementSmoothness);

            playerTransform.rotation = Quaternion.RotateTowards(playerRot, destinationRot, Time.fixedDeltaTime * playerRotationSmoothness);


            if (Vector3.Distance(playerPos, destinationPos) < 0.01f)
            {
                if(Quaternion.Angle(playerRot, destinationRot) < 2f)
                {
                    _movePlayer = false;
                }
            }

        }

        private IEnumerator StartWheelTrigger(bool direction)
        {

            _animationTriggered = true;
            _movePlayer = true;

            if (invertAxis)
            {
                direction = !direction;
            }

            yield return new WaitUntil(() => !_movePlayer);

            if (!_animationTriggered) yield break;

            var normalizeTime = _lastAnimationNormalizedTime;

            if (_lastRotation != direction)
            {
                normalizeTime = 1 - normalizeTime;
                _lastRotation = direction;
            }


            if (direction)
            {
                PlayerController.Instance.Player.AnimationController.CrossFade("Rotate", 0.2f, 1, normalizeTime - 0.05f);
                PlayerController.Instance.Player.canRotate = false;
                yield return new WaitForSeconds(0.1f);
                wheelAnimator.CrossFade("Rotate", 0.0f, 0, normalizeTime);
            }
            else
            {
                PlayerController.Instance.Player.AnimationController.CrossFade("Rotate 0", 0.2f, 1, normalizeTime - 0.05f);
                PlayerController.Instance.Player.canRotate = false;
                yield return new WaitForSeconds(0.1f);
                wheelAnimator.CrossFade("Rotate 0", 0.0f, 0, normalizeTime);
            }



        }

        private IEnumerator StopWheelTrigger()
        {
            _movePlayer = false;
            _animationTriggered = false;

            yield return new WaitForSeconds(0.4f);


            //TODO FIX SOME ANIMATION ERROR

            PlayerController.Instance.Player.AnimationController.CrossFade("Default", 0.2f);
            PlayerController.Instance.Player.canRotate = true;
            _lastAnimationNormalizedTime = wheelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            wheelAnimator.Play("Stop");


        }

        public float GetFraction()
        {
            return currentTriggerValue / maximumTriggerValue;
        }

    }
}
