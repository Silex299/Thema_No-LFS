using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Player_Scripts.States
{

    [Serializable]
    public class FreeBasicMovement : PlayerBaseStates
    {

        #region Variables
        [SerializeField, BoxGroup("Weapon")] private bool canEquipeMelee;
        [SerializeField, BoxGroup("Weapon")] private Sword sword;


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
            #region Gravity, Ground Check
            player.MovementController.GroundCheck();
            player.MovementController.ApplyGravity();

            player.CController.Move(player.playerVelocity * Time.deltaTime);
            #endregion

            #region Inputs
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            #endregion

            #region Boost
            if (player.canBoost && Input.GetButton("Sprint"))
            {
                Debug.Log("Sprint");
                vertical *= 2;
                horizontal *= 2;
            }
            #endregion

            #region Jump
            if (Input.GetButtonDown("Jump"))
            {
                player.AnimationController.SetTrigger(Jump);
                player.StartCoroutine(ResetJump(player));
            }
            #endregion

            #region Animation
            float speed = Mathf.Max(Mathf.Abs(vertical), Mathf.Abs(horizontal));
            player.AnimationController.SetFloat(Speed, speed, 0.1f, Time.deltaTime);
            #endregion

            #region Rotation
            if (MathF.Abs(vertical) > 0.1f || MathF.Abs(horizontal) > 0.1f)
            {
                RotatePlayer(player, vertical, horizontal);
            }
            #endregion

            #region Melee
            EquipMelee(player);
            MeleeActions(player, speed);
            #endregion
        
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
        private void EquipMelee(Player player)
        {
            if (!canEquipeMelee) return;

            if (Input.GetButtonDown("Primary"))
            {
                if (player.IsGrounded)
                {
                    _meleeEquiped = !_meleeEquiped;

                    player.AnimationController.SetBool("Melee", _meleeEquiped);
                    if (sword)
                    {
                        sword.ActivateSword(_meleeEquiped);
                    }
                }
            }

        }

        /// <summary> Actions for the melee weapon
        /// <param name="player"> The player to attack </param>
        /// </summary>
        private void MeleeActions(Player player, float speed)
        {
            if (!_meleeEquiped || !player.IsGrounded || !_canAttack)
                return;

            sword.ActivateSword(speed < 1.5f);


            if (Input.GetButtonDown("Fire1"))
            {
                if (sword)
                {
                    sword.ActivateSword(true);
                }

                string attackString = "Move_Attack_" + UnityEngine.Random.Range(1, 5);
                Debug.Log(attackString);
                player.AnimationController.CrossFade(attackString, 0.2f, 0);

                _boostResetCoroutine = player.StartCoroutine(ResetCanBoost(player));
            }
        }

        #endregion

        private Coroutine _boostResetCoroutine;
        private IEnumerator ResetCanBoost(Player player)
        {
            if (_boostResetCoroutine != null)
                yield break;

            player.canBoost = false;
            _canAttack = false;
            
            yield return new WaitForSeconds(1f);

            player.canBoost = true;
            _canAttack = true;
            _boostResetCoroutine = null;
        }

        #endregion

    }

}