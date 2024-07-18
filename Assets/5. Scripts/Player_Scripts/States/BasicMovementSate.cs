using Path_Scripts;
using Sirenix.OdinInspector;
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
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalAcceleration = Animator.StringToHash("VerticalAcceleration");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Push = Animator.StringToHash("Push");


        #region Unused Methods

        public override void ExitState(Player player)
        {
        }
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
            defaultStateIndex = player.currentStateIndex;
            player.AnimationController.SetInteger(StateIndex, defaultStateIndex);
        }

        public override void UpdateState(Player player)
        {

            //Input
            var input = player.UseHorizontal ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");

            #region PLAYER DIRECTIONS

            if (player.oneWayRotation)
            {

                if (input >= 0)
                {
                    SideRotation(player, PlayerPathController.Instance.GetNextPosition(), true);
                }
                else
                {
                    SideRotation(player, PlayerPathController.Instance.GetPreviousPosition(), false);
                }

            }
            else
            {
                //Rotate player to next or previous destination
                if (input < -0.3f)
                {

                    Rotate(player, PlayerPathController.Instance.GetPreviousPosition());
                }
                else if (input > 0.3f)
                {

                    Rotate(player, PlayerPathController.Instance.GetNextPosition());
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
                multiplier = player.CanBoost ? (Input.GetButton("Sprint") ? 2 : 1) : 1;
            }

            //Apply gravity and ground check
            player.MovementController.GroundCheck();
            player.MovementController.ApplyGravity();

            //Move player base on player velocity (Only responsible for gravity and jump)
            player.CController.Move(player.playerVelocity * Time.deltaTime);

            #endregion

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

            input = (player.enabledDirectionInput ? input : Mathf.Abs(input)) * multiplier;

            #endregion

            #region PLAYER ANIMATION UPDATE
            
            if(Input.GetButtonDown("Crouch"))
            {
                CrouchPlayer(player, true);
            }
            else if(Input.GetButtonUp("Crouch"))
            {
                CrouchPlayer(player, false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (player.CanJump && player.IsGrounded)
                {
                    player.AnimationController.SetTrigger(Jump);
                    
                }
            }
            //Update Speed in animator
            player.AnimationController.SetFloat(Speed, input);
            #endregion
        }

        #endregion


        #region Custom Methods

        private void Rotate(Player player, Vector3 rotateTowards)
        {
            if(!player.CanRotate) return;
            
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
            if(crouch){
                player.AnimationController.SetInteger(StateIndex, -6);
            }
            else{
                player.AnimationController.SetInteger(StateIndex, player.currentStateIndex);
            }
        }

        #endregion



    }


}
