using System.Collections;
using Misc.Items;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        #region Serialized Fields

        [SerializeField] private Rope attachedRope;
        public Transform handSocket;
        public Vector3 offset;

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

        #endregion

        #region Overriden Methods

        /// <summary>
        /// This method is called when the player exits the rope movement state.
        /// It resets the attached rope, the attachment status, and the player's speed.
        /// It also resets the player's rotation.
        /// </summary>
        public override void ExitState(Player player)
        {
            attachedRope.Detached();
            attachedRope = null;
            _isAttached = false;
            player.MovementController.ResetAnimator();
            ResetPlayerRotation(player);
        }

        /// <summary>
        /// This method is called when the player enters the rope movement state.
        /// It starts the "Rope" animation and sets the player's grounded status to false.
        /// </summary>
        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Rope", 0.1f, 0);
            player.IsGrounded = false;
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

            if (Input.GetButtonDown("Jump"))
            {
                if (Time.time - _attachTime > 1)
                {
                    Debug.LogError("You pressed Jump");
                    _detachCoroutine = player.StartCoroutine(DetachPlayer(player));
                }
                else
                {
                    Debug.Log("You can't dettach");
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
            player.AnimationController.SetFloat(Speed,
                attachedRope.InitialRotation.y is > 90 or < -90 ? input : -input);

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
        private IEnumerator DetachPlayer(Player player)
        {
            // Set the attachment status to false
            _isAttached = false;

            // Trigger the Jump animation
            player.AnimationController.SetTrigger(Jump);
            // Reset the player's rotation
            ResetPlayerRotation(player);
            // Detach the rope
            attachedRope.Detached();

            player.playerVelocity  = new Vector3(0, attachedRope.exitForce, attachedRope.exitForce * -Input.GetAxis("Horizontal") + attachedRope.CurrentRopeSegment().velocity.z );
            

            // While the player is not grounded, apply the calculated velocity
            while (!player.IsGrounded)
            {
                player.MovementController.ApplyGravity();

                player.CController.Move(player.playerVelocity * Time.deltaTime);

                player.MovementController.GroundCheck();

                yield return null;
            }

            // If the player is in the Rope state, change the player's state to Ground
            if (player.MovementController.VerifyState(PlayerMovementState.Rope))
            {
                player.MovementController.ChangeState(0);
            }
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