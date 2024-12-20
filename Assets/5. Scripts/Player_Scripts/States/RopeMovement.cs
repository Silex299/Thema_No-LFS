﻿using System.Collections;
using Misc.Items;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        #region Serialized Fields

        public Rope attachedRope;
        public Transform handSocket;
        public Vector3 offset;
        public bool invertedAxis;

        #endregion

        #region Private Fields

        private bool _isAttached = false;
        private bool _isSwinging;
        private float _attachTime = 0;


        private Coroutine _detachCoroutine;

        #endregion

        #region Animator Hashes

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Action = Animator.StringToHash("Action");
        private static readonly int VerticalAcceleration = Animator.StringToHash("VerticalAcceleration");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

        #endregion

        #region Overriden Methods

        /// <summary>
        /// This method is called when the player exits the rope movement state.
        /// It resets the attached rope, the attachment status, and the player's speed.
        /// It also resets the player's rotation.
        /// </summary>
        public override void ExitState(Player player)
        {
            if (_isAttached)
            {
                player.AnimationController.SetTrigger(Jump);
                player.StartCoroutine(DetachPlayer(player));
            }
            
            attachedRope = null;
        }

        /// <summary>
        /// This method is called when the player enters the rope movement state.
        /// It starts the "Rope" animation and sets the player's grounded status to false.
        /// </summary>
        public override void EnterState(Player player)
        {
            player.MovementController.ResetAnimator();
            player.AnimationController.CrossFade("Rope", 0.1f, 0);
            player.IsGrounded = false;
            player.AnimationController.SetBool(IsGrounded, false);
            _attachTime = Time.time;

            if (_detachCoroutine != null)
            {
                player.StopCoroutine(_detachCoroutine);
            }
        }

        /// <summary>
        /// This method is called every frame during the rope movement state.
        /// It checks for the jump input to start the detachment process.
        /// If the player is attached and not swinging, it updates the player's speed based on the vertical input.
        /// </summary>
        public override void UpdateState(Player player)
        {
            if (!_isAttached) return;

            if (player.CanJump && Input.GetButtonDown("Jump"))
            {
                if (Time.time - _attachTime > 0.1f)
                {
                    player.AnimationController.SetTrigger(Jump);
                    _detachCoroutine = player.StartCoroutine(DetachPlayer(player));
                }
            }

            if (_isSwinging) return;

            player.AnimationController.SetFloat(Speed, Input.GetAxis("Vertical"));
        }

        /// <summary>
        /// This method is called every fixed frame-rate frame during the rope movement state.
        /// If the player is attached, it updates the player's speed based on the horizontal input and controls the swinging action.
        /// </summary>
        public override void FixedUpdateState(Player player)
        {
            if (!_isAttached) return;

            var input = Input.GetAxis("Horizontal");

            float adjustedInput = (attachedRope.initialRotation.y is > 90 or < -90) ? -input : input;
            player.AnimationController.SetFloat(Speed, invertedAxis ? -adjustedInput : adjustedInput);

            if (Mathf.Abs(input) > 0.5f)
            {
                attachedRope.SwingRope((invertedAxis ? -1 : 1) * Input.GetAxis("Horizontal"));

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

        /// <summary>
        /// This method is called after all Update functions have been processed.
        /// If the player is attached, it moves the player along the rope based on the vertical input (if not swinging).
        /// </summary>
        public override void LateUpdateState(Player player)
        {
            if (!_isAttached) return;

            var input = _isSwinging ? 0 : Input.GetAxis("Vertical");

            attachedRope.MovePlayer(input);
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Attempts to attach the player to a rope.
        /// </summary>
        /// <param name="rope">The rope to attach to.</param>
        /// <returns>True if the player was successfully attached to the rope, false otherwise.</returns>
        public bool AttachRope(Rope rope)
        {
            // If the player is already attached to a rope or the rope is the same as the currently attached one, return false
            if (_isAttached || rope == attachedRope) return false;

            // If the player is not in the Rope state, change the player's state to Rope
            if (!PlayerMovementController.Instance.VerifyState(PlayerMovementState.Rope))
            {
                PlayerMovementController.Instance.ChangeState(3);
            }

            // If there is a rope currently attached, detach it
            attachedRope?.Detached();
            // Attach the new rope
            attachedRope = rope;

            EnterState(PlayerMovementController.Instance.player);
            // Set the attachment status to true   
            _isAttached = true;

            return true;
        }

        /// <summary>
        /// Detaches the player from the rope.
        /// </summary>
        /// <param name="player">The player to detach.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        public IEnumerator DetachPlayer(Player player)
        {
            
            Debug.Log("Detaching");
            
            // Set the attachment status to false
            _isAttached = false;
            ResetPlayerRotation(player);
            // Detach the rope
            attachedRope.Detached();
            var horizontalInput = Input.GetAxis("Horizontal");
            player.playerVelocity = new Vector3(attachedRope.exitForce.x, attachedRope.exitForce.y,  attachedRope.exitForce.z * (invertedAxis? horizontalInput : -horizontalInput + attachedRope.CurrentRopeSegment().velocity.z));
            
            
            // While the player is not grounded, apply the calculated velocity
            while (!player.IsGrounded)
            {
                player.MovementController.ApplyGravity();
                player.CController.Move(player.playerVelocity * Time.deltaTime);
                player.MovementController.GroundCheck();
                player.AnimationController.SetFloat(Speed, horizontalInput);
                yield return null;
            }

            // If the player is in the Rope state, change the player's state to Ground
            if (player.MovementController.VerifyState(PlayerMovementState.Rope))
            {
                attachedRope = null;
                player.MovementController.ChangeState(0);
            }
        }
        
        public IEnumerator BrokRope(Rope rope, bool exitFall)
        {
            if (rope != attachedRope)
            {
                yield break;
            }
            
            var player = PlayerMovementController.Instance.player;

            player.AnimationController.CrossFade("Falling Back", 0.2f, 0);
            
            player.oneWayRotation = true;
            player.CanJump = true;
            player.enabledDirectionInput = true;
            
            yield return DetachPlayer(player);

            if (!exitFall)
            {
                yield break;
            }
            
            player.CanRotate = false;
            
            yield return new WaitForSeconds(1f);
            
            player.AnimationController.CrossFade("Standing up", 0.2f, 1);
            player.AnimationController.Play("Land To Idle", 0);
            
            yield return new WaitForSeconds(2f);
            
            player.CanRotate = true;
            player.oneWayRotation = false;
            player.enabledDirectionInput = false;
        }
        
        
        /// <summary>
        /// Resets the player's rotation.
        /// </summary>
        /// <param name="player">The player whose rotation to reset.</param>
        private static void ResetPlayerRotation(Player player)
        {
            // Get the player's current rotation
            var playerRotation = player.transform.eulerAngles;

            // Create a new rotation with the same y-axis rotation as the player's current rotation
            Vector3 eulerAngle = Quaternion.identity.eulerAngles;
            eulerAngle.y = playerRotation.y;

            // Apply the new rotation to the player
            player.transform.eulerAngles = eulerAngle;
        }

        #endregion
    }
}