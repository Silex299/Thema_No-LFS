using System;
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
        private bool _atSurface;

        private GameObject _surfaceEffect;
        private GameObject _underWaterEffect;


        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int OnSurface = Animator.StringToHash("onSurface");
        private static readonly int Push = Animator.StringToHash("Push");
        public WaterVolume PlayerWaterVolume
        {
            set
            {
                waterVolume = value;
                if (_atSurface)
                {
                    waterVolume.OnSurface();
                }
                else
                {
                    waterVolume.UnderWater();
                }
            }
        }
        
        #region Overriden Methods

        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Fall in Water", 0.1f);
            player.CController.height = 0.34f;

            //Spawn effects
            if (!_surfaceEffect)
            {
                _surfaceEffect =
                    player.EffectsManager.SpawnEffects("SurfaceWater", player.transform.position,
                        player.transform.parent);
            }

            if (!_underWaterEffect)
            {
                _underWaterEffect =
                    player.EffectsManager.SpawnEffects("UnderWater",
                        player.transform.position + player.transform.forward * 0.5f + player.transform.up,
                        player.transform);
            }

            player.EffectsManager.PlayInteractionSound(_atSurface ? "Dive In" : "Dive In Muffled");

            //disable Surface effect
            _surfaceEffect.SetActive(_atSurface);
            _underWaterEffect.SetActive(!_atSurface);
        }

        public override void ExitState(Player player)
        {
            player.CController.height = 1.91f;
            if (_surfaceEffect)
            {
                _surfaceEffect.SetActive(false);
            }

            if (_underWaterEffect)
            {
                _underWaterEffect.SetActive(false);
            }
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

            player.AnimationController.SetFloat(Speed,
                player.enabledDirectionInput ? horizontalInput : Mathf.Abs(horizontalInput));
            player.AnimationController.SetFloat(Direction, verticalInput);

            #endregion

            #region Player depth, interaction & health

            CheckDepth(player, SurfaceChangeAction);

            if (_atSurface)
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
            }
            else
            {
                #region Health

                PlayerMovementController.Instance.player.Health.TakeDamage(waterVolume.damageSpeed * Time.deltaTime);

                #endregion

                _speedMultiplier = 1;
            }

            #endregion

            UpdateSurfaceEffect(player);
        }

        public override void LateUpdateState(Player player)
        {
            #region Player position update

            Vector3 desiredPos = player.transform.position;
            desiredPos.x = waterVolume.playerX;

            if (_atSurface)
            {
                if (Input.GetAxis("Vertical") >= 0)
                {
                    desiredPos.y = waterVolume.surfaceLevel - 1.3f;
                }
            }

            player.transform.position = Vector3.Distance(desiredPos, player.transform.position) > 0.2f
                ? Vector3.Lerp(player.transform.position, desiredPos, Time.deltaTime * swimSpeed)
                : desiredPos;

            #endregion
        }

        #endregion

        #region Usused Methods

        public override void FixedUpdateState(Player player)
        {
        }

        #endregion

        #region Custom methods

        private void CheckDepth(Player player, Action<Player, bool> surfaceChangeAction)
        {
            var transform = player.transform;
            bool result = (transform.position.y + 2) >= waterVolume.surfaceLevel;

            if (result == _atSurface) return;

            _atSurface = result;
            surfaceChangeAction?.Invoke(player, _atSurface);
        }

        //Called only when at surface is changed
        private void SurfaceChangeAction(Player player, bool atSurface)
        {
            player.AnimationController.SetBool(OnSurface, atSurface);

            if (atSurface)
            {
                if (_surfaceEffect) _surfaceEffect.SetActive(true);
                if (_underWaterEffect) _underWaterEffect.SetActive(false);

                waterVolume.OnSurface();

                player.Health.ResetHealth();
                player.EffectsManager.PlayInteractionSound("Dive Out");
            }
            else
            {
                if (_surfaceEffect) _surfaceEffect.SetActive(false);
                if (_underWaterEffect) _underWaterEffect.SetActive(true);

                waterVolume.UnderWater();
                player.EffectsManager.PlayInteractionSound("Dive In Muffled");
            }
        }

        private void UpdateSurfaceEffect(Player player)
        {
            if (!_atSurface) return;

            Vector3 effectPos = player.transform.position;

            effectPos.y = (waterVolume.surfaceLevel + 0.01f);
            effectPos += player.transform.forward * 0.5f;
            _surfaceEffect.transform.position = effectPos;
        }

        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation,
                Time.deltaTime * player.RotationSmoothness / 4);
        }

        #endregion
    }
}