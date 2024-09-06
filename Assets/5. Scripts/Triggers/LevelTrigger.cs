using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(Collider))]
    public class LevelTrigger : MonoBehaviour
    {
        #region Anim Params
        
        [FoldoutGroup("Animation Params")] public string trigger_Pull_Enter = "Trigger_Pull_Enter";
        [FoldoutGroup("Animation Params")] public string trigger_Push_Enter = "Trigger_Push_Enter";
        [FoldoutGroup("Animation Params")] public string trigger_Pull_Enter_Inverse = "Trigger_Pull_Enter_Inv";
        [FoldoutGroup("Animation Params")] public string trigger_Push_Enter_Inverse = "Trigger_Push_Enter_Inv";
        [FoldoutGroup("Animation Params")] public float engageTransitionTime = 0.5f;
        [FoldoutGroup("Animation Params")] public float engageDelay = 1f;
        [FoldoutGroup("Animation Params")] public float triggerDelay = 1f;
        
        #endregion
        #region Trigger Params

        [SerializeField, FoldoutGroup("Trigger Params")]
        private bool triggerPulled;

        [SerializeField, FoldoutGroup("Trigger Params")]
        private Animator leverAnimator;

        [SerializeField, FoldoutGroup("Trigger Params")]
        private float actionDelay;

        [SerializeField, FoldoutGroup("Player Movement")]
        private Transform forwardPosition;

        [SerializeField, FoldoutGroup("Player Movement")]
        private Transform backwardPosition;

        [SerializeField, FoldoutGroup("Player Movement")]
        private float movementSpeed;

        [SerializeField, FoldoutGroup("Player Movement")]
        private float rotationSpeed;
        
        #endregion
        #region Sounds
        [SerializeField, FoldoutGroup("Sounds")] private AudioSource soundSource;
        [SerializeField, FoldoutGroup("Sounds")] private SoundClip triggerSound;
        [SerializeField, FoldoutGroup("Sounds")] private SoundClip disabledResetSound;
        [SerializeField, FoldoutGroup("Sounds")] private SoundClip disabledTriggerSound;
        #endregion
        #region Events
        [SerializeField, FoldoutGroup("Events")] private UnityEvent engageAction;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent triggerPullAction;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent triggerPushAction;
        #endregion
        #region Other Variables
        
        [FoldoutGroup("Misc")] public bool canPull = true;
        #endregion
        
        #region Other Variables
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
        
        #endregion
        
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
        
        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if (Input.GetButtonDown("e"))
            {
                if (_triggerEngaged) return;
                _engageCoroutine ??= StartCoroutine(EngageTrigger());
            }
        }



        private void Start()
        {
            //Set Initial State
            leverAnimator.Play(triggerPulled ? "Trigger" : "Trigger Inverse");
            _defaultPull = triggerPulled;
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
                    //PUSH
                    playerController.player.AnimationController.SetTrigger(Trigger1);

                    yield return TriggerPushCoroutine();

                }
                else if (!triggerPulled && Input.GetAxis("Horizontal") < 0)
                {
                    //PULL  
                    playerController.player.AnimationController.SetTrigger(Trigger1);

                    yield return TriggerPullCoroutine();
                }

                yield return null;
            }

            if (!canPull)
            {
                //reset the trigger
                triggerPulled = _defaultPull;
                leverAnimator.Play(triggerPulled ? "Trigger" : "Trigger Inverse");
                if(soundSource) soundSource.PlayOneShot(disabledResetSound.clip, disabledResetSound.volume);
                //Play reset sound probably
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
        public void Reset()
        {
            _triggerEngaged = false;
            if (PlayerMovementController.Instance.player.CController.enabled)
            {
                PlayerMovementController.Instance.PlayAnimation("Default", 0.5f, 1);
                PlayerMovementController.Instance.DisablePlayerMovement(false);
            }
        }
        public void TriggerPull()
        {
            StartCoroutine(TriggerPullCoroutine());
        }
        private IEnumerator TriggerPullCoroutine()
        {
            //PULL  
            if(triggerPulled) yield break;
                    
            ChangeTriggerState(true);
            if (soundSource)
            {
                soundSource.PlayOneShot(triggerSound.clip, triggerSound.volume);
            }

            if (canPull)
            {
                yield return new WaitForSeconds(triggerDelay);
                triggerPullAction.Invoke();
            }
        }
        public void TriggerPush()
        {
            StartCoroutine(TriggerPushCoroutine());
        }
        private IEnumerator TriggerPushCoroutine()
        {
            if(!triggerPulled) yield break;
            
            ChangeTriggerState(false);
            if (soundSource)
            {
                soundSource.PlayOneShot(disabledTriggerSound.clip, disabledTriggerSound.volume);
            }

            if (canPull)
            {
                yield return new WaitForSeconds(triggerDelay);
                triggerPushAction.Invoke();
            }
        }
        private void ChangeTriggerState(bool pulled)
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