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

        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Fall in Water", 0.1f);
        }

        public override void ExitState(Player player)
        {
        }

        public override void UpdateState(Player player)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            #region  Player Movement

            Rotate(player, horizontalInput);
            player.CController.Move(new Vector3(0, verticalInput * swimSpeed * Time.deltaTime,
                -horizontalInput * swimSpeed * Time.deltaTime));

            #endregion


            #region Animation Upadate

            player.AnimationController.SetFloat("Speed", Mathf.Abs(horizontalInput));
            player.AnimationController.SetFloat("Direction", verticalInput);

            #endregion
            
        }

        public override void FixedUpdateState(Player player)
        {
        }

        public override void LateUpdateState(Player player)
        {
        }


        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation,
                Time.deltaTime * player.RotationSmoothness);
        }
    }
}