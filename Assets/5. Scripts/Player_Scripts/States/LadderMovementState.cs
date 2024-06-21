using System.Collections;
using Misc.Items;
using UnityEngine;

namespace Player_Scripts.States
{

    [System.Serializable]
    public class LadderMovementState : PlayerBaseStates
    {

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");
        
        private bool _isJumped;
        public Ladder connectedLadder;
        
        #region Unused Methods

        public override void FixedUpdateState(Player player)
        {
        }

        public override void LateUpdateState(Player player)
        {
        }

        #endregion

        #region Overriden Methods

        public override void EnterState(Player player)
        {
            player.CController.enabled = false;
            player.MovementController.ResetAnimator();
            player.IsGrounded = false;
            _isJumped = false;
        }
        public override void UpdateState(Player player)
        {
            Debug.Log("updating ladder movement");
            if (_isJumped) return;

            var input = Input.GetAxis("Vertical");
            
            connectedLadder.MoveLadder(input);
            player.AnimationController.SetFloat(Speed, input);
            
            if (!Input.GetButtonDown("Jump")) return;
            if (connectedLadder.canJumpOfTheLadder)
            {
                player.StartCoroutine(LeaveLadder(player));
            }


        }
        public override void ExitState(Player player)
        {
            player.CController.enabled = true;
            if (connectedLadder)
            {
                connectedLadder.engaged = false;
                connectedLadder = null;
            }
        }

        #endregion

        #region Custom Methods

        public IEnumerator LeaveLadder(Player player)
        {
            _isJumped = true;
            player.AnimationController.SetTrigger(Jump);

            yield return Fall(player);
        }

        private IEnumerator Fall(Player player)
        {
            
            player.playerVelocity = Vector3.zero;
            
            while (!player.IsGrounded)
            {
                player.MovementController.ApplyGravity();
                player.MovementController.GroundCheck();
                player.CController.Move(player.playerVelocity * Time.deltaTime);
                
                yield return null;
            }

            PlayerMovementController.Instance.RollBack();
        }
        

        #endregion

    }



}
