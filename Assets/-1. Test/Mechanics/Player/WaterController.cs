using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player
{
    public class WaterController : Controller
    {
        [FoldoutGroup("Movement")] public float speed = 10;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10;

        [FoldoutGroup("Water Property")] public bool constrainX;
        [FoldoutGroup("Water Property"), ShowIf(nameof(constrainX))] public float defaultX;
        [FoldoutGroup("Water Property")] public bool constrainZ;
        [FoldoutGroup("Water Property"), ShowIf(nameof(constrainZ))] public float defaultZ;


        [TabGroup("Character Controller Properties", "Default")]
        public Vector3 defaultCenter;

        [TabGroup("Character Controller Properties", "Base")]
        public float defaultHeight;


        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");

        public override void ControllerUpdate(PlayerV1 player)
        {
            print("Hello");

            //get horizontal and vertical  input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            MovePlayer(player, horizontal, vertical);
            UpdateAnimator(player, horizontal, vertical);
            Rotate(player, horizontal);
        }


        public override void ControllerLateUpdate(PlayerV1 player)
        {
            ConstrainX(player);
        }


        private void MovePlayer(PlayerV1 playerV1, float horizontalInput, float verticalInput)
        {
            var moveDirection = playerV1.transform.up * verticalInput + playerV1.transform.forward * Mathf.Abs(horizontalInput);
            moveDirection *= speed;
            
            playerV1.characterController.Move(moveDirection);
            
        }
        private void UpdateAnimator(PlayerV1 player, float horizontal, float vertical)
        {
            //Set movement speed and rotation
            player.animator.SetFloat(Speed, horizontal, 0.1f, Time.deltaTime);
            player.animator.SetFloat(Direction, vertical, 0.1f, Time.deltaTime);
        }
        private void Rotate(PlayerV1 player, float horizontalInput)
        {
            //Rotate player in forward direction of the controller if horizontal input is greater than 0 and vice versa

            Vector3 desiredRotation;

            switch (horizontalInput)
            {
                case > 0:
                    desiredRotation = new Vector3(1, 0, 0);
                    break;
                case < 0:
                    desiredRotation = new Vector3(-1, 0, 0);
                    break;
                default:
                    return;
            }

            player.transform.rotation = Quaternion.Slerp(player.transform.rotation,
                Quaternion.LookRotation(desiredRotation), rotationSpeed * Time.deltaTime);
        }


        private void ConstrainX(PlayerV1 player)
        {
            Vector3 position = player.transform.position;
            
            if(constrainX)
                position.x = defaultX;
            if(constrainZ)
                position.z = defaultZ;
            
            player.transform.position = position;
        }
        
    }
}