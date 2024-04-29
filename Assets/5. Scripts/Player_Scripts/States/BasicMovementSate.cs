using Path_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.States
{

    [System.Serializable]
    public class BasicMovementSate : PlayerBaseStates
    {

        [SerializeField, BoxGroup("Ground Check Variables")]
        private float sphereCastOffset;
        [SerializeField, BoxGroup("Ground Check Variables")]
        private float sphereCastRadius;
        [SerializeField, BoxGroup("Ground Check Variables")]
        private float GroundOffset;

        [SerializeField, BoxGroup("Misc")] private int defaultStateIndex = 0;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalAcceleration = Animator.StringToHash("VerticalAcceleration");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Push = Animator.StringToHash("Push");


        private Vector3 _m_playerVelocity;



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


            //Rotate player to next or previous destination
            if (input < -0.3f)
            {
                if (player.oneWayRotation)
                {
                    OneWayRotate(player, PlayerPathController.Instance.GetPreviousPosition());
                }
                else
                {
                    Rotate(player, PlayerPathController.Instance.GetPreviousPosition());
                }
            }
            else if (input > 0.3f)
            {
                Rotate(player, PlayerPathController.Instance.GetNextPosition());
            }

            #endregion

            #region PLAYER MOVEMENT
            //Sprint
            float multiplier = player.canBoost ? (Input.GetButton("Sprint") ? 2 : 1) : 1;

            //Apply gravity and ground check
            GroundCheck(player);
            Gravity(player);

            //Move player base on player velocity (Only responsible for gravity and jump)
            player.CController.Move(_m_playerVelocity * Time.deltaTime);

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
            if (Input.GetButtonDown("Jump"))
            {
                if (player.currentStateIndex == -5)
                {
                    //WHILE SLIDING
                    player.AnimationController.SetTrigger(Jump);
                }
                else if (player.CanJump && player.IsGrounded)
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
            var transform = player.transform;
            var pos = transform.position;
            rotateTowards.y = pos.y;

            var newRotation = Quaternion.LookRotation((rotateTowards - pos), transform.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness);
        }

        private void OneWayRotate(Player player, Vector3 rotateTowards)
        {
            var transform = player.transform;
            var pos = transform.position;
            rotateTowards.y = pos.y;

            var newRotation = Quaternion.LookRotation((pos - rotateTowards), transform.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness);

        }




        private void Gravity(Player player)
        {
            _m_playerVelocity.y -= 10 * Time.deltaTime;

            if (player.IsGrounded)
            {
                if (_m_playerVelocity.y <= 0)
                {
                    _m_playerVelocity = new Vector3(0, -10, 0);
                }
            }
            else
            {
                player.AnimationController.SetFloat(VerticalAcceleration, _m_playerVelocity.y);
            }
        }

        private void GroundCheck(Player player)
        {
            var transform = player.transform;

            Ray ray = new Ray(transform.position + Vector3.up * sphereCastOffset, Vector3.down);

            if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, 2f, player.groundMask))
            {
                player.IsGrounded = hit.distance < GroundOffset + sphereCastOffset;
            }
            else
            {
                player.IsGrounded = false;
            }
            //TODO may need change????
            player.AnimationController.SetBool(IsGrounded, player.IsGrounded);
        }


        #endregion

        #region Animation Callbacks

        public void PlayJump(Player player, int JumpForward)
        {
            if (JumpForward == 1)
            {
                Vector3 velocityChange = player.transform.forward * player.JumpForwardVelocity;

                _m_playerVelocity += velocityChange;

            }


            _m_playerVelocity.y = player.JumpVelocity;
        }


        #endregion


    }


}
