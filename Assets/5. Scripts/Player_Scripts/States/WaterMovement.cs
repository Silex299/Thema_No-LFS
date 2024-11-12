using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;


namespace Player_Scripts.States
{
    [System.Serializable]
    public class WaterMovement : PlayerBaseStates
    {
        [SerializeField] private float swimSpeed = 10;
        [field: SerializeField] public float DamageSpeed { get; set; } = 10f;

        [BoxGroup("RigidBody Flat Settings")] public float flatHeight = 0.34f;
        [BoxGroup("RigidBody Flat Settings")] public Vector3 flatCenter = new Vector3(0, 0.14f, 0);
        [BoxGroup("RigidBody normal Settings")] public float normalHeight = 1.7f;
        [BoxGroup("RigidBody normal Settings")] public Vector3 normalCenter = new Vector3(0, 0.92f, 0);
        
        public Action<bool> onSurfaceAction;
        
        
        public bool OnSurface
        {
            get => _onSurface;
            set
            {
                onSurfaceAction?.Invoke(value);
                _onSurface = value;
            }
        } 
        public bool CanDamage { get; set; } = true;
        
    
        private bool _onSurface;
        private bool _flatPosition;
        private float _speedMultiplier = 1;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int OnSurface1 = Animator.StringToHash("onSurface");
        private static readonly int Push = Animator.StringToHash("Push");
     
        
        #region Overriden Methods

        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Fall in Water", 0.1f);
            player.GroundTag = "Water";
        }

        public override void ExitState(Player player)
        {
            UpdatePlayerRigidBody(player, false);
            _speedMultiplier = 1;
        }

        public override void UpdateState(Player player)
        {
            #region Input Check

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            #endregion

            #region Player Movement

            Rotate(player, horizontalInput);
            
            player.CController.Move(new Vector3(0, verticalInput * swimSpeed * Time.deltaTime,
                -horizontalInput * swimSpeed * _speedMultiplier * Time.deltaTime));

            #endregion

            #region Animation Upadate

            player.AnimationController.SetFloat(Speed, player.enabledDirectionInput ? horizontalInput : Mathf.Abs(horizontalInput));
            player.AnimationController.SetFloat(Direction, verticalInput, 0.3f, Time.deltaTime);
            player.AnimationController.SetBool(OnSurface1, OnSurface);
            

            #endregion

            #region interaction & health


            if (OnSurface)
            {
                #region Interaction

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
                if(CanDamage) player.Health.ResetHealth();
            }
            else
            {
                _speedMultiplier = 1;
                if(CanDamage) player.Health.TakeDamage(Time.deltaTime * DamageSpeed);
            }

            #endregion
            
            UpdatePlayerRigidBody(player, !_onSurface);
        }

        public override void LateUpdateState(Player player)
        {
           
        }

        #endregion

        #region Usused Methods

        public override void FixedUpdateState(Player player)
        {
        }

        #endregion

        #region Custom methods
        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation,
                Time.deltaTime * player.RotationSmoothness / 4);
        }


        private void UpdatePlayerRigidBody(Player player, bool isFlat)
        {
            if(isFlat == _flatPosition) return;
            
            _flatPosition = isFlat;
            player.CController.height = isFlat ? flatHeight : normalHeight;
            player.CController.center = isFlat ? flatCenter : normalCenter;
            
        }
        
        #endregion
    }
}