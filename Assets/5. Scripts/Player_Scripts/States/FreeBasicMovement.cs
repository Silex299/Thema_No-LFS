using System;
using System.Collections;
using Interactable_Items.Triggers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Video;

namespace Player_Scripts.States
{

    public class FreeBasicMovement : PlayerBaseStates
    {

        private Transform _cameraTransform;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int Jump = Animator.StringToHash("Jump");


        public override void EnterState(Player player)
        {
            _cameraTransform = Camera.main.transform;
            player.AnimationController.CrossFade("Free Movement", 0.2f);
        }

        public override void ExitState(Player player)
        {
        }

        public override void FixedUpdateState(Player player)
        {
        }

        public override void LateUpdateState(Player player)
        {
        }

        public override void UpdateState(Player player)
        {

            player.MovementController.GroundCheck();
            player.MovementController.ApplyGravity();


            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");


            if (Input.GetButton("Sprint"))
            {

                Debug.Log("Sprint");
                vertical = 2 * vertical;

                if (vertical < 0.4f && vertical > -0.05f)
                {
                    horizontal = 2 * horizontal;
                }

            }

            if(Input.GetButtonDown("Jump"))
            {
                player.AnimationController.SetTrigger(Jump);
                player.StartCoroutine(ResetJump(player));
            }

            player.CController.Move(player.playerVelocity * Time.deltaTime);


            player.AnimationController.SetFloat(Speed, Mathf.Sqrt(vertical*vertical + horizontal*horizontal), 0.1f, Time.deltaTime);

            if (MathF.Abs(vertical) > 0.1f || MathF.Abs(horizontal) > 0.1f)
            {
                RotatePlayer(player, vertical, horizontal);
            }


        }


        private IEnumerator ResetJump(Player player)
        {

            yield return new WaitForSeconds(0.3f);
            player.AnimationController.ResetTrigger(Jump);
            
        }


        private void RotatePlayer(Player player, float vertical = 0, float horizontal = 0)
        {
            Vector3 forward = _cameraTransform.forward;
            forward.y = 0;

            forward = forward * vertical + _cameraTransform.right * horizontal;

            forward.Normalize();

            player.transform.forward = Vector3.Lerp(player.transform.forward, forward, Time.deltaTime * player.RotationSmoothness);
        }

    }

}