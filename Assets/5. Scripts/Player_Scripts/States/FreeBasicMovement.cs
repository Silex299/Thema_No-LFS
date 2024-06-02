using UnityEngine;

namespace Player_Scripts.States
{

    public class FreeBasicMovement : PlayerBaseStates
    {

        private Transform _cameraTransform;

        public override void EnterState(Player player)
        {
            _cameraTransform = Camera.main.transform;
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

            player.AnimationController.SetFloat("Speed", Input.GetAxis("Vertical"));

        }


        private void RotatePlayer(Player player)
        {
            // Get the direction the camera is facing.
            Vector3 cameraDirection = _cameraTransform.forward;
            cameraDirection.y = 0; // Prevent vertical rotation.

            // Calculate the rotation needed for the player to face the camera direction.
            Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);

            // Apply the rotation to the player, smoothly over time.
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * player.RotationSmoothness);
        }

    }

}