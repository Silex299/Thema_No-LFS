using MyCamera;
using UnityEngine;

namespace Player_Scripts.Player_States
{
    [System.Serializable]
    public class BasicFreeMovement : BasicRestrictedMovement
    {

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("Is Grounded");

        private static readonly int StateIndex = Animator.StringToHash("State Index");



        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.2f)
        {
            GroundCheck(controller);

            controller.Player.AnimationController.SetInteger(StateIndex, index);
            if (!isGrounded)
            {
                controller.Player.AnimationController.CrossFade("BRM_Fall", stateTransitionTime);
            }
            else
            {
                switch (index)
                {
                    case 0:
                        controller.Player.AnimationController.CrossFade("BRM", stateTransitionTime);
                        break;
                    case 1:
                        controller.Player.AnimationController.CrossFade("BRM_Water", stateTransitionTime);
                        break;
                    case 2:
                        controller.Player.AnimationController.CrossFade("BRM_Balance", stateTransitionTime);
                        break;
                    default:
                        break;
                }

            }
        }

        public override void OnStateUpdate(PlayerController controller)
        {
            Vector3 movementDirection = Vector3.zero;
            movementDirection.x = Input.GetAxis("Vertical");
            movementDirection.z = Input.GetAxis("Horizontal");

            GroundCheck(controller);

            controller.Player.AnimationController.SetBool(IsGrounded, isGrounded);
            controller.Player.AnimationController.SetFloat(Speed, Mathf.Abs(movementDirection.magnitude), 0.1f, Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                switch (ProcessPlayerProximity())
                {
                    case PlayerProximity.Jump:
                        controller.StartCoroutine(Jump(controller));
                        break;
                    case PlayerProximity.Vault:
                        controller.StartCoroutine(StartClimb(controller));
                        break;
                    case PlayerProximity.GoUnder:
                        break;
                    default:
                        controller.StartCoroutine(Jump(controller));
                        break;
                }
            }

            if (Mathf.Abs(movementDirection.magnitude) > 0.2f)
            {
                RotatePlayer(controller, movementDirection);
            }

        }


        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {

            return base.Interact(controller, type, status, value);
        }

        private void RotatePlayer(PlayerController controller, Vector3 direction)
        {

            if (_isPushing)
            {
                if (controller.Player.invertedAxis)
                {
                    direction.x = 0;
                }
                else
                {
                    direction.z = 0;
                }
            }

            if (!controller.Player.canRotate) return;

            var forward = CameraManager.Instance.followCamera.transform.forward;
            var cameraAngle = Vector3.SignedAngle(new Vector3(forward.x, 0, forward.z), Vector3.forward, Vector3.up);

            //Rotate the player in desired direction
            var targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - cameraAngle - 00;

            var angle = Mathf.SmoothDampAngle(controller.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, playerRotationSmoothness);
            controller.transform.rotation = Quaternion.Euler(0, angle, 0);

        }



    }
}
