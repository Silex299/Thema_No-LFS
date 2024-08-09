using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Controllers
{
    public class BasicController : Controller
    {
        [FoldoutGroup("Movement")] public float forwardJumpSpeed = 10;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10;
        [FoldoutGroup("Movement")] public float jumpHeight = 10;
        private PlayerPathManager _pathManager;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private Coroutine _jumpCoroutine;
        private static readonly int Jump1 = Animator.StringToHash("Jump");

        private void Start()
        {
            _pathManager = PlayerPathManager.Instance;
        }


        public override void ControllerEnter(PlayerV1 player)
        {
            player.animator.CrossFade(player.IsGrounded ? "Basic Movement" : "Jump Loop", 0.2f, 0);
        }
        
        public override void ControllerUpdate(PlayerV1 player)
        {
            MovePlayer(player);
        }
        public override void ControllerFixedUpdate(PlayerV1 player)
        {
            //Apply gravity
            player.ApplyGravity();
        }

        #region Player Movement

        private float _horizontalInput;
        private void MovePlayer(PlayerV1 player)
        {
            //Get desired movement inputs
            if (player.IsGrounded)
            {
                _horizontalInput = player.DisableInput ? 0 : Input.GetAxis("Horizontal");
            }
            Vector3 rotation = _pathManager.GetDestination(player.transform.position, _horizontalInput >= 0);

            //Set movement speed and rotation
            if (player.CanBoost)
            {
                if (Input.GetButton("Sprint"))
                {
                    player.animator.SetFloat(Speed, 2 * _horizontalInput, 0.1f, Time.deltaTime);
                }
                else
                {
                    player.animator.SetFloat(Speed, _horizontalInput, 0.1f, Time.deltaTime);
                }
            }
            else
            {
                player.animator.SetFloat(Speed, _horizontalInput);
            }
            
            //Rotate player
            Rotate(player.transform, rotation);
            
            //Jump
            if (Input.GetButtonDown("Jump") && _jumpCoroutine == null)
            {
                print("Dumb fuck");
                if (!player.AltMovement && !player.DisableInput && player.CanJump && player.IsGrounded)
                {
                    if (player.IsInteracting) return;
                    _jumpCoroutine = StartCoroutine(Jump(player, _horizontalInput));
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

        }
        private void Rotate(Transform target, Vector3 lookAt)
        {
            Vector3 direction = (lookAt - target.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            target.rotation = Quaternion.Slerp(target.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        private IEnumerator Jump(PlayerV1 player, float horizontalInput)
        {
            player.animator.SetTrigger(Jump1);

            yield return new WaitForSeconds(Mathf.Abs(horizontalInput) < 0.4f ? 0.3f : 0.1f);

            player.animator.ResetTrigger(Jump1);

            if (!player.CanJump || player.DisableAllMovement)
            {
                _jumpCoroutine = null;
                yield break;
            }

            Vector3 jumpMovement = player.transform.forward * (player.Boost ? 2 : 1 * Mathf.Abs(horizontalInput) * forwardJumpSpeed);
            jumpMovement.y = Mathf.Sqrt(jumpHeight * -2f * -9.8f);
            
            player.AddForce(jumpMovement);
            _jumpCoroutine = null;
        }

        #endregion
    }
}