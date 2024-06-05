using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.States
{

    [Serializable]
    public class FreeBasicMovement : PlayerBaseStates
    {

        #region Variables
        [SerializeField, BoxGroup("Weapon")] private bool canEquipeMelee;
        private bool _meleeEquiped;
        private bool _canAttack = true;
        private Transform _cameraTransform;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int Jump = Animator.StringToHash("Jump");
        #endregion

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
            _cameraTransform = Camera.main.transform;
            player.AnimationController.CrossFade("Free Movement", 0.2f);
        }

        public override void UpdateState(Player player)
        {

            player.MovementController.GroundCheck();
            player.MovementController.ApplyGravity();


            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");


            if (player.canBoost)
            {

                if (Input.GetButton("Sprint"))
                {

                    Debug.Log("Sprint");
                    vertical = 2 * vertical;

                    if (vertical < 0.4f && vertical > -0.05f)
                    {
                        horizontal = 2 * horizontal;
                    }

                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                player.AnimationController.SetTrigger(Jump);
                player.StartCoroutine(ResetJump(player));
            }

            player.CController.Move(player.playerVelocity * Time.deltaTime);


            float speed = Mathf.Sqrt(vertical * vertical + horizontal * horizontal);

            player.AnimationController.SetFloat(Speed, speed, 0.1f, Time.deltaTime);

            if (MathF.Abs(vertical) > 0.1f || MathF.Abs(horizontal) > 0.1f)
            {
                RotatePlayer(player, vertical, horizontal);
            }


            EquipeMelee(player);
            MeleeActions(player);


        }


        #endregion

        #region Custom Methods

        private IEnumerator ResetJump(Player player)
        {
            yield return new WaitForSeconds(0.3f);
            player.AnimationController.ResetTrigger(Jump);
        }


        /// <summary> Rotate the player to the direction of the camera added to the input
        /// <param name="player"> The player to rotate </param>
        /// <param name="vertical"> The vertical input </param>
        /// <param name="horizontal"> The horizontal input </param>
        /// </summary>
        private void RotatePlayer(Player player, float vertical = 0, float horizontal = 0)
        {
            Vector3 forward = _cameraTransform.forward;
            forward.y = 0;
            forward = forward * vertical + _cameraTransform.right * horizontal;
            forward.Normalize();
            player.transform.forward = Vector3.Lerp(player.transform.forward, forward, Time.deltaTime * player.RotationSmoothness);
        }


        #region Melee Actions

        /// <summary> Equipe the melee weapon
        /// <param name="player"> The player to equipe the weapon </param>
        /// </summary>
        private void EquipeMelee(Player player)
        {
            if (!canEquipeMelee) return;

            if (Input.GetButtonDown("Primary"))
            {
                if (player.IsGrounded)
                {
                    _meleeEquiped = !_meleeEquiped;
                    player.AnimationController.SetBool("Melee", _meleeEquiped);
                }
            }

        }

        /// <summary> Actions for the melee weapon
        /// <param name="player"> The player to attack </param>
        /// </summary>
        private void MeleeActions(Player player)
        {
            if (!_meleeEquiped || !player.IsGrounded || !_canAttack)
                return;

            if (Input.GetButtonDown("Fire1"))
            {
                string attackString = "Move_Attack_" + UnityEngine.Random.Range(1, 6);
                Debug.Log(attackString);
                player.AnimationController.CrossFade(attackString, 0.2f, 0);

                player.StartCoroutine(ResetCanBoost(player));
            }
        }

        #endregion
        
        private IEnumerator ResetCanBoost(Player player)
        {
            player.canBoost = false;
            _canAttack = false;
            yield return new WaitForSeconds(1);
            player.canBoost = true;
            _canAttack = true;
        }

        #endregion

    }

}