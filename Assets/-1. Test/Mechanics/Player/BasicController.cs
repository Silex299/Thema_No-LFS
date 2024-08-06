using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Player
{
    public class BasicController : Controller
    {
        [FoldoutGroup("Movement")] public float forwardJumpSpeed = 10;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10;
        [FoldoutGroup("Movement")] public float jumpHeight = 10;

        [FoldoutGroup("Ground Check")] public LayerMask groundMask;
        [FoldoutGroup("Ground Check")] public float groundDistance = 0.4f;
        [FoldoutGroup("Ground Check")] public float proximityThreshold = 1f;

        private PlayerPathManager _pathManager;
        private Vector3 _movementDirection;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int Stop = Animator.StringToHash("Stop");
        private static readonly int IsInProximity = Animator.StringToHash("IsInProximity");
        private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");

        private Coroutine _jumpCoroutine;
        private static readonly int Jump1 = Animator.StringToHash("Jump");

        private void Start()
        {
            _pathManager = PlayerPathManager.Instance;
        }

        public override void UpdateController(PlayerV1 player)
        {
            MovePlayer(player);
        }

        #region Player Movement
        
        
        private void MovePlayer(PlayerV1 player)
        {
            var horizontalInput = player.DisableInput ? 0 : Input.GetAxis("Horizontal");
            

            #region Gravity and Jump

            if (player.isGrounded && _movementDirection.y < 0)
            {
                _movementDirection = new Vector3(0, -2f, 0);
            }

            if (Input.GetButtonDown("Jump") && player.isGrounded)
            {
                _jumpCoroutine ??= StartCoroutine(SetJumpForce(player, horizontalInput));
            }

            _movementDirection.y += -9.8f * Time.deltaTime;

            #endregion

            player.characterController.Move((_movementDirection) * Time.deltaTime);
            
            #region Rotation and Ground Check

            Vector3 lookAt = _pathManager.GetDestination(player.transform.position, horizontalInput > 0);
            if (player.isGrounded && !Mathf.Approximately(horizontalInput, 0))
            {
                Rotate(player.transform, lookAt);
            }
            GroundCheck(player);

            #endregion


            if (!player.DisableAnimationUpdate)
            {
                //Update speed and Grounded
                player.animator.SetFloat(Speed, horizontalInput);
                player.animator.SetFloat(VerticalVelocity, player.PlayerVelocity.y);
                player.animator.SetBool(IsGrounded, player.isInGroundProximity);
                player.animator.SetBool(IsInProximity, player.isInGroundProximity);
            }
            
        }
        
        private void GroundCheck(PlayerV1 player)
        {
            player.isGrounded = Physics.CheckSphere(player.transform.position, groundDistance, groundMask);
            player.isInGroundProximity = Physics.CheckSphere(player.transform.position, proximityThreshold, groundMask);

            //REMOVE
            Debug.DrawLine(player.transform.position,
                player.transform.position + Vector3.down * (player.isGrounded ? groundDistance : proximityThreshold),
                player.isGrounded ? Color.green : player.isInGroundProximity ? Color.blue : Color.red, 1f);
        }
        private void Rotate(Transform target, Vector3 lookAt)
        {
            Vector3 direction = (lookAt - target.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            target.rotation = Quaternion.Slerp(target.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        private IEnumerator SetJumpForce(PlayerV1 player, float horizontalInput)
        {

           
            if (!player.DisableAnimationUpdate) player.animator.SetTrigger(Jump1);

            yield return new WaitForSeconds(Mathf.Abs(horizontalInput) < 0.4f ? 0.3f : 0.1f);
            
            if (!player.DisableAnimationUpdate) player.animator.ResetTrigger(Jump1);
            _movementDirection = player.transform.forward * (Mathf.Abs(horizontalInput) * forwardJumpSpeed);
            _movementDirection.y = Mathf.Sqrt(jumpHeight * -2f * -9.8f);
            _jumpCoroutine = null;
        }

        #endregion
    }
}