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

        private static PlayerMovementController _instance;
        public static PlayerMovementController Instance
        {
            get => _instance;
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
                PlayerMovementController._instance = this;
            }

            player.AnimationController.SetInteger(StateIndex, player.currentStateIndex);
            //MARKER : states
            switch (player.eCurrentState)
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

        public void DisablePlayerMovement(bool disable, bool invokeReset = true)
        {
            if (invokeReset)
            {
                ResetAnimator();
            }
            player.DisabledPlayerMovement = disable;

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
            player.interactable = item;
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

                player.verticalAcceleration = 0;
            }
            else
            {
                player.AnimationController.SetFloat("VerticalAcceleration", player.playerVelocity.y);
                player.verticalAcceleration = player.playerVelocity.y;
            }
        }

        public void GroundCheck()
        {

            Ray ray = new Ray(transform.position + Vector3.up * player.sphereCastOffset, Vector3.down);

            if (Physics.SphereCast(ray, player.sphereCastRadius, out RaycastHit hit, 2f, player.groundMask))
            {
                player.IsGrounded = hit.distance < player.groundOffset + player.sphereCastOffset;
            }
            else
            {
                player.IsGrounded = false;
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f, Color.red);

            }

            //TODO may need change????
            player.AnimationController.SetBool(IsGrounded, player.IsGrounded);
        }


        #region State Methods

        public bool VerifyState(PlayerMovementState state)
        {
            return state == player.eCurrentState;
        }


        //MARKER: State

        //MARKER: State
        //TODO : switch case to change state directly
        public void ChangeState(int index)
        {
            if(player.OverrideFlags)  return;

            //if (player.DisablePlayerMovement) return;

            if (index == player.currentStateIndex) return;

            player.previousStateIndex = player.currentStateIndex;
            player.currentStateIndex = index;


            player.AnimationController.SetInteger(StateIndex, index);

            player.currentState.ExitState(player);
            switch (index)
            {
                case <= 0:
                    player.eCurrentState = PlayerMovementState.BasicMovement;
                    player.currentState = player.basicMovementState;
                    break;

                case 1:
                    player.eCurrentState = PlayerMovementState.Ladder;
                    player.currentState = player.ladderMovementState;
                    break;
                case 2:
                    player.eCurrentState = PlayerMovementState.Water;
                    player.currentState = player.waterMovement;
                    break;
                case 3:
                    player.eCurrentState = PlayerMovementState.Rope;
                    player.currentState = player.ropeMovement;
                    break;
                case 11:
                    player.eCurrentState = PlayerMovementState.FreeBasicMovement;
                    player.currentState = player.freeBasicMovement;
                    break;
            }
            player.currentState.EnterState(player);
        }

        public void ChangeState(int index, string animationName)
        {
            if(player.OverrideFlags)  return;
            
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
            if (player.eCurrentState == PlayerMovementState.Ladder)
            {
                StartCoroutine(player.ladderMovementState.LeaveLadder(player));
            }
        }

        public void PlayJump(int forward)
        {
             if (forward == 1)
             {
                 Vector3 velocityChange = player.transform.forward * player.JumpForwardVelocity;

                 player.playerVelocity += velocityChange;

             }

             player.playerVelocity.y = player.JumpVelocity;

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
            print("Resetting animator");
            player.AnimationController.SetFloat(Speed, 0);

            player.AnimationController.SetBool(IsGrounded, true);
            player.AnimationController.ResetTrigger($"Jump");
        }

        #endregion


        public void Reset()
        {
            player.AnimationController.Play("Default", 1);
            player.CController.enabled = true;
            player.DisabledPlayerMovement = false;
            player.OverrideFlags = false;
        }


    }
}
