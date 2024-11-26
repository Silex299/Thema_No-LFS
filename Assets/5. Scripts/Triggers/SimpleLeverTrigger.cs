using System.Collections;
using System.Linq;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class SimpleLeverTrigger : MonoBehaviour
    {

        [FoldoutGroup("References")] public Animator leverAnimator;
        
        [FoldoutGroup("Action")] public string engageInput;
        [FoldoutGroup("Action")] public string actionAxis;
        [FoldoutGroup("Action")] public bool invertActionAxis;
        [FoldoutGroup("Action")] public float transitionTime = 0.2f;
        [FoldoutGroup("Action")] public TriggerCondition[] conditions;
        
        [FoldoutGroup("Action"), Space(10)] public Transform actionTransform;
        [FoldoutGroup("Action")] public bool alignBoneSocket;
        [FoldoutGroup("Action"), ShowIf(nameof(alignBoneSocket))] public Transform socketTransform;
        [FoldoutGroup("Action"), ShowIf(nameof(alignBoneSocket))]
        public HumanBodyBones alignBone;

        [FoldoutGroup("Animation")] public string engageAnimName;
        [FoldoutGroup("Animation")] public string reverseEngageAnimName;

        [FoldoutGroup("Animation")] public string actionAnimName;
        [FoldoutGroup("Animation")] public string reverseActionAnimName;
        
        [FoldoutGroup("Animation")] public float engageAnimTime;

        [FoldoutGroup("Sound")] public AudioSource source;
        [FoldoutGroup("Sound")] public SoundClip triggerSound;
        [FoldoutGroup("Sound")] public SoundClip unTriggerSound;

        [FoldoutGroup("Events"), Tooltip("After what time on trigger actions are called")] public float actionTime;
        [FoldoutGroup("Events"), Tooltip("Time before the player can pull/push the lever again")] public float secondActionDelay = 1;
        [FoldoutGroup("Events")] public UnityEvent onTrigger;
        [FoldoutGroup("Events")] public UnityEvent onUnTrigger;
        
        [SerializeField, FoldoutGroup("Misc")] public bool oneTime;
        [field: SerializeField, FoldoutGroup("Misc")]
        public bool IsTriggered { get; set; }
        [field: SerializeField, FoldoutGroup("Misc")]
        public bool CanTrigger { get; set; } = true;

        private bool _initiallyTriggered;
        private int _colliderCount;
        private bool _playerInTrigger;
        private bool _isEngaged;
        private Coroutine _playerEngageCoroutine;
        private static readonly int Trigger1 = Animator.StringToHash("Trigger");
        private Player Player => PlayerMovementController.Instance.player;

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;

            if (!other.CompareTag("Player")) return;
            _colliderCount++;
            _playerInTrigger = _colliderCount > 0;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player")) return;
            _colliderCount = Mathf.Clamp(_colliderCount - 1, 0, int.MaxValue);
            _playerInTrigger = _colliderCount > 0;
        }


        private void Start()
        {
            _initiallyTriggered = IsTriggered;
        }
        private void Update()
        {
            if (!_playerInTrigger) return;
            if(oneTime && IsTriggered) return;

            if (conditions.Any(condition => !condition.Condition(Player.CController))) return;

            if (Input.GetButtonDown(engageInput))
            {
                _playerEngageCoroutine ??= StartCoroutine(PlayerEngage());
            }
        }
        private void LateUpdate()
        {
            if(_isEngaged && alignBoneSocket) LockPosition();
        }

        private IEnumerator PlayerEngage()
        {
            Player.DisabledPlayerMovement = true;
            Player.AnimationController.CrossFade(IsTriggered ? reverseEngageAnimName : engageAnimName, 0.1f, 1);
            yield return MovePlayer();

            while (Input.GetButton(engageInput))
            {
                _isEngaged = true;
                
                float actionInput = Input.GetAxis(actionAxis);

                if (invertActionAxis) actionInput *= -1;

                if (!IsTriggered && actionInput < -0.5f)
                {
                    yield return Action();
                }
                else if (IsTriggered && actionInput > 0.5f)
                {
                    yield return Action();
                }
                
                yield return new WaitForEndOfFrame();
            }


            if (!CanTrigger)
            {
                LeverAnim(_initiallyTriggered);
                IsTriggered = _initiallyTriggered;
            }
            
            
            _isEngaged = false;
            Player.AnimationController.CrossFade("Default", 0.25f, 1);
            Player.DisabledPlayerMovement = false;
            _playerEngageCoroutine = null;
        }
        
        private IEnumerator MovePlayer()
        {
            //move player to actionTransform

            float timeElapsed = 0;
            Vector3 initPlayerPos = Player.transform.position;
            Quaternion initPlayerRot = Player.transform.rotation;



            if (alignBoneSocket)
            {
                Transform socketBone = Player.AnimationController.GetBoneTransform(alignBone);
                var playerTransform = Player.transform;
                Vector3 desiredPos = socketTransform.position + (playerTransform.position - socketBone.position);
                
                while (timeElapsed < transitionTime)
                {
                    timeElapsed += Time.deltaTime;
                
                    if(alignBoneSocket) desiredPos = socketTransform.position + (playerTransform.position - socketBone.position);
                
                    Player.transform.position = Vector3.Lerp(initPlayerPos, desiredPos, timeElapsed / transitionTime);
                    Player.transform.rotation = Quaternion.Slerp(initPlayerRot, actionTransform.rotation, timeElapsed / transitionTime);

                    yield return new WaitForEndOfFrame();
                }

                Player.transform.position = desiredPos;
                Player.transform.rotation = actionTransform.rotation;

            }
            else
            {
                Vector3 desiredPos = actionTransform.position;
                
                while (timeElapsed < transitionTime)
                {
                    timeElapsed += Time.deltaTime;
                    Player.transform.position = Vector3.Lerp(initPlayerPos, desiredPos, timeElapsed / transitionTime);
                    Player.transform.rotation = Quaternion.Slerp(initPlayerRot, actionTransform.rotation, timeElapsed / transitionTime);

                    yield return new WaitForEndOfFrame();
                }

                Player.transform.position = desiredPos;
                Player.transform.rotation = actionTransform.rotation;

            }
            
            yield return new WaitForSeconds(engageAnimTime);
        }

        private void LockPosition()
        {
            Vector3 socketPos = Player.AnimationController.GetBoneTransform(alignBone).position;
            var playerPos = Player.transform.position;
            Vector3 desiredPos = socketTransform.position + (playerPos - socketPos);

            Player.transform.position = desiredPos;
            Player.transform.rotation = actionTransform.rotation;
        }


        private IEnumerator Action()
        {
            
            PlayerAnim(!IsTriggered);
            LeverAnim(!IsTriggered);
            
            yield return new WaitForSeconds(actionTime);
            
            IsTriggered = !IsTriggered;
            
            if (CanTrigger)
            {
                if (IsTriggered)
                {
                    onTrigger.Invoke();
                }
                else
                {
                    onUnTrigger.Invoke();
                }
                PlaySound(IsTriggered);
            }
            yield return new WaitForSeconds(secondActionDelay);
            
        }

        private void PlayerAnim(bool trigger)
        {
            Player.AnimationController.CrossFade(trigger ? actionAnimName : reverseActionAnimName, 0.2f, 1);
        }
        
        private void LeverAnim(bool trigger)
        {
            if(trigger == IsTriggered) return;
            
            leverAnimator?.Play(trigger? "Pull" : "Push");
        }

        private void PlaySound(bool trigger)
        {
            if (source)
            {
                source.PlayOneShot(trigger ? triggerSound.clip : unTriggerSound.clip, trigger? triggerSound.volume : unTriggerSound.volume);
            }
        }


        public void Reset()
        {
            _colliderCount = 0;
            _playerInTrigger = false;
            _isEngaged = false;
            _playerEngageCoroutine = null;
            Invoke(nameof(DelayedReset), 0.5f);
        }

        private void DelayedReset()
        {
            IsTriggered = _initiallyTriggered;
            leverAnimator?.Play(IsTriggered ? "Pull" : "Push", 0, 1f);
        }


        public void Set()
        {
            _colliderCount = 0;
            _playerInTrigger = false;
            _isEngaged = false;
            _playerEngageCoroutine = null;
            Invoke(nameof(DelayedSet), 0.5f);
        }

        public void DelayedSet()
        {
            IsTriggered = !_initiallyTriggered;
            leverAnimator?.Play(IsTriggered ? "Pull" : "Push", 0, 1f);
        }
        
    }
}