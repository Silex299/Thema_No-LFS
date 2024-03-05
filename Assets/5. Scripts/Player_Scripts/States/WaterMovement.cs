using UnityEngine;


namespace Player_Scripts.States
{

    [System.Serializable]
    public class WaterMovement : PlayerBaseStates
    {

        private readonly static int Horizontal = Animator.StringToHash("Speed");
        private readonly static int Vertical = Animator.StringToHash("Direction");

        [SerializeField] private float swimSpeed = 10;
        public float restrictedYPosition = 7.1f;



        //Defines player position if player is at the surface of the water body or at bottom depth
        internal bool atBottom;
        internal bool atSurface;


        private float previousCharacterHeight;

        #region Overriden Methods
        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Basic Under Water", 0.2f);
            previousCharacterHeight = player.CController.height;
            player.CController.height = 0.7f;
        }
        public override void UpdateState(Player player)
        {

            //Basic movement and animator

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

            Vector3 movementVector = new Vector3(0, verticalInput, -horizontalInput);
            player.CController.Move(movementVector * swimSpeed * Time.deltaTime);


            player.AnimationController.SetFloat(Horizontal, Mathf.Abs(horizontalInput), 0.2f, Time.deltaTime);
            player.AnimationController.SetFloat(Vertical, verticalInput, 0.2f, Time.deltaTime);

            Rotate(player, horizontalInput);



        }


        public override void LateUpdateState(Player player)
        {
            if (atSurface && Input.GetAxis("Vertical") > 0)
            {
                AboveWater(player);
            }
        }

        public override void ExitState(Player player)
        {
            player.CController.height = previousCharacterHeight;
        }

        #endregion

        #region Unused Methods

        public override void FixedUpdateState(Player player)
        {
        }

        #endregion

        #region Custom Methods

        private void Rotate(Player player, float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) < 0.3f) return;

            Quaternion newRotation = Quaternion.LookRotation(-horizontalInput * Vector3.forward, Vector3.up);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * player.RotationSmoothness);

        }

        private void AboveWater(Player player)
        {
            //Restrict Y position;
            Vector3 playerPos = player.transform.position;
            playerPos.y = restrictedYPosition;
            player.transform.position = playerPos;

        }


        #endregion

    }

}