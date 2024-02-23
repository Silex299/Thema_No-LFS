using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items
{
    [RequireComponent(typeof(BoxCollider))]
    public class BreakableBarrier : MonoBehaviour
    {

        #region Variables     

        #region Non exposed variables  

        private bool _playerIsInTrigger;
        private Transform _target;

        private bool _movePlayer;
        private bool _isHooked;
        private bool _playingAction;

        private bool barrierStatus;
        private int currentBreakableIndex;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private float _time;

        #endregion

        [SerializeField, BoxGroup("Player Movement")] private Transform movePlayerTo;
        [SerializeField, BoxGroup("Player Movement")] private float movementSmoothness;
        [SerializeField, BoxGroup("Player Movement")] private float rotationSmoothness;
        [SerializeField, BoxGroup("Player Movement")] private float timeToAct;

        [SerializeField, BoxGroup("Player Animation")] private string enterAnimation = "Pull Start";
        [SerializeField, BoxGroup("Player Animation")] private string pullAnimation = "Pull fall backward";


        [SerializeField, BoxGroup("Input")] private string actionEnterInput;
        [SerializeField, BoxGroup("Input")] private string actionInput;

        [SerializeField, BoxGroup("Misc")] private float exitCrossfadeTime = 0.2f;

        [SerializeField, BoxGroup("Breakable"), InfoBox("Disable gravity and enable kinematics")] private List<Barrier> barriers;

        [SerializeField, BoxGroup("Event")] private UnityEvent onBarrierDown;

        #endregion


        #region Built-in methods

        private void OnTriggerEnter(Collider other)
        {
            if (barrierStatus) return;

            if (other.CompareTag("Player") || other.CompareTag("Player_Main"))
            {
                _target = Player_Scripts.PlayerController.Instance.transform;
                _playerIsInTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (barrierStatus) return;

            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = false;
                _movePlayer = false;
            }
        }


        private void Update()
        {
            if (barrierStatus) return;
            if (!_playerIsInTrigger) return;

            if (Input.GetButtonDown(actionEnterInput))
            {
                _movePlayer = true;
            }
            if (Input.GetButtonUp(actionEnterInput))
            {
                ExitAction();
            }


            if (!_isHooked) return;
            if (_playingAction) return;

            if (Input.GetButton(actionInput))
            {
                if (_time == 0)
                {
                    _time = Time.time + timeToAct;
                }

                if (Time.time > _time)
                {
                    _time = 0;
                    StartCoroutine(PlayAction());
                }
            }

        }

        private void LateUpdate()
        {
            if (barrierStatus) return;
            if (!_movePlayer) return;


            var player = Player_Scripts.PlayerController.Instance.Player;

            player.CanMove = false;
            player.canRotate = false;
            player.AnimationController.SetFloat(Speed, 0);



            var targetPos = _target.position;
            var targetRot = _target.rotation;

            var destinationPos = movePlayerTo.position;
            var destinationRot = movePlayerTo.rotation;

            _target.position = Vector3.MoveTowards(targetPos, destinationPos, Time.deltaTime * movementSmoothness);
            _target.rotation = Quaternion.RotateTowards(targetRot, destinationRot, Time.deltaTime * rotationSmoothness);

            if (!_isHooked)
            {
                StartCoroutine(EnterAction());
            }

            if (Mathf.Abs((destinationPos - targetPos).magnitude) < 0.01f)
            {
                if (Mathf.Abs((destinationRot.eulerAngles - targetRot.eulerAngles).magnitude) == 0)
                {
                    _movePlayer = false;
                }
            }

        }

        #endregion

        #region Methods

        private IEnumerator EnterAction()
        {
            var player = Player_Scripts.PlayerController.Instance.Player;
            player.AnimationController.CrossFade(enterAnimation, 0.4f, 1);

            player.Rig.SetLeftHandTarget(barriers[currentBreakableIndex].leftHandIKTarget);
            player.Rig.SetRightHandTarget(barriers[currentBreakableIndex].rightHandIKTarget);

            _isHooked = true;
            _playingAction = true;

            yield return new WaitForSeconds(1f);

            _playingAction = false;

        }
        private IEnumerator PlayAction()
        {
            _playingAction = true;
            Player_Scripts.PlayerController.Instance.Player.AnimationController.CrossFade(pullAnimation, 0.4f, 1);

            yield return new WaitForSeconds(0.8f);

            barriers[currentBreakableIndex].BreakBarrier();
            currentBreakableIndex++;
            barrierStatus = barriers.Count <= currentBreakableIndex;

            if (barrierStatus)
            {
                onBarrierDown?.Invoke();
            }

            yield return new WaitForSeconds(4f);


            _playingAction = false;
            _isHooked = false;
            _movePlayer = false;
            _time = 0;

            if (barrierStatus)
            {
                DestroyBarriers();
            }

        }


        private void ExitAction()
        {
            if (!_isHooked) return;
            if (_playingAction) return;

            var player = Player_Scripts.PlayerController.Instance.Player;
            player.AnimationController.CrossFade("Default", exitCrossfadeTime, 1);

            player.Rig.ResetLeftHandIK();
            player.Rig.ResetRightHandIK();

            player.CanMove = true;
            player.canRotate = true;

            _isHooked = false;
            _movePlayer = false;
        }

        private void DestroyBarriers()
        {
            foreach (var barrier in barriers)
            {
                foreach (var rb in barrier.rigidBody)
                {
                    Destroy(rb);
                }
            }

            Destroy(this);
        }

        private void OnDestroy()
        {
            Destroy(GetComponent<BoxCollider>());
        }

        #endregion

    }

    [System.Serializable]
    public class Barrier
    {
        [SerializeField] public List<Rigidbody> rigidBody;

        [SerializeField] public Transform forceTransform;
        [SerializeField] public float breakForce;

        [SerializeField, Space(20)] public Transform leftHandIKTarget;
        [SerializeField] public Transform rightHandIKTarget;


        public void BreakBarrier()
        {
            foreach (var rb in rigidBody)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.AddForceAtPosition(breakForce * forceTransform.forward, forceTransform.position);
            }
        }
    }
}