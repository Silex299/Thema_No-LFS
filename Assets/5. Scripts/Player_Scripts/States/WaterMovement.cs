using UnityEngine;


namespace Player_Scripts.States
{

    public class WaterMovement : PlayerBaseStates
    {

        private readonly static int Horizontal = Animator.StringToHash("Speed");
        private readonly static int Vertical = Animator.StringToHash("Direction");

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


            player.AnimationController.SetFloat(Horizontal, -horizontalInput, 0.2f, Time.deltaTime);
            player.AnimationController.SetFloat(Vertical, verticalInput, 0.2f, Time.deltaTime);


        }
    }

}