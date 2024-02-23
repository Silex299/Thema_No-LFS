using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items
{
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerSwitch : MonoBehaviour
    {

        #region Variables

        #region Non Exposed Variables 

        [HideInInspector] public bool isActive = true;
        [SerializeField] private bool state;
        private bool _movePlayer;
        private bool _isHooked;
        private bool _playedAction;
        private bool _playerIsInTrigger;
        private Transform _target;

        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region Input
        [SerializeField, BoxGroup("Input")] private string interactInput;
        [SerializeField, BoxGroup("Input")] private string triggerInput;
        [SerializeField, BoxGroup("Input")] private float timeToAct = 1f;
        #endregion

        #region Movement

        [SerializeField, BoxGroup("Movement"), InfoBox("If initially the trigger is on or off")] private bool invertedTrigger;
        [SerializeField, BoxGroup("Movement")] private bool invertedAxis;
        [SerializeField, BoxGroup("Movement")] private bool oneWay;
        [SerializeField, BoxGroup("Movement")] private float movementSmoothness;
        [SerializeField, BoxGroup("Movement")] private float rotationSmoothness;
        [SerializeField, BoxGroup("Movement")] private Transform movePlayerTo;

        #endregion

        #region Action Names
        [SerializeField, TabGroup("Actions", "PlayerAnimations")] private string enterAnimationName;
        [SerializeField, TabGroup("Actions", "PlayerAnimations")] private string actionAnimationName;

        [SerializeField, TabGroup("Actions", "PlayerAnimations"), HideIf("oneWay")] private string inverseEnterAnimationName;

        [SerializeField, TabGroup("Actions", "PlayerAnimations"), HideIf("oneWay")] private string inverseActionName;
        #endregion



        #region Trigger Animation
        //[SerializeField, TabGroup("Actions", "TriggerAnimation")] private float triggerAnimationDelay = 1f;

        [SerializeField, TabGroup("Actions", "TriggerAnimation")] private Animator triggerAnimator;
        [SerializeField, TabGroup("Actions", "TriggerAnimation")] private string triggerAction;
        [SerializeField, TabGroup("Actions", "TriggerAnimation"), HideIf("oneWay")] private string inverseTriggerAction;

        #endregion

        #region Events
        [SerializeField] private List<TimedAction> triggers;
        
        [System.Serializable]
        public struct TimedAction
        {
            public UnityEvent<bool> onTrigger;
            public float time;
        }
        
        #endregion

        #endregion

        #region Builtin methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") && !other.CompareTag("Player_Main")) return;
            _target = Player_Scripts.PlayerController.Instance.transform;
            _playerIsInTrigger = true;

        }
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            if (!other.GetComponent<Player_Scripts.PlayerController>()) return;
            _movePlayer = false;
            _playerIsInTrigger = false;

        }

        private float _triggerTime;
        private void Update()
        {

            if (!_playerIsInTrigger) return;


            //Exit Trigger
            if (Input.GetButtonUp(interactInput))
            {
                if (_playedAction) return;
                StartCoroutine(ExitTrigger());
            }

            //Don't check for further action if trigger is one way and action is already performed
            if (state && oneWay) return;

            //Engage Trigger
            if (Input.GetButtonDown(interactInput))
            {
                _movePlayer = true;

            }



            if (!_isHooked) return;


            var player = Player_Scripts.PlayerController.Instance.Player;
            player.CanMove = false;
            player.canRotate = false;
            player.AnimationController.SetFloat(Speed, 0);


            if (!state)
            {
                if ((Input.GetAxis(triggerInput) < 0 && !invertedAxis) || (Input.GetAxis(triggerInput) > 0 && invertedAxis))
                {
                    if (_playedAction) return;

                    if (_triggerTime == 0)
                    {
                        _triggerTime = Time.time;
                    }

                    if (_triggerTime + timeToAct < Time.time)
                    {
                        StartCoroutine(PlayAction(actionAnimationName, triggerAction));
                    }
                }
                else
                {
                    _triggerTime = 0;
                }
            }
            else
            {
                if ((Input.GetAxis(triggerInput) > 0 && !invertedAxis) || (Input.GetAxis(triggerInput) < 0 && invertedAxis))
                {
                    if (_playedAction) return;

                    if (_triggerTime == 0)
                    {
                        _triggerTime = Time.time;
                    }

                    if (_triggerTime + timeToAct < Time.time)
                    {
                        StartCoroutine(PlayAction(inverseActionName, inverseTriggerAction));
                    }
                }
                else
                {
                    _triggerTime = 0;
                }
            }


        }
        private void LateUpdate()
        {
            if (!_movePlayer) return;


            Vector3 pos = _target.position;
            Quaternion rot = _target.rotation;

            Vector3 pos1 = movePlayerTo.position;
            Quaternion rot1 = movePlayerTo.rotation;


            if (!_isHooked)
            {
                EnterAction();
            }
            if (Mathf.Abs((pos - pos1).magnitude) == 0)
            {
                if (Mathf.Abs((rot.eulerAngles - rot1.eulerAngles).magnitude) == 0)
                {
                    _movePlayer = false;
                    return;
                }
            }

            _target.position = Vector3.MoveTowards(pos, pos1, Time.deltaTime * movementSmoothness);
            _target.rotation = Quaternion.RotateTowards(rot, rot1, Time.deltaTime * rotationSmoothness);


        }


        #endregion

        #region Custom Methods

        private void EnterAction()
        {
            var player = Player_Scripts.PlayerController.Instance.Player;

            player.AnimationController.CrossFade(!state ? enterAnimationName : inverseEnterAnimationName, 0.2f, 1);

            _isHooked = true;
        }

        private IEnumerator PlayAction(string anim, string triggerAnimation)
        {

            _playedAction = true;
            state = !state;

            triggerAnimator.CrossFade(triggerAnimation, 0f);
            Player_Scripts.PlayerController.Instance.Player.AnimationController.CrossFade(anim, 0.5f, 1);



            yield return new WaitForSeconds(1f);

            StartCoroutine(ExecuteActions());
            _movePlayer = false;
            _playedAction = false;
            _triggerTime = 0;

        }
        
        private IEnumerator ExecuteActions()
        {
            foreach (var trigger in triggers)
            {
                trigger.onTrigger?.Invoke(invertedTrigger ? !state : state);
                yield return new WaitForSeconds(trigger.time);
            }
        }

       

        private IEnumerator ExitTrigger()
        {
            var player = Player_Scripts.PlayerController.Instance;
            player.Player.AnimationController.CrossFade("Default", 0.4f, 1);
            _movePlayer = false;
            _isHooked = false;
            _playedAction = false;
            _triggerTime = 0;

            yield return new WaitForSeconds(0.5f);

            player.Player.CanMove = true;
            player.Player.canRotate = true;
        }

        #endregion


    }
}
