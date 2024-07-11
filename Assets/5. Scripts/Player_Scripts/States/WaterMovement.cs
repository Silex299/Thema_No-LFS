using System.Collections;
using Player_Scripts.Volumes;
using UnityEngine;


namespace Player_Scripts.States
{
    [System.Serializable]
    public class WaterMovement : PlayerBaseStates
    {
        [SerializeField] private float swimSpeed = 10;
        public WaterVolume waterVolume;


        private float _speedMultiplier = 1;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int OnSurface = Animator.StringToHash("onSurface");
        private static readonly int Push = Animator.StringToHash("Push");

        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Fall in Water", 0.1f);
            player.CController.height = 0.34f;
        }

        public override void ExitState(Player player)
        {
            player.CController.height = 1.91f;
        }

        public override void UpdateState(Player player)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            #region Player Movement

            Rotate(player, horizontalInput);
            player.CController.Move(new Vector3(0, verticalInput * swimSpeed * Time.deltaTime,
                -horizontalInput * swimSpeed * _speedMultiplier * Time.deltaTime));

            #endregion


            #region Animation Upadate

            player.AnimationController.SetFloat(Speed, player.enabledDirectionInput? horizontalInput: Mathf.Abs(horizontalInput));
            player.AnimationController.SetFloat(Direction, verticalInput);
            
            #endregion


        }
        
        public override void LateUpdateState(Player player)
        {
            CheckDepth(player, out bool onSurface, out bool atBottom);

            #region Player position and Camera update
            
            Vector3 desiredPos = player.transform.position;
            desiredPos.x = waterVolume.playerX;
            
            if (onSurface)
            {
                if (Input.GetAxis("Vertical") >= 0)
                {
                    desiredPos.y = waterVolume.surfaceLevel - 1.3f;
                }
            }
            if (Vector3.Distance(desiredPos, player.transform.position) > 0.2f)
            {
                player.transform.position = Vector3.Lerp(player.transform.position, desiredPos, Time.deltaTime * swimSpeed);
            }
            else
            {
                player.transform.position = desiredPos;
            }

            waterVolume.ChangeCameraOffset(onSurface);
            player.AnimationController.SetBool(OnSurface, onSurface);
            
            #endregion


            #region Interaction

            if (!onSurface)
            {
                _speedMultiplier = 1;
                return;
            }
            
            if (player.interactable)
            {
                PlayerInteractionType interaction = player.interactable.Interact();

                switch (interaction)
                {
                    case PlayerInteractionType.NONE:
                        
                        if (player.isInteracting)
                        {
                            _speedMultiplier = 1;
                            player.isInteracting = false;
                            player.AnimationController.SetBool(Push, false);
                            player.enabledDirectionInput = false;
                        }

                        break;

                    case PlayerInteractionType.PUSH:
                        
                        if (!player.isInteracting)
                        {
                            _speedMultiplier = 0.5f;
                            player.enabledDirectionInput = true;
                            player.isInteracting = true;
                            player.AnimationController.SetBool(Push, true);
                        }

                        //UPDATE PUSH
                        break;
                }
            }
            else
            {
                if (player.isInteracting)
                {
                    _speedMultiplier = 1;
                    player.isInteracting = false;
                    player.AnimationController.SetBool(Push, false);
                    player.enabledDirectionInput = false;
                }
            }


            #endregion
            
        }

        public override void FixedUpdateState(Player player)
        {
        }



        private void CheckDepth(Player player, out bool atSurface, out bool atBottom)
        {
            var transform = player.transform;
            atSurface = (transform.position.y + 2) >= waterVolume.surfaceLevel;
            atBottom = transform.position.y <= waterVolume.bottomLevel;
        }

        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation,
                Time.deltaTime * player.RotationSmoothness/4);
        }
    }
}