using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player
{
    public class WaterController : Controller
    {
        [FoldoutGroup("Movement")] public float speed = 10;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10;

        [FoldoutGroup("Water Property")] public bool constrainX;

        [FoldoutGroup("Water Property"), ShowIf(nameof(constrainX))]
        public float defaultX;

        [FoldoutGroup("Water Property")] public bool constrainZ;

        [FoldoutGroup("Water Property"), ShowIf(nameof(constrainZ))]
        public float defaultZ;

        [FoldoutGroup("Water Property"), Space(10)]
        public float surfaceLevel;

        [FoldoutGroup("Water Property")]
        public float bottomLevel;


        [TabGroup("Character Controller Properties", "Default")]
        public Vector3 defaultCenter;
        [TabGroup("Character Controller Properties", "Default")]
        public float defaultHeight, defaultRadius;

        [TabGroup("Character Controller Properties", "Forward")]
        public Vector3 forwardCenter;
        [TabGroup("Character Controller Properties", "Forward")]
        public float forwardHeight, forwardRadius;


        private bool _atSurface;
        private bool _atBottom;
        private bool _atDefaultHeight = true;
        
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int AtSurface = Animator.StringToHash("AtSurface");
        private static readonly int AtBottom = Animator.StringToHash("AtBottom");


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + transform.up * surfaceLevel,
                transform.position + transform.up * bottomLevel);
            
            Gizmos.color= Color.yellow;
            Gizmos.DrawSphere(transform.position + transform.up * surfaceLevel, 0.1f);
            Gizmos.DrawSphere(transform.position + transform.up * bottomLevel, 0.1f);
        }

        public override void ControllerUpdate(PlayerV1 player)
        {
            //get horizontal and vertical  input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            MovePlayer(player, horizontal, vertical);
            AdjustCharacterController(player, horizontal, vertical);
            UpdateAnimator(player, horizontal, vertical);
            Rotate(player, horizontal);
        }

        public override void ControllerLateUpdate(PlayerV1 player)
        {
            ConstrainPosition(player);
        }


        private void MovePlayer(PlayerV1 playerV1, float horizontalInput, float verticalInput)
        {
            var moveDirection = playerV1.transform.up * verticalInput +
                                playerV1.transform.forward * Mathf.Abs(horizontalInput);
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

        private void ConstrainPosition(PlayerV1 player)
        {
            Vector3 position = player.transform.position;

            if (constrainX)
                position.x = defaultX;
            if (constrainZ)
                position.z = defaultZ;

            if (position.y > surfaceLevel +1.9f)
            {
                if (Input.GetAxis("Vertical")>=0)
                {
                    position.y = Mathf.Lerp(position.y, surfaceLevel + 2f, Time.deltaTime * 50f);
                }
                _atSurface = true;
                _atBottom = false;
            }
            else if(position.y < bottomLevel + 2.1f)
            {
                if (Input.GetAxis("Vertical") <= 0)
                {
                    position.y = Mathf.Lerp(position.y, bottomLevel + 2f, Time.deltaTime * 50f);
                }

                _atSurface = false;
                _atBottom = true;
            }
            else
            {
                _atBottom = false;
                _atSurface = false;
            }

            player.transform.position = position;
            player.animator.SetBool(AtSurface, _atSurface);
            player.animator.SetBool(AtBottom, _atBottom);
        }

        private void AdjustCharacterController(PlayerV1 playerV1, float horizontalInput, float verticalInput)
        {
            if ((Mathf.Abs(horizontalInput) > 0.4f && Mathf.Abs(verticalInput) < 0.4f) || _atSurface)
            {
                if(!_atDefaultHeight) return;
                
                _atDefaultHeight = false;
                playerV1.characterController.center = forwardCenter;
                playerV1.characterController.height = forwardHeight;
                playerV1.characterController.radius = forwardRadius;
            }
            else
            {
                if (_atDefaultHeight) return;
                
                _atDefaultHeight = true;
                playerV1.characterController.center = defaultCenter;
                playerV1.characterController.height = defaultHeight;
                playerV1.characterController.radius = defaultRadius;
            }
        }
        
    }

    //Add Surface movements;
    //Add bottom blocks
}