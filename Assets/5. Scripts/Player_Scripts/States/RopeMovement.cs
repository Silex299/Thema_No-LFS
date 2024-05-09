using System.Collections;
using Misc.Items;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        public Rope attachedRope;
        public Transform handSocket;
        public Vector3 offset;
        [SerializeField] private float exitForceMultiplier = 3;

        private bool _isAttached = false;
        private bool _isSwinging;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Action = Animator.StringToHash("Action");
        private static readonly int VerticalAcceleration = Animator.StringToHash("VerticalAcceleration");
        private static readonly int Jump = Animator.StringToHash("Jump");

        public override void ExitState(Player player)
        {
            attachedRope = null;
            _isAttached = false;
            player.AnimationController.SetFloat(Speed, 0);

            ResetPlayerRotation(player);
        }


        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Rope", 0.1f, 0);
            player.IsGrounded = false;
        }

        public override void UpdateState(Player player)
        {
            if (Input.GetButtonDown("Jump"))
            {
                player.StartCoroutine(DetachPlayer(player));
            }

            if (!_isAttached || _isSwinging) return;

            player.AnimationController.SetFloat(Speed, Input.GetAxis("Vertical"));
        }


        public override void FixedUpdateState(Player player)
        {
            if (!_isAttached) return;

            var input = Input.GetAxis("Horizontal");
            player.AnimationController.SetFloat(Speed, input);

            if (Mathf.Abs(input) > 0.2f)
            {
                attachedRope.SwingRope(Input.GetAxis("Horizontal"));
                if (!_isSwinging)
                {
                    player.AnimationController.SetBool(Action, true);
                    _isSwinging = true;
                }
            }
            else
            {
                if (_isSwinging)
                {
                    player.AnimationController.SetBool(Action, false);
                    _isSwinging = false;
                }
            }
        }

        public override void LateUpdateState(Player player)
        {
            if (!_isAttached) return;

            var input = _isSwinging ? 0 : Input.GetAxis("Vertical");

            attachedRope.MovePlayer(input);
        }

        public bool AttachRope(Rope rope)
        {
            if (_isAttached || rope == attachedRope) return false;

            if (!PlayerMovementController.Instance.VerifyState(PlayerMovementState.Rope))
            {
                PlayerMovementController.Instance.ChangeState(3);
            }

            attachedRope?.Detached();
            attachedRope = rope;
            _isAttached = true;

            return true;
        }
        
        private IEnumerator DetachPlayer(Player player)
        {
            _isAttached = false;

            player.AnimationController.SetTrigger(Jump);
            ResetPlayerRotation(player);
            attachedRope.Detached();
            
            Vector3 playerVelocity = attachedRope.CurrentRopeSegment().velocity * exitForceMultiplier +
                                     Vector3.up * exitForceMultiplier;

            while (!player.IsGrounded)
            {
                playerVelocity.y -= 10 * Time.deltaTime;

                player.AnimationController.SetFloat(VerticalAcceleration, playerVelocity.y);

                player.CController.Move(playerVelocity * Time.deltaTime);

                player.basicMovementState.GroundCheck(player);

                yield return null;
            }


            if (player.MovementController.VerifyState(PlayerMovementState.Rope))
            {
                player.MovementController.ChangeState(0);
            }
        }
        
        private static void ResetPlayerRotation(Player player)
        {
            var playerRotation = player.transform.eulerAngles;

            Vector3 eulerAngle = Quaternion.identity.eulerAngles;
            eulerAngle.y = playerRotation.y;

            player.transform.eulerAngles = eulerAngle;
        }
    }
}