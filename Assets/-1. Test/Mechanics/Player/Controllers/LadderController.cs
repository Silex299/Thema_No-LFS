using System.Collections;
using Mechanics.Player.Custom;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Controllers
{
    public class LadderController : Controller
    {
        public float transitionTime;
        public float movementSpeed;
        public bool canJumpOff;

        public float startLadder;
        public float endLadder;
        public Vector3 playerPositionOffset;


        public UnityEvent exitEvent;
        

        private float _playerAt;
        private bool _engaged;

        private Coroutine _engageCoroutine;
        private Coroutine _jumpCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");


        #region DEBUG

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var playerPosition = (transform.position + transform.up * startLadder) +
                                 transform.up * _playerAt * (endLadder - startLadder) + playerPositionOffset;
            Gizmos.DrawWireSphere(playerPosition, 0.2f);

            Gizmos.color = Color.yellow;
            var starPos = transform.position + transform.up * startLadder;
            var endPos = transform.position + transform.up * endLadder;
            Gizmos.DrawLine(starPos, endPos);
        }

#endif

        #endregion


        #region Enter Ladder

        public override void ControllerEnter(PlayerV1 player)
        {
            if(_engaged) return;
            base.ControllerEnter(player);
            _engageCoroutine ??= StartCoroutine(EngagePlayer(player));
        }

        private IEnumerator EngagePlayer(PlayerV1 player)
        {
            Vector3 initPlayerPos = player.transform.position;
            Quaternion initPlayerRot = player.transform.rotation;

            GetClosestTransform(player.transform.position, out var targetPos, out var targetRot);

            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                player.transform.position = Vector3.Lerp(initPlayerPos, targetPos, timeElapsed / transitionTime);
                player.transform.rotation = Quaternion.Slerp(initPlayerRot, targetRot, timeElapsed / transitionTime);
                
                yield return new WaitForEndOfFrame();
            }

            _engaged = true;
            _engageCoroutine = null;
        }

        private void GetClosestTransform(Vector3 point, out Vector3 desiredPos, out Quaternion desiredRot)
        {
            var startPos = transform.position + transform.up * startLadder;
            var endPos = transform.position + transform.up * endLadder;
            var closestPoint = ThemaVector.GetClosestPointToLine(startPos, endPos, point);

            _playerAt = (closestPoint - startPos).magnitude / (endPos - startPos).magnitude;

            desiredPos = closestPoint;
            desiredRot = Quaternion.LookRotation(transform.forward, transform.up);
        }

        #endregion


        #region Ladder Action

        public override void ControllerUpdate(PlayerV1 player)
        {
            if (!_engaged) return;

            float input = Input.GetAxis("Vertical");
            MovePlayer(player, input);
        }


        private void MovePlayer(PlayerV1 player, float input)
        {
            _playerAt = Mathf.Clamp01(_playerAt + (input * movementSpeed * Time.deltaTime));

            player.characterController.Move(GetMovementVector(player.transform));
            player.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);

            player.animator.SetFloat(Speed, input);

            if (Input.GetButtonDown("Jump") && canJumpOff)
            {
                _jumpCoroutine ??= StartCoroutine(JumpCoroutine(player));
            }
        }
        private Vector3 GetMovementVector(Transform playerTransform)
        {
            Vector3 startPos = transform.position + transform.up * startLadder;
            Vector3 targetPos = startPos + transform.up * (_playerAt * (endLadder - startLadder)) +
                                playerPositionOffset;
            return targetPos - playerTransform.position;
        }
        private IEnumerator JumpCoroutine(PlayerV1 player)
        {

            #region Player Constrain
            
            player.GroundCheck();
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
            
            #endregion
            
            #region Trigger Jump off
            
            _engaged = false;
            player.animator.SetTrigger(Jump);
            
            #endregion
            
            #region Apply gravity untill grounded
            
            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }
            
            #endregion

            #region Exit Controller

            exitEvent.Invoke();
            _jumpCoroutine = null;

            #endregion
            
        }

        #endregion


        #region Ladder Exit

        public override void ControllerExit(PlayerV1 player)
        {
            _engaged = false;
            player.ResetMovement();
        }

        #endregion
    }
}