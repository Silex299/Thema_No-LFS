using System;
using Player_Scripts.Interactables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

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
            //MARKER : states
            switch (player.e_currentState)
            {
                case PlayerMovementState.BasicMovement:
                    player.currentState = player.basicMovementState;
                    break;

                case PlayerMovementState.Ladder:
                    player.currentState = player.ladderMovementState;
                    break;

                case PlayerMovementState.Water:
                    player.currentState = player.waterMovement;
                    break;
                case PlayerMovementState.Rope:
                    player.currentState = player.ropeMovement;
                    break;
                case PlayerMovementState.FreeBasicMovement:
                    player.currentState = player.freeBasicMovement;
                    break;
                default:
                    player.currentState = player.basicMovementState;
                    break;

            }

            player.currentState.EnterState(player);
        }

        private void Update()
        {
            if (player.DisabledPlayerMovement) return;

            player.currentState.UpdateState(player);
        }

        private void LateUpdate()
        {
            if (player.DisabledPlayerMovement) return;

            player.currentState.LateUpdateState(player);
        }

        private void FixedUpdate()
        {
            if (!player.DisabledPlayerMovement)
            {
                player.currentState.FixedUpdateState(player);
            }
        }

        public void DisablePlayerMovement(bool disable)
        {
            player.DisabledPlayerMovement = disable;

            if (disable)
            {
                ResetAnimator();
            }

        }

        public void DisablePlayerMovementInt(int disable)
        {
            player.DisabledPlayerMovement = (disable == 1);

            if (disable == 1)
            {
                ResetAnimator();
            }
        }

        internal void SetIntractable(Interactable item)
        {
            player.Interactable = item;
        }

        /// <summary>
        /// only updates player velocity vector, Doesn't move the player
        /// </summary>
        public void ApplyGravity()
        {
            player.playerVelocity.y -= 10 * Time.deltaTime;

            if (player.IsGrounded)
            {
                if (player.playerVelocity.y <= 0)
                {
                    player.playerVelocity = new Vector3(0, -10, 0);
                }
            }
            else
            {
                player.AnimationController.SetFloat("VerticalAcceleration", player.playerVelocity.y);
                player.verticalAcceleration = player.playerVelocity.y;
            }
        }

        public void GroundCheck(){
            
        }


        #region State Methods

        public bool VerifyState(PlayerMovementState state)
        {
            return state == player.e_currentState;
        }


        //MARKER: State
        public void ChangeState(PlayerMovementState newState, int stateIndex)
        {
            print(stateIndex);

            if (stateIndex == player.currentStateIndex) return;

            player.previousStateIndex = player.currentStateIndex;
            player.currentStateIndex = stateIndex;

            player.AnimationController.SetInteger(StateIndex, stateIndex);

            if (newState != player.e_currentState)
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
                case PlayerMovementState.Water:
                    player.currentState = player.waterMovement;
                    break;
                case PlayerMovementState.Rope:
                    player.currentState = player.ropeMovement;
                    break;
                case PlayerMovementState.FreeBasicMovement:
                    player.currentState = player.freeBasicMovement;
                    break;
            }


            player.currentState.EnterState(player);
        }

        //MARKER: State
        //TODO : switch case to change state directly
        public void ChangeState(int index)
        {
            print("Changing State : " + index);

            //if (player.DisablePlayerMovement) return;

            if (index == player.currentStateIndex) return;

            player.previousStateIndex = player.currentStateIndex;
            player.currentStateIndex = index;


            player.AnimationController.SetInteger(StateIndex, index);

            player.currentState.ExitState(player);

            ResetAnimator();

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
                case 2:
                    player.e_currentState = PlayerMovementState.Water;
                    player.currentState = player.waterMovement;
                    break;
                case 3:
                    player.e_currentState = PlayerMovementState.Rope;
                    player.currentState = player.ropeMovement;
                    break;
                case 11:
                    player.e_currentState = PlayerMovementState.FreeBasicMovement;
                    player.currentState = player.freeBasicMovement;
                    break;
            }
            player.currentState.EnterState(player);
        }

        public void ChangeState(int index, string animationName)
        {
            ChangeState(index);
            PlayAnimation(animationName, 0.3f, 0);
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
                if (!player.DisabledPlayerMovement)
                {
                    player.basicMovementState.PlayJump(player, forward);
                }
            }
        }


        //WATER MOVEMENT

        public void WaterSurfaceHit(bool atSurface)
        {
            if (player.e_currentState == PlayerMovementState.Water)
            {
                player.waterMovement.PlayerAtSurfact(player, atSurface);
            }
        }

        public void WaterBottomHit(bool atBottom)
        {
            if (player.e_currentState == PlayerMovementState.Water)
            {
                player.waterMovement.PlayerAtBottom(player, atBottom);
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

        public void PlayAnimation(string animationName)
        {
            player.AnimationController.CrossFade(animationName, 0.25f, 1);
        }
        public void SetAnimationTrigger(string triggerName)
        {
            player.AnimationController.SetTrigger(triggerName);
        }

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        public void ResetAnimator()
        {
            player.AnimationController.SetFloat(Speed, 0);
            player.AnimationController.SetBool(IsGrounded, true);
            player.AnimationController.ResetTrigger($"Jump");
        }

        #endregion


        public void Reset()
        {
            player.AnimationController.Play("Default", 1);
            player.DisabledPlayerMovement = false;
        }


    }
}
