using System.Collections;
using Path_Scripts;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.States
{
    [System.Serializable]
    public class BasicMovementSate : PlayerBaseStates
    {
        [SerializeField, BoxGroup("Misc")] private int defaultStateIndex = 0;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Push = Animator.StringToHash("Push");

        private Coroutine _jumpCoroutine;

        #region Unused Methods

        public override void ExitState(Player player)
        {
        }

        public override void FixedUpdateState(Player player)
        {
        }

        #endregion

        #region Overriden Methods

        public override void EnterState(Player player)
        {
            defaultStateIndex = player.currentStateIndex;
            player.AnimationController.SetInteger(StateIndex, defaultStateIndex);
        }

        public override void UpdateState(Player player)
        {
            //Input
            var input = player.UseHorizontal ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");
            var otherInput = player.UseHorizontal ? Input.GetAxis("Vertical") : Input.GetAxis("Horizontal");

            #region PLAYER DIRECTIONS

            if (player.oneWayRotation)
            {
                //Rotate player to next or previous destination
                SideRotation(player, PlayerPathController.Instance.GetNextPosition(input, otherInput), input >= 0);
            }
            else
            {
                if (PlayerPathController.Instance.overridePath?.useBothAxes ?? false)
                {
                    if (Mathf.Abs(input) > 0.2f || Mathf.Abs(otherInput) > 0.2f)
                    {
                        Rotate(player, PlayerPathController.Instance.GetNextPosition(input, otherInput));
                    }
                }
                else if (Mathf.Abs(input) > 0.2f)
                {
                    Rotate(player, PlayerPathController.Instance.GetNextPosition(input, otherInput));
                }
            }

            #endregion

            #region PLAYER MOVEMENT

            float multiplier = 1;
            //Sprint
            if (player.ForceBoost)
            {
                multiplier = 2;
            }
            else
            {
                multiplier = player.CanBoost ? (Mathf.Abs(Input.GetAxis("Sprint")) > 0.2f ? 2 : 1) : 1;
            }

            input = (player.enabledDirectionInput ? input : Mathf.Abs(input)) * multiplier;
            otherInput = (player.enabledDirectionInput? otherInput : Mathf.Abs(otherInput)) * multiplier;

            //Apply gravity and ground check
            player.MovementController.GroundCheck();
            player.MovementController.ApplyGravity();

            //Move player base on player velocity (Only responsible for gravity and jump)
            player.CController.Move(player.playerVelocity * Time.deltaTime);

            #endregion

            #region PLAYER ANIMATION UPDATE

            if (player.CanPlayAlternateMovement)
            {
                if (Mathf.Abs(Input.GetAxis("Crouch")) > 0.5f || Input.GetButtonDown("Crouch"))
                {
                    CrouchPlayer(player, true);
                }
                else if (Mathf.Abs(Input.GetAxis("Crouch")) < 0.5f || Input.GetButtonUp("Crouch"))
                {
                    CrouchPlayer(player, false);
                }
            }

            //Don't keep crouching if force boost
            if (player.ForceBoost)
            {
                player.currentStateIndex = 0;
                CrouchPlayer(player, false);
            }

            //Jump
            if (Input.GetButtonDown("Jump"))
            {
                if (player.CanJump && player.IsGrounded && !player.IsOverridingAnimation)
                {
                    _jumpCoroutine ??= player.StartCoroutine(JumpCoroutine(player));
                }
            }

            //Update Speed in animator
            if (PlayerPathController.Instance.overridePath?.useBothAxes ?? false)
            {
                var combinedInput = Mathf.Sqrt(Mathf.Pow(input, 2) + Mathf.Pow(otherInput, 2));
                player.AnimationController.SetFloat(Speed, combinedInput);
            }
            else
            {
                player.AnimationController.SetFloat(Speed, input, 0.05f, Time.deltaTime);
            }

            #endregion
        }

        public override void LateUpdateState(Player player)
        {
            var input = player.UseHorizontal ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");

            #region PLAYER INTERACTION

            //Interact
            if (player.interactable)
            {
                //I need Directions while interacting like if its pushing left or right
                var interactionType = player.interactable.Interact();


                switch (interactionType)
                {
                    case PlayerInteractionType.PUSH:

                        //input =  Mathf.Lerp(input, input * multiplier, Time.deltaTime * 0.001f);

                        if (!player.isInteracting)
                        {
                            player.isInteracting = true;
                            player.AnimationController.SetBool(Push, true);
                        }

                        player.enabledDirectionInput = true;
                        //UPDATE PUSH
                        break;

                    case PlayerInteractionType.NONE:

                        if (player.isInteracting)
                        {
                            player.isInteracting = false;
                            player.AnimationController.SetBool(Push, false);
                        }

                        player.enabledDirectionInput = false;
                        break;
                }
            }
            else
            {
                if (player.isInteracting)
                {
                    player.isInteracting = false;
                    player.AnimationController.SetBool(Push, false);
                    player.enabledDirectionInput = false;
                }
            }

            #endregion
        }

        #endregion

        #region Custom Methods

        //Reset Jump trigger if jump is not triggered Instantly;
        private IEnumerator JumpCoroutine(Player player)
        {
            player.AnimationController.SetTrigger(Jump);
            yield return new WaitForSeconds(0.5f);
            player.AnimationController.ResetTrigger(Jump);
            _jumpCoroutine = null;
        }

        private void Rotate(Player player, Vector3 rotateTowards)
        {
            if (!player.CanRotate || player.DisabledPlayerMovement) return;

            var transform = player.transform;
            var pos = transform.position;
            rotateTowards.y = pos.y;

            var newRotation = Quaternion.LookRotation((rotateTowards - pos), transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness * Mathf.Rad2Deg);
        }

        private void SideRotation(Player player, Vector3 rotateTowards, bool isRight)
        {
            var transform = player.transform;
            var pos = transform.position;
            rotateTowards.y = pos.y;

            Quaternion newRotation;

            if (isRight)
            {
                newRotation = Quaternion.LookRotation((rotateTowards - pos), transform.up);
                //Rotate newRotation by 90degrees in Y axis
                //newRotation *= Quaternion.Euler(0, 90, 0);
            }
            else
            {
                newRotation = Quaternion.LookRotation((pos - rotateTowards), transform.up);
                //Rotate newRotation by 90degrees in Y axis
                //newRotation *= Quaternion.Euler(0, 90, 0);
            }


            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness * Mathf.Rad2Deg);
        }

        private void CrouchPlayer(Player player, bool crouch)
        {
            if (crouch)
            {
                player.AnimationController.SetInteger(StateIndex, -6);
            }
            else
            {
                player.AnimationController.SetInteger(StateIndex, player.currentStateIndex);
            }
        }

        #endregion
    }
}