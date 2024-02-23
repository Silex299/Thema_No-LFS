using Path_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace Player_Scripts.Player_States
{
    [System.Serializable]
    public class StealthRestrictedMovement : PlayerBaseState
    {

        #region Variabls

        [SerializeField]
        private float playerRotationSmoothness = 0.1f;

        private bool _isGrounded;

        //TODO check if you can make _path more direct
        private PlayerPathController _path;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("Is Grounded");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private float _turnSmoothVelocity;

        #endregion


        #region Unused methods

        public override void OnStateLateUpdate(PlayerController controller)
        {
        }
        public override void OnGizmos(PlayerController controller)
        {
        }

        public override void OnStateExit(PlayerController controller)
        {
        }

        public override void SimpleInteract(PlayerController controller, int value = 0)
        {

        }


        #endregion


        #region Overriden methods

        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.5f)
        {
            GroundCheck(controller);
            controller.Player.AnimationController.CrossFade(_isGrounded ? "SRM" : "SRM_Fall", stateTransitionTime);
            _path = PlayerPathController.Instance;
        }

        public override void OnStateUpdate(PlayerController controller)
        {
            float input = Input.GetAxis(controller.Player.invertedAxis ? "Horizontal" : "Vertical");

            controller.Player.AnimationController.SetFloat(Speed, input);
            //Set Grounded
            controller.Player.AnimationController.SetBool(IsGrounded, _isGrounded);


            RotatePlayer(controller, input > 0);

        }

        public override void OnStateFixedUpdate(PlayerController controller)
        {
            GroundCheck(controller);
        }

        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {

            if (type == InteractionType.CoverDirection)
            {
                controller.Player.AnimationController.SetFloat(Direction, value);
                return false;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Checks if player is grounded 
        /// </summary>
        /// <param name="controller"></param>
        private void GroundCheck(PlayerController controller)
        {
            if (Physics.SphereCast(
                new Vector3(controller.transform.position.x, controller.transform.position.y + controller.Player.sphereCastOffset,
                    controller.transform.position.z),
                controller.Player.sphereCastRadius, -controller.transform.up, out RaycastHit hit, 5f, controller.Player.raycastMask))
            {
                //TODO REMOVE DEBUG
                //Debug.DrawLine(new Vector3(controller.transform.position.x, controller.transform.position.y + controller.Player.sphereCastOffset, controller.transform.position.z), hit.point, Color.yellow, 2f);
                _isGrounded = hit.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
            }
            else
            {
                _isGrounded = false;
            }
        }

        private void RotatePlayer(PlayerController controller, bool rotate = false)
        {
            Vector3 direction;

            //Horizontal axis
            if (controller.Player.invertedAxis)
            {
                if (Input.GetAxis("Horizontal") > 0.2f)
                {
                    direction = _path.PathPoints[_path.nextDestination].position - controller.transform.position;
                }
                else if (Input.GetAxis("Horizontal") < -0.2f)
                {
                    direction = _path.PathPoints[_path.previousDestination].position - controller.transform.position;
                }
                else
                {
                    return;
                }
            }

            //Vertical axis
            else
            {
                if (Input.GetAxis("Vertical") > 0.2f)
                {
                    direction = _path.PathPoints[_path.nextDestination].position - controller.transform.position;
                }
                else if (Input.GetAxis("Vertical") < -0.2f)
                {
                    direction = _path.PathPoints[_path.previousDestination].position - controller.transform.position;
                }
                else
                {
                    return;
                }
            }

            //Rotate the player in desired direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (!rotate) targetAngle += 180f;
            float angle = Mathf.SmoothDampAngle(controller.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, playerRotationSmoothness);
            controller.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        #endregion

    }
}
