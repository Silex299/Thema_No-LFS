using Player_Scripts.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts
{

    [RequireComponent(typeof(Player))]
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField, BoxGroup("References")] internal Player player;

        private static PlayerMovementController instance;
        public static PlayerMovementController Instance
        {
            get => instance;
        }


        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        private void Awake()
        {
            if (PlayerMovementController.Instance != null)
            {
                Destroy(PlayerMovementController.Instance);
            }
            else
            {
                PlayerMovementController.instance = this;
            }

            player.AnimationController.SetInteger(StateIndex, player.currentStateIndex);
            switch (player.e_currentState)
            {
                case PlayerMovementState.BasicMovement:
                    player.currentState = player.basicMovementState;
                    break;

                case PlayerMovementState.Ladder:
                    player.currentState = player.ladderMovementState;
                    break;

                default:
                    player.currentState = player.basicMovementState;
                    break;

            }

            player.currentState.EnterState(player);
        }

        private void Update()
        {
            if (player.DisablePlayerMovement) return;

            player.currentState.UpdateState(player);
        }


        public void DiablePlayerMovement(bool disable)
        {
            player.DisablePlayerMovement = disable;

            if (disable)
            {
                ResetAnimator();
            }

        }

        internal void SetInteractable(Interactable item)
        {
            player.interactable = item;
        }

        #region State Methods

        public bool VerifyState(PlayerMovementState state)
        {
            return state == player.e_currentState;
        }

        public void ChangeState(PlayerMovementState newState, int stateIndex)
        {
            if (stateIndex == player.currentStateIndex) return;

            player.previousStateIndex = player.currentStateIndex;
            player.currentStateIndex = stateIndex;

            player.AnimationController.SetInteger(StateIndex, stateIndex);
            
            if(newState != player.e_currentState)
            {
                player.currentState.ExitState(player);
            }


            player.e_currentState = newState;
            switch (newState)
            {
                case PlayerMovementState.BasicMovement:
                    player.currentState = player.basicMovementState;
                    break;
                case PlayerMovementState.Ladder:
                    player.currentState = player.ladderMovementState;
                    break;
            }


            player.currentState.EnterState(player);
        }

        public void ChangeState(int index)
        {
            if (index == player.currentStateIndex) return;

            player.previousStateIndex = player.currentStateIndex;
            player.currentStateIndex = index;


            player.AnimationController.SetInteger(StateIndex, index);

            player.currentState.ExitState(player);

            switch (index)
            {
                case <= 0:
                    player.e_currentState = PlayerMovementState.BasicMovement;
                    player.currentState = player.basicMovementState;
                    break;

                case 1:
                    player.e_currentState = PlayerMovementState.Ladder;
                    player.currentState = player.ladderMovementState;
                    break;
            }

            player.currentState.EnterState(player);
        }

        /// <summary>
        /// Reverts back to previous state
        /// </summary>
        public void RollBack()
        {
            ChangeState(player.previousStateIndex);
        }


        #endregion

        #region Custom State movements

        public void LadderExit()
        {
            if (player.e_currentState == PlayerMovementState.Ladder)
            {
                StartCoroutine(player.ladderMovementState.LeaveLadder(player));
            }
        }

        public void PlayJump(int forward)
        {
            if (player.e_currentState == PlayerMovementState.BasicMovement)
            {
                if (!player.DisablePlayerMovement)
                {
                    player.basicMovementState.PlayJump(player, forward);
                }
            }
        }

        #endregion

        #region Animations
        public void PlayAnimation(string animationName, int animationLayer)
        {
            player.AnimationController.Play(animationName, animationLayer);
        }
        public void PlayAnimation(string animationName, float normalisationDuration, int animationLayer)
        {
            player.AnimationController.CrossFade(animationName, normalisationDuration, animationLayer);
        }

        public void SetAnimationTrigger(string triggerName)
        {
            player.AnimationController.SetTrigger(triggerName);
        }

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private void ResetAnimator()
        {
            player.AnimationController.SetFloat(Speed, 0);
            player.AnimationController.SetBool(IsGrounded, true);
            player.AnimationController.ResetTrigger("Jump");
        }

        #endregion


        public void Reset()
        {
            player.AnimationController.Play("Default", 1);
        }


    }
}
