using System.Collections;
using UnityEngine;


namespace Player_Scripts.States
{
    [System.Serializable]
    public class WaterMovement : PlayerBaseStates
    {

        [SerializeField] private float swimSpeed = 10;
        [SerializeField] private float lostBreathSpeed = 10;
        public float restrictedYPosition = 7.1f;
        public float restrictedXPosition;


        //Defines player position if player is at the surface of the water body or at bottom depth
        internal bool atBottom;
        internal bool atSurface;


        private float previousCharacterHeight;
        private GameObject surfaceEffect;
        private GameObject bubbleEffect;
        private float _speedMultiplier = 1;


        private static readonly int Horizontal = Animator.StringToHash("Speed");
        private static readonly int Vertical = Animator.StringToHash("Direction");
        private static readonly int onSurface = Animator.StringToHash("onSurface");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Push = Animator.StringToHash("Push");

        #region Overriden Methods

        public override void EnterState(Player player)
        {
            Debug.Log("Entering Water");
            player.AnimationController.CrossFade("Fall in Water", 0.1f);
            
            //TODO: REMOVE THIS STATE INDEX??
            player.AnimationController.SetInteger(StateIndex, 2);

            previousCharacterHeight = player.CController.height;
            player.CController.height = 0.7f;
            Physics.gravity = new Vector3(0, -1f, 0);

            PlayerMovementController.Instance.DisablePlayerMovement(true);

            player.StartCoroutine(EnablePlayerMovement());


            #region Spawning Effects

            bubbleEffect = player.EffectsManager.SpawnEffect("UnderWater", new Vector3(0, 1f, 0));
            Transform transform = player.transform;
            Vector3 surfaceEffectPosition = transform.forward * 0.606f + transform.up * 1.3f;
            surfaceEffect = player.EffectsManager.SpawnEffect("SurfaceWater", surfaceEffectPosition);


            if (atSurface)
            {
                bubbleEffect.SetActive(false);
                surfaceEffect.SetActive(true);
            }
            else
            {
                bubbleEffect.SetActive(true);
                surfaceEffect.SetActive(false);
            }

            #endregion
        }

        public override void UpdateState(Player player)
        {
            //Basic movement and animator

            #region Basic Movement

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");


            if (atSurface && verticalInput > 0)
            {
                verticalInput = 0;
            }
            else if (atBottom && verticalInput < 0)
            {
                verticalInput = 0;
            }

            if (!atSurface)
            {
                LoseBreath(player);
            }
            else
            {
                player.EffectsManager.SetInteractionSound(Mathf.Abs(horizontalInput));
            }


            Vector3 movementVector = new Vector3(0, verticalInput, -horizontalInput);

            player.CController.Move((atSurface ? 0.8f : 1) * _speedMultiplier * movementVector * swimSpeed *
                                    Time.deltaTime);


            player.AnimationController.SetFloat(Horizontal,
                ((player.enabledDirectionInput) ? horizontalInput : Mathf.Abs(horizontalInput)), 0.2f, Time.deltaTime);
            player.AnimationController.SetFloat(Vertical, verticalInput, 0.2f, Time.deltaTime);

            Rotate(player, horizontalInput);

            #endregion


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

                        if (!atSurface) break;

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
                //TODO: MAKE IT CALL ONLY ONCE
                _speedMultiplier = 1;

                if (player.isInteracting)
                {
                    player.isInteracting = false;
                    player.AnimationController.SetBool(Push, false);
                    player.enabledDirectionInput = false;
                }
            }

            #endregion
        }


        public override void LateUpdateState(Player player)
        {
        }

        public override void ExitState(Player player)
        {
            player.CController.height = previousCharacterHeight;
            player.EffectsManager.PlayLoopInteraction("", false); // Stop loop interaction

            if (surfaceEffect)
            {
                GameObject.Destroy(surfaceEffect);
            }

            if (bubbleEffect)
            {
                GameObject.Destroy(bubbleEffect);
            }

            Physics.gravity = new Vector3(0, -9.8f, 0);
        }


        private void LoseBreath(Player player)
        {
            player.Health.TakeDamage(Time.deltaTime * lostBreathSpeed);
        }

        #endregion

        #region Unused Methods

        public override void FixedUpdateState(Player player)
        {
        }

        #endregion

        #region Custom Methods

        private IEnumerator EnablePlayerMovement()
        {
            yield return new WaitForSeconds(1.5f);

            PlayerMovementController.Instance.DisablePlayerMovement(false);
        }

        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation,
                Time.deltaTime * player.RotationSmoothness);
        }

        public void PlayerAtSurfact(Player player, bool status)
        {
            atSurface = status;

            player.AnimationController.SetBool(onSurface, status);


            //Play single shot of dive out

            if (status)
            {
                player.Health.ResetHealth();
                //TODO: Play interaction sound instead
                player.EffectsManager.PlayPlayerSound("DiveOut");
            }

            //play swim sound

            player.EffectsManager.PlayLoopInteraction("Swim", status);

            bubbleEffect.SetActive(!status);
            surfaceEffect.SetActive(status);
        }

        public void PlayerAtBottom(Player player, bool status)
        {
            atBottom = status;
        }

        #endregion
    }
}