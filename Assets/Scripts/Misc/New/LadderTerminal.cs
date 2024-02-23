using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.New
{
    public class LadderTerminal : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")]
        private string climbUpLadder;
        [SerializeField, BoxGroup("Animation")]
        private string climbDownLadder;
        [SerializeField, BoxGroup("Animation")]
        private string climbUpExit;

        [SerializeField, BoxGroup("Movement")]
        private bool rotationRestricted;
        [SerializeField, BoxGroup("Movement"), InfoBox("Angle in degrees"), ShowIf("rotationRestricted")]
        private float maximumAngle = 70f;
        [SerializeField, BoxGroup("Movement")]
        private Transform movePlayerTo;
        [SerializeField, BoxGroup("Movement")]
        private Transform movePlayerTo1;
        [SerializeField, BoxGroup("Movement")]
        private float movementSmoothness;
        [SerializeField, BoxGroup("Movement")]
        private float rotationSmoothness;


        private bool _climbingPlayer;
        private bool _climbingDownPlayer;
        private Transform _target;

        private void Start()
        {
            Player_Scripts.PlayerController.Instance.AnimationCall += ChangeState;
        }
        private void OnDisable()
        {
            Player_Scripts.PlayerController.Instance.AnimationCall -= ChangeState;
        }


        private void FixedUpdate()
        {
            if (_climbingPlayer)
            {
                var targetPos = _target.position;
                var destinationPos = movePlayerTo.position;

                var targteRot = _target.rotation;
                var destinationRotation = movePlayerTo.rotation;

                _target.position = Vector3.MoveTowards(targetPos, destinationPos, movementSmoothness * Time.deltaTime);
                _target.rotation = Quaternion.RotateTowards(targteRot, destinationRotation, rotationSmoothness * Time.deltaTime);


                if (Vector3.Distance(targetPos, destinationPos) < 0.1f)
                {
                    if (Quaternion.Angle(targteRot, destinationRotation) < 5f)
                    {
                        _climbingPlayer = false;
                    }
                }

            }

            if (_climbingDownPlayer)
            {
                var targetPos = _target.position;
                var destinationPos = movePlayerTo1.position;

                var targteRot = _target.rotation;
                var destinationRotation = movePlayerTo1.rotation;

                _target.position = Vector3.MoveTowards(targetPos, destinationPos, movementSmoothness * Time.deltaTime);
                _target.rotation = Quaternion.RotateTowards(targteRot, destinationRotation, rotationSmoothness * Time.deltaTime);


                if (Vector3.Distance(targetPos, destinationPos) < 0.01f)
                {
                    if (Quaternion.Angle(targteRot, destinationRotation) < 2f)
                    {
                        _climbingDownPlayer = false;
                    }
                }
            }
        }

        public void JumpToLadder()
        {
            _target = Player_Scripts.PlayerController.Instance.transform;

            if (rotationRestricted)
            {
                var angle = Quaternion.Angle(_target.rotation, movePlayerTo.rotation);
                if (angle > maximumAngle)
                {
                    return;
                }
            }

            StartCoroutine(ClimbUpLadder());
        }

        public void DownToLadder()
        {
            _target = Player_Scripts.PlayerController.Instance.transform;


            StartCoroutine(ClimbDownToLadder());
        }

        private IEnumerator ClimbUpLadder()
        {

            _climbingPlayer = true;
            Player_Scripts.PlayerController.Instance.Player.canRotate = false;
            Player_Scripts.PlayerController.Instance.Player.CanMove = false;


            yield return new WaitUntil(() => { return !_climbingPlayer; });
            Player_Scripts.PlayerController.Instance.Player.canRotate = true;

            Player_Scripts.PlayerController.Instance.Player.AnimationController.CrossFade(climbUpLadder, 0.2f, 1);
        }

        private IEnumerator ClimbDownToLadder()
        {
            _climbingDownPlayer = true;
            Player_Scripts.PlayerController.Instance.Player.canRotate = false;

            yield return new WaitUntil(() => { return !_climbingDownPlayer; });
            Player_Scripts.PlayerController.Instance.Player.canRotate = true;

            Player_Scripts.PlayerController.Instance.CrossFadeAnimation(climbDownLadder);
        }


        public void ClimbUpExit()
        {
            Player_Scripts.PlayerController.Instance.CrossFadeAnimation(climbUpExit);
        }


        private void ChangeState(string message)
        {
            if (message == "ladder")
            {
                Player_Scripts.PlayerController.Instance.ChangeState(Player_Scripts.PlayerController.PlayerStates.LadderMovement, 0);
            }
            else if (message == "basic Restricted")
            {
                Player_Scripts.PlayerController.Instance.ChangeState(Player_Scripts.PlayerController.PlayerStates.BasicRestrictedMovement, 0);
            }
        }


    }
}