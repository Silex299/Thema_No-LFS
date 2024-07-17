using System.Collections;
using System.Net.NetworkInformation;
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
        [BoxGroup("Animation Params")] public string trigger_Pull_Enter_Inverse="Trigger_Pull_Enter_Inv";
        [BoxGroup("Animation Params")] public string trigger_Push_Enter_Inverse = "Trigger_Push_Enter_Inv";
        
        [SerializeField, BoxGroup("Trigger Params")] private bool triggerPulled;
        [SerializeField, BoxGroup("Trigger Params")] private Animator leverAnimator;

        [SerializeField, BoxGroup("Trigger Params")]
        private float actionDelay;
        [SerializeField, BoxGroup("Trigger Params")] private float leverPullDelay = 1;

        [SerializeField, BoxGroup("Player Movement")] private Transform forwardPosition;
        [SerializeField, BoxGroup("Player Movement")] private Transform backwardPosition;
        [SerializeField, BoxGroup("Player Movement")] private float movementSpeed;
        [SerializeField, BoxGroup("Player Movement")] private float rotationSpeed;


        [SerializeField, BoxGroup("Events")] private UnityEvent triggerPullAction;
        [SerializeField, BoxGroup("Events")] private UnityEvent triggerPushAction;

        [SerializeField, BoxGroup("Sounds")] private AudioSource soundSource;
        [SerializeField, BoxGroup("Sounds")] private AudioClip triggerSound;
        [SerializeField, BoxGroup("Sounds")] private AudioClip disabledTriggerSound;

        [Space(10)] public bool canPull = true;

        public bool CanPull
        {
            get => canPull;
            set=> canPull = value;
        }
        private bool _playerIsInTrigger;
        private bool _triggerEngaged;
        private float _lastTriggerTime;

        /// <summary>
        /// If player is facing forward direction
        /// </summary>

        private bool _forwardDirection;

        private bool _defaultPull;
        private Coroutine _trigger;

        private void OnTriggerEnter(Collider other)
        {
            if(!CanPull) return;
            
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


        private void Start()
        {
            //Set Initial State
            leverAnimator.Play(triggerPulled ? "Trigger" : "Trigger Inverse");
            _defaultPull = triggerPulled;
        }

        private void Update()
        {

            if (!_playerIsInTrigger) return;

            if (Input.GetButtonUp("e") )
            {
                if (_lastTriggerTime + 0.8f > Time.time)
                {
                    Invoke(nameof(Reset), _lastTriggerTime + 0.8f - Time.time);
                }
                else
                {
                    Reset();
                }
            }
            
            if (Input.GetButton("e"))
            {
                if (!CanPull) return;

                var player = PlayerMovementController.Instance;

                if (!_triggerEngaged)
                {
                    _triggerEngaged = true;
                    _lastTriggerTime = Time.time;
                    player.DisablePlayerMovement(true);

                    _forwardDirection = CheckPlayerDirection();
                
                    if (_forwardDirection)
                    {
                        player.PlayAnimation(!triggerPulled ? "Trigger_Pull_Enter" : "Trigger_Push_Enter", 0.2f, 1);
                    }
                    else
                    {
                        player.PlayAnimation(!triggerPulled ? "Trigger_Pull_Enter_Inv" : "Trigger_Push_Enter_Inv", 0.2f, 1);
                    }

                }
                else
                {
                    if (_lastTriggerTime + 0.5f > Time.time) return;

                    var input = Input.GetAxis("Horizontal");

                    if (input < 0 && !triggerPulled)
                    {
                        _lastTriggerTime = Time.time;
                        triggerPulled = true;

                        player.SetAnimationTrigger("Trigger");
                        leverAnimator.Play("Trigger");

                        _trigger ??= StartCoroutine(Trigger());
                    }
                    else if (input > 0 && triggerPulled)
                    {
                        _lastTriggerTime = Time.time;
                        triggerPulled = false;
                        player.SetAnimationTrigger("Trigger");
                        leverAnimator.Play("Trigger Inverse");

                        _trigger ??= StartCoroutine(Trigger());
                    }
                }


            }
        }

        private void LateUpdate()
        {
            if (!canPull) return;

            if (_triggerEngaged && PlayerMovementController.Instance.player.CController.enabled)
            {
                var player = PlayerMovementController.Instance;
                if (_forwardDirection)
                {
                    //Move to position 1
                    var playerTransform = PlayerMovementController.Instance.transform;

                    player.transform.position = Vector3.Lerp(playerTransform.position, forwardPosition.position, Time.deltaTime * movementSpeed);
                    player.transform.rotation = Quaternion.Lerp(playerTransform.rotation, forwardPosition.rotation, Time.deltaTime * rotationSpeed);
                }
                else
                {

                    var playerTransform = player.transform;

                    playerTransform.position = Vector3.Lerp(playerTransform.position, backwardPosition.position, Time.deltaTime * movementSpeed);
                    playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, backwardPosition.rotation, Time.deltaTime * rotationSpeed);
                    //move to position 2
                }
            }
        }

        private IEnumerator Trigger()
        {

            if (!canPull)
            {
                yield return new WaitUntil(() => !_triggerEngaged);
            
                _lastTriggerTime = Time.time;
                triggerPulled = false;
            
                leverAnimator.speed = 3;
                leverAnimator.Play("Trigger Inverse");
                if (soundSource)
                {
                    soundSource.PlayOneShot(disabledTriggerSound);
                }
                yield return new WaitForSeconds(0.5f);
                leverAnimator.speed = 1;
                _trigger = null;
            }
            else
            {
                yield return new WaitForSeconds(leverPullDelay);

                if (soundSource)
                {
                    soundSource.PlayOneShot(triggerSound);
                }

                yield return new WaitForSeconds(actionDelay);

                if (triggerPulled)
                {
                    triggerPullAction?.Invoke();
                }
                else
                {
                    triggerPushAction?.Invoke();
                }

                _trigger = null;
            }
        
        
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

        private bool CheckPlayerDirection()
        {
            Vector3 v1 = transform.forward;
            Vector3 v2 = PlayerMovementController.Instance.transform.forward;

            v1.y = 0;
            v2.y = 0;

            var angle = Vector3.Angle(v1, v2);



            return angle < 90;
        }

        public void ChangeState(bool pulled)
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
