using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(Collider))]
    public class LevelTrigger : MonoBehaviour
    {
        [BoxGroup("Animation Params")] public string trigger_Pull_Enter = "Trigger_Pull_Enter";
        [BoxGroup("Animation Params")] public string trigger_Push_Enter = "Trigger_Push_Enter";
        [BoxGroup("Animation Params")] public string trigger_Pull_Enter_Inverse = "Trigger_Pull_Enter_Inv";
        [BoxGroup("Animation Params")] public string trigger_Push_Enter_Inverse = "Trigger_Push_Enter_Inv";
        [BoxGroup("Animation Params")] public float engageTransitionTime = 0.5f;
        [BoxGroup("Animation Params")] public float engageDelay = 1f;
        [BoxGroup("Animation Params")] public float triggerDelay = 1f;

        [SerializeField, BoxGroup("Trigger Params")]
        private bool triggerPulled;

        [SerializeField, BoxGroup("Trigger Params")]
        private Animator leverAnimator;

        [SerializeField, BoxGroup("Trigger Params")]
        private float actionDelay;

        [SerializeField, BoxGroup("Player Movement")]
        private Transform forwardPosition;

        [SerializeField, BoxGroup("Player Movement")]
        private Transform backwardPosition;

        [SerializeField, BoxGroup("Player Movement")]
        private float movementSpeed;

        [SerializeField, BoxGroup("Player Movement")]
        private float rotationSpeed;


        [SerializeField, BoxGroup("Events")] private UnityEvent engageAction;
        [SerializeField, BoxGroup("Events")] private UnityEvent triggerPullAction;
        [SerializeField, BoxGroup("Events")] private UnityEvent triggerPushAction;

        [SerializeField, BoxGroup("Sounds")] private AudioSource soundSource;
        [SerializeField, BoxGroup("Sounds")] private AudioClip triggerSound;
        [SerializeField, BoxGroup("Sounds")] private AudioClip disabledTriggerSound;

        [Space(10)] public bool canPull = true;

        public bool CanPull
        {
            get => canPull;
            set => canPull = value;
        }

        public string Trigger_Pull_Enter
        {
            set => trigger_Pull_Enter = value;
        }

        public string Trigger_Pull_Enter_Inverse
        {
            set => trigger_Pull_Enter_Inverse = value;
        }

        private bool _playerIsInTrigger;
        private bool _triggerEngaged;
        private float _lastTriggerTime;

        /// <summary>
        /// If player is facing forward direction
        /// </summary>
        private bool _forwardDirection;

        private bool _defaultPull;
        private Coroutine _engageCoroutine;
        private static readonly int Trigger1 = Animator.StringToHash("Trigger");


        private void OnTriggerEnter(Collider other)
        {
            if (!CanPull) return;

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

        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if (Input.GetButtonDown("e"))
            {
                if (_triggerEngaged) return;
                _engageCoroutine ??= StartCoroutine(EngageTrigger());
            }
        }

        private IEnumerator EngageTrigger()
        {
            PlayerMovementController playerController = PlayerMovementController.Instance;


            bool isForwardPoint = Vector3.Distance(playerController.transform.position, forwardPosition.position) < Vector3.Distance(playerController.transform.position, backwardPosition.position);
            Transform engagePoint = isForwardPoint ? forwardPosition : backwardPosition;

            //Check distance between player and forward position and backward position
            if (isForwardPoint)
            { 
                playerController.PlayAnimation(!triggerPulled? trigger_Pull_Enter : trigger_Push_Enter, 0.2f, 1);
            }
            else
            {
                playerController.PlayAnimation(!triggerPulled? trigger_Pull_Enter_Inverse : trigger_Push_Enter_Inverse, 0.2f, 1);
            }
            
            playerController.DisablePlayerMovement(true);
            playerController.player.CController.enabled = false;
            playerController.player.CanRotate = false;

            Vector3 initialPos = playerController.transform.position;
            Quaternion initialRot = playerController.transform.rotation;
                
                
            float timeElapsed = 0;
            while (timeElapsed <= engageTransitionTime)
            {
                timeElapsed += Time.deltaTime;

                playerController.transform.position = Vector3.Lerp(initialPos, engagePoint.position,
                    timeElapsed / engageTransitionTime);
                playerController.transform.rotation = Quaternion.Slerp(initialRot, engagePoint.rotation,
                    timeElapsed / engageTransitionTime);

                yield return null;
            }
            
            
            yield return new WaitForSeconds(engageDelay - engageTransitionTime);

            engageAction.Invoke();
            _triggerEngaged = true;

            while (Input.GetButton("e"))
            {
                if (triggerPulled && Input.GetAxis("Horizontal") > 0)
                {
                    playerController.player.AnimationController.SetTrigger(Trigger1);
                    ChangeTriggerState(false);

                    if (soundSource)
                    {
                        soundSource.PlayOneShot(disabledTriggerSound);
                    }
                    
                    yield return new WaitForSeconds(triggerDelay);
                    triggerPushAction.Invoke();
                    
                }
                else if (!triggerPulled && Input.GetAxis("Horizontal") < 0)
                {
                    print("pulling");

                    playerController.player.AnimationController.SetTrigger(Trigger1);
                    ChangeTriggerState(true);
                    if (soundSource)
                    {
                        soundSource.PlayOneShot(triggerSound);
                    }

                    
                    yield return new WaitForSeconds(triggerDelay);
                    triggerPullAction.Invoke();
                }

                yield return null;
            }

            //If you don't get e input, go back to default;
            playerController.PlayAnimation("Default", 0.2f, 1);

            yield return new WaitForSeconds(0.2f);

            playerController.DisablePlayerMovement(false);
            playerController.player.CController.enabled = true;
            playerController.player.CanRotate = true;
            
            _triggerEngaged = false;
            _engageCoroutine = null;
        }


        private void Start()
        {
            //Set Initial State
            leverAnimator.Play(triggerPulled ? "Trigger" : "Trigger Inverse");
            _defaultPull = triggerPulled;
        }


        public void Reset()
        {
            _triggerEngaged = false;
            if (PlayerMovementController.Instance.player.CController.enabled)
            {
                PlayerMovementController.Instance.PlayAnimation("Default", 0.5f, 1);
                PlayerMovementController.Instance.DisablePlayerMovement(false);
            }
        }
        
        public void ChangeTriggerState(bool pulled)
        {
            if (triggerPulled == pulled) return;
            triggerPulled = pulled;
            leverAnimator.Play(pulled ? "Trigger" : "Trigger Inverse");
        }

        public void ResetTrigger()
        {
            CanPull = true;
            triggerPulled = _defaultPull;
            leverAnimator.Play(triggerPulled ? "Trigger" : "Trigger Inverse");
        }
    }
}