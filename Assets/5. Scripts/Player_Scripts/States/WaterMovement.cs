using UnityEngine;


namespace Player_Scripts.States
{

    [System.Serializable]
    public class WaterMovement : PlayerBaseStates
    {

        private readonly static int Horizontal = Animator.StringToHash("Speed");
        private readonly static int Vertical = Animator.StringToHash("Direction");

        [SerializeField] private float swimSpeed = 10;

        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Basic Under Water", 0.2f);
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



            //Basic movement and animator

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");


            Vector3 movementVector = new Vector3(0, verticalInput, -horizontalInput);
            player.CController.Move(movementVector * swimSpeed * Time.deltaTime);


            player.AnimationController.SetFloat(Horizontal, Mathf.Abs(horizontalInput), 0.2f, Time.deltaTime);
            player.AnimationController.SetFloat(Vertical, verticalInput, 0.2f, Time.deltaTime);

            Rotate(player, horizontalInput);



        }


        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness);

        }

    }

}