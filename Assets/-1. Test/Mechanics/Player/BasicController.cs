using System.Collections;
using Sirenix.OdinInspector;
using Unity.Plastic.Antlr3.Runtime;
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
        private static readonly int Speed = Animator.StringToHash("Speed");

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
            //Get desired movement directions and inputs
            var horizontalInput = player.DisableInput ? 0 : Input.GetAxis("Horizontal");
            Vector3 rotation = _pathManager.GetDestination(player.transform.position, horizontalInput > 0);


            //Set movement speed and rotation
            if (player.CanBoost)
            {
                if (Input.GetButton("Sprint"))
                {
                    player.animator.SetFloat(Speed, 2 * horizontalInput, 0.1f, Time.deltaTime);
                }
                else
                {
                    player.animator.SetFloat(Speed, horizontalInput, 0.1f, Time.deltaTime);
                }
            }
            else
            {
                player.animator.SetFloat(Speed, horizontalInput);
            }
            //Rotate player
            if (player.IsGrounded && !Mathf.Approximately(horizontalInput, 0))
            {
                Rotate(player.transform, rotation);
            }
            
            //Jump
            if (Input.GetButtonDown("Jump") && _jumpCoroutine == null)
            {
                if (!player.AltMovement && !player.DisableInput && player.CanJump && player.IsGrounded)
                {
                    _jumpCoroutine = StartCoroutine(Jump(player, horizontalInput));
                }
            }

            //Crouch
            if (player.CanAltMovement)
            {
                if (Input.GetButtonDown("Crouch"))
                {
                    player.AltMovement = true;
                }
                else if (Input.GetButtonUp("Crouch"))
                {
                    player.AltMovement = false;
                }
            }

            //Apply gravity
            player.ApplyGravity();
        }

        private void Rotate(Transform target, Vector3 lookAt)
        {
            Vector3 direction = (lookAt - target.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            target.rotation = Quaternion.Slerp(target.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        private IEnumerator Jump(PlayerV1 player, float horizontalInput)
        {
            player.animator.SetTrigger(Jump1);

            yield return new WaitForSeconds(Mathf.Abs(horizontalInput) < 0.4f ? 0.3f : 0.1f);

            player.animator.ResetTrigger(Jump1);


            Vector3 jumpMovement = player.transform.forward *
                                   (player.Boost ? 2 : 1 * Mathf.Abs(horizontalInput) * forwardJumpSpeed);
            jumpMovement.y = Mathf.Sqrt(jumpHeight * -2f * -9.8f);
            
            player.AddForce(jumpMovement);
            _jumpCoroutine = null;
        }

        #endregion
    }
}