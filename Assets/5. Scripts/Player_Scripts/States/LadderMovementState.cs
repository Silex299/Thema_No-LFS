using System.Collections;
using UnityEngine;

namespace Player_Scripts.States
{

    [System.Serializable]
    public class LadderMovementState : PlayerBaseStates
    {

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");
        
        private bool _isJumped;
        [HideInInspector] public Ladder connectedLadder;
        
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
            _isJumped = false;
            player.MovementController.ResetAnimator();
        }
        public override void UpdateState(Player player)
        {
            if (_isJumped) return;

            var input = Input.GetAxis("Vertical");

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
            connectedLadder.engaged = false;
            connectedLadder = null;
        }

        #endregion

        #region Custom Methods

        public IEnumerator LeaveLadder(Player player)
        {
            _isJumped = true;
            player.AnimationController.SetTrigger(Jump);
            yield return new WaitForSeconds(0.1f);
            PlayerMovementController.Instance.RollBack();
        }

        #endregion

    }



}
