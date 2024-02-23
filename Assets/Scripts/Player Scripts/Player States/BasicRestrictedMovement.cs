using System.Collections;
using Path_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Player_States
{
    [System.Serializable]
    public class BasicRestrictedMovement : PlayerBaseState
    {

        protected PlayerPathController path;
        protected float turnSmoothVelocity;

        protected bool isGrounded;
        /// <summary>
        /// saves all the raycast hits distances(two front, two left, two right respectively)
        /// </summary>
        private float[] _raycastHitDistances = new float[6];

        private bool _climb;
        private Vector3 _climbPosition;
        [SerializeField, BoxGroup("Vault Variables")] private Vector3 climbOffset;
        [SerializeField, BoxGroup("Vault Variables")] private float climbSmoothness;


        [SerializeField, BoxGroup("Misc")] protected float playerRotationSmoothness = 0.05f;

        protected bool _isPushing;
        // Side rotation
        private bool _overrideRotation;
        private Vector3 _overriderRotationPosition;
        private static readonly int PushDirection = Animator.StringToHash("Direction");
        private static readonly int Push = Animator.StringToHash("Push");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsGrounded = Animator.StringToHash("Is Grounded");
        private static readonly int PlayerJump = Animator.StringToHash("Jump");
        private static readonly int StateIndex = Animator.StringToHash("State Index");

        #region Overriden Methods

        /// <summary>
        /// index = 0 normal movement
        /// index = 1 water movement
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="index"></param>
        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.2f)
        {
            GroundCheck(controller);
            path = PlayerPathController.Instance;

            controller.Player.AnimationController.SetInteger(StateIndex, index);

            if (!isGrounded)
            {
                controller.Player.AnimationController.CrossFade("BRM_Fall", stateTransitionTime);
            }
            else
            {
                if (controller.initState == PlayerController.PlayerStates.BasicRestrictedMovement) return;
                switch (index)
                {
                    case -1:
                        controller.Player.AnimationController.CrossFade("Walk", stateTransitionTime);
                        break;
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

            float input = Input.GetAxis(controller.Player.invertedAxis ? "Horizontal" : "Vertical");

            var multiplier = Input.GetButton("Sprint") && controller.Player.canSprint ? 2 : 1;
            
            controller.Player.AnimationController.SetFloat(Speed, Mathf.Abs(input) * multiplier, 0.1f, Time.deltaTime);

            
            RotatePlayer(controller);
            GroundCheck(controller);

            //Set Grounded
            controller.Player.AnimationController.SetBool(IsGrounded, isGrounded);

            // Jump Action
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
                    case PlayerProximity.VaultLeft:
                        controller.StartCoroutine(SideClimb(controller, true));
                        break;
                    case PlayerProximity.VaultRight:
                        controller.StartCoroutine(SideClimb(controller, false));
                        break;
                    case PlayerProximity.GoUnder:
                        break;
                    default:
                        controller.StartCoroutine(Jump(controller));
                        break;
                }
            }

        }

        public override void OnGizmos(PlayerController controller)
        {
            Raycast(controller);

            Transform transform = controller.transform;
            Vector3 pos = transform.position;
            Vector3 firstRaycastPos = new Vector3(pos.x, pos.y + controller.Player.firstRaycastHeight, pos.z);
            Vector3 secondRaycastPos = new Vector3(pos.x, pos.y + controller.Player.secondRaycastHeight, pos.z);

            // draw raycast positions
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(pos.x, controller.Player.firstRaycastHeight, pos.z), 0.05f);
            Gizmos.DrawSphere(new Vector3(pos.x, controller.Player.secondRaycastHeight, pos.z), 0.05f);

            // draw rays
            #region Forward

            if (_raycastHitDistances[0] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(firstRaycastPos, transform.forward * _raycastHitDistances[0]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(firstRaycastPos, transform.forward * controller.Player.raycastDistance);
            }

            if (_raycastHitDistances[1] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(secondRaycastPos, transform.forward * _raycastHitDistances[1]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(secondRaycastPos, transform.forward * controller.Player.raycastDistance);
            }

            #endregion

            #region Left

            if (_raycastHitDistances[2] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(firstRaycastPos, -transform.right * _raycastHitDistances[2]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(firstRaycastPos, -transform.right * controller.Player.raycastDistance);
            }

            if (_raycastHitDistances[3] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(secondRaycastPos, -transform.right * _raycastHitDistances[3]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(secondRaycastPos, -transform.right * controller.Player.raycastDistance);
            }

            #endregion

            #region Right

            if (_raycastHitDistances[4] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(firstRaycastPos, transform.right * _raycastHitDistances[4]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(firstRaycastPos, transform.right * controller.Player.raycastDistance);
            }

            if (_raycastHitDistances[5] != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(secondRaycastPos, transform.right * _raycastHitDistances[5]);
            }
            else
            {
                Gizmos.color = Color.red;

                Gizmos.DrawRay(secondRaycastPos, transform.right * controller.Player.raycastDistance);
            }

            #endregion

            #region Vertical rays

            Gizmos.DrawSphere(_climbPosition, 0.05f);

            if (Application.isPlaying) return;

            var forward = transform.forward;
            var verticalRayPos = pos + new Vector3(forward.x * controller.Player.verticalRaycastOffset, 3f, forward.z * controller.Player.verticalRaycastOffset);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(verticalRayPos, verticalRayPos - (transform.up * controller.Player.verticalRaycastDistance));



            #endregion

        }

        public override void OnStateFixedUpdate(PlayerController controller)
        {
            Raycast(controller);
        }

        public override void OnStateLateUpdate(PlayerController controller)
        {
            if (_climb)
            {
                controller.transform.position = Vector3.Lerp(controller.transform.position, _climbPosition, Time.deltaTime * climbSmoothness);

                if (Mathf.Abs((controller.transform.position - _climbPosition).magnitude) < 0.1f)
                {
                    _climb = false;
                }
            }
        }

        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {

            switch (type)
            {
                case InteractionType.Push:

                    if (!status)
                    {
                        controller.Player.AnimationController.SetBool(Push, false);
                        return false;
                    }

                    if (!controller.Player.PlayerController.enabled) return false;
                    switch (ProcessPlayerProximity())
                    {
                        //If box is in front of the player. Push without any user input
                        case PlayerProximity.Vault:

                            if (_raycastHitDistances[0] > 0.45f) return false;

                            if (!_isPushing)
                            {
                                _isPushing = true;
                            }
                            controller.Player.AnimationController.SetBool(Push, true);
                            controller.Player.AnimationController.SetFloat(PushDirection, 0);
                            return true;

                        case PlayerProximity.VaultLeft:
                        case PlayerProximity.BlockedLeft:

                            if (Input.GetButton("f"))
                            {
                                if (!_isPushing)
                                {
                                    _isPushing = true;
                                }
                                controller.Player.AnimationController.SetBool(Push, true);
                                controller.Player.AnimationController.SetFloat(PushDirection, 1);
                                return true;
                            }
                            else
                            {
                                _isPushing = false;
                                controller.Player.AnimationController.SetBool(Push, false);
                                return false;
                            }

                        case PlayerProximity.VaultRight:
                        case PlayerProximity.BlockedRight:

                            if (Input.GetButton("f"))
                            {
                                if (!_isPushing)
                                {
                                    _isPushing = true;
                                }
                                controller.Player.AnimationController.SetBool(Push, true);
                                controller.Player.AnimationController.SetFloat(PushDirection, -1);
                                return true;
                            }
                            else
                            {
                                _isPushing = false;
                                controller.Player.AnimationController.SetBool(Push, false);
                                return false;
                            }

                        default:
                            _isPushing = false;
                            controller.Player.AnimationController.SetBool(Push, false);
                            return false;
                    }
                default:
                    return false;
            }
        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Rotated player in desired direction
        /// </summary>
        protected void RotatePlayer(PlayerController controller)
        {


            if (!controller.Player.canRotate) return;


            Vector3 direction;

            //Get Direction to rotate to
            if (_overrideRotation)
            {
                direction = _overriderRotationPosition;
            }

            //Horizontal axis
            else if (controller.Player.invertedAxis)
            {
                if (Input.GetAxis("Horizontal") > 0f)
                {
                    direction = path.PathPoints[path.nextDestination].position - controller.transform.position;
                }
                else if (Input.GetAxis("Horizontal") < 0f)
                {
                    direction = path.PathPoints[path.previousDestination].position - controller.transform.position;
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
                    direction = path.PathPoints[path.nextDestination].position - controller.transform.position;
                }
                else if (Input.GetAxis("Vertical") < -0.2f)
                {
                    direction = path.PathPoints[path.previousDestination].position - controller.transform.position;
                }
                else
                {
                    return;
                }
            }

            //Rotate the player in desired direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            float angle = Mathf.SmoothDampAngle(controller.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, playerRotationSmoothness);
            controller.transform.rotation = Quaternion.Euler(0, angle, 0);

        }

        /// <summary>
        /// Check if player is grounded or not
        /// </summary>
        /// <param name="controller">player transform</param>
        protected void GroundCheck(PlayerController controller)
        {

            #region New Ground check

            //TODO: remove redundant parts

            var rayOrigin = new Vector3(controller.transform.position.x, controller.transform.position.y + controller.Player.sphereCastOffset, controller.transform.position.z);
            var down = -controller.transform.up;
            var forward = controller.transform.forward;
            var right = controller.transform.right;


            if(Physics.Raycast(rayOrigin, down, out RaycastHit hit1, 5f, controller.Player.raycastMask))
            {
                Debug.DrawLine(rayOrigin, hit1.point, Color.yellow, 1f);
                isGrounded = hit1.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
                if (isGrounded) return;
            }

            if (Physics.Raycast(rayOrigin + controller.Player.sphereCastRadius * forward , down, out RaycastHit hit2, 5f, controller.Player.raycastMask))
            {
                Debug.DrawLine(rayOrigin, hit2.point, Color.yellow, 1f);
                isGrounded = hit2.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
                if (isGrounded) return;                 
            }

            if (Physics.Raycast(rayOrigin + controller.Player.sphereCastRadius * -forward, down, out RaycastHit hit3, 5f, controller.Player.raycastMask))
            {
                Debug.DrawLine(rayOrigin, hit3.point, Color.yellow, 1f);
                isGrounded = hit3.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
                if (isGrounded) return;
            }

            if (Physics.Raycast(rayOrigin + controller.Player.sphereCastRadius * right, down, out RaycastHit hit4, 5f, controller.Player.raycastMask))
            {
                Debug.DrawLine(rayOrigin, hit4.point, Color.yellow, 1f);
                isGrounded = hit4.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
                if (isGrounded) return;

            }

            if (Physics.Raycast(rayOrigin + controller.Player.sphereCastRadius * -right, down, out RaycastHit hit5, 5f, controller.Player.raycastMask))
            {
                Debug.DrawLine(rayOrigin, hit5.point, Color.yellow, 1f);
                isGrounded = hit5.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset;
                if (isGrounded) return;
            }

            else
            {
                isGrounded = false;
            }



            #endregion

        }

        /// <summary>
        /// Sets and resets jump trigger
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        protected IEnumerator Jump(PlayerController controller)
        {
            if (isGrounded)
            {
                controller.Player.AnimationController.SetTrigger(PlayerJump);
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(1f);
            controller.Player.AnimationController.ResetTrigger(PlayerJump);
        }

        /// <summary>
        /// Starts climb action
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        protected IEnumerator StartClimb(PlayerController controller)
        {
            //Don't climb if far away
            if (_raycastHitDistances[0] == 0)
            {
                yield break;
            }

            Transform transform = controller.transform;
            Vector3 position = transform.position;
            Vector3 forward = transform.forward;
            var right = transform.right;

            Vector3 verticalRaycastPos = position + new Vector3(forward.x * controller.Player.verticalRaycastOffset, 3f, forward.z * controller.Player.verticalRaycastOffset);

            //more than one raycast
            var newLayerMask = controller.Player.raycastMask;
            newLayerMask &= ~(1 << 6);
            if (Physics.BoxCast(verticalRaycastPos, new Vector3(0.7f, 0.01f, 0.1f), -transform.up, out RaycastHit hit, Quaternion.LookRotation(transform.right), controller.Player.verticalRaycastDistance, newLayerMask))
            {
                Debug.DrawLine(verticalRaycastPos, hit.point, Color.red, 2f);

                _climbPosition = position + new Vector3(forward.x * _raycastHitDistances[0], 0, forward.z * _raycastHitDistances[0]);

                _climbPosition += new Vector3(climbOffset.z * forward.x + right.x * climbOffset.x, climbOffset.y + hit.point.y - position.y, climbOffset.z * forward.z + right.z * climbOffset.x);

                _climb = true;

                controller.CrossFadeAnimation("Sprint To Wall Climb", 0.1f);

                controller.Player.PlayerController.enabled = false;

                yield return new WaitUntil(() => !_climb);

                yield return new WaitForSeconds(1f);

                // ReSharper disable once Unity.InefficientPropertyAccess
                controller.Player.PlayerController.enabled = true;
                _climb = false;

            }


        }

        protected IEnumerator SideClimb(PlayerController controller, bool climbLeft)
        {
            Transform transform = controller.transform;
            Vector3 right = transform.right;

            if (_overrideRotation) yield break;

            if (climbLeft)
            {
                _overriderRotationPosition = -new Vector3(right.x, controller.Player.firstRaycastHeight, right.z);

                Debug.DrawLine(transform.position, _overriderRotationPosition, Color.white, 5f);
            }
            else
            {
                _overriderRotationPosition = new Vector3(right.x, controller.Player.firstRaycastHeight, right.z);
                Debug.DrawLine(transform.position, _overriderRotationPosition, Color.white, 5f);

            }

            _overrideRotation = true;

            yield return new WaitForSeconds(0.2f);

            controller.StartCoroutine(StartClimb(controller));

            yield return new WaitForSeconds(0.5f);

            _overrideRotation = false;

        }

        /// <summary>
        /// Raycasts in forward right and left to check players proximity
        /// </summary>
        /// <param name="controller"> player </param>
        private void Raycast(PlayerController controller)
        {
            var transform = controller.transform;
            Vector3 position = transform.position;
            Vector3 forward = transform.forward;
            Vector3 left = -transform.right;

            Vector3 firstRaycastPos = new Vector3(0, controller.Player.firstRaycastHeight, 0);
            Vector3 secondRaycastPos = new Vector3(0, controller.Player.secondRaycastHeight, 0);


            // Forward raycasts
            _raycastHitDistances[0] = Physics.Raycast(position + firstRaycastPos, forward, out RaycastHit hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;
            _raycastHitDistances[1] = Physics.Raycast(position + secondRaycastPos, forward, out hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;


            //Left raycasts
            _raycastHitDistances[2] = Physics.Raycast(position + firstRaycastPos, left, out hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;
            _raycastHitDistances[3] = Physics.Raycast(position + secondRaycastPos, left, out hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;

            //Right raycasts
            _raycastHitDistances[4] = Physics.Raycast(position + firstRaycastPos, -left, out hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;
            _raycastHitDistances[5] = Physics.Raycast(position + secondRaycastPos, -left, out hit, controller.Player.raycastDistance, controller.Player.raycastMask)
                ? hit.distance
                : 0;
        }

        /// <summary>
        /// Checks player's proximity states
        /// </summary>
        /// <returns></returns>
        protected PlayerProximity ProcessPlayerProximity()
        {
            if (_raycastHitDistances[0] != 0 && _raycastHitDistances[1] == 0)
            {
                return PlayerProximity.Vault;
            }
            if (_raycastHitDistances[2] != 0 && _raycastHitDistances[3] == 0)
            {
                return PlayerProximity.VaultLeft;
            }
            if (_raycastHitDistances[4] != 0 && _raycastHitDistances[5] == 0)
            {
                return PlayerProximity.VaultRight;
            }
            if (_raycastHitDistances[0] == 0 && _raycastHitDistances[1] != 0)
            {
                return PlayerProximity.GoUnder;
            }
            if(_raycastHitDistances[2] != 0&& _raycastHitDistances[3] != 0)
            {
                return PlayerProximity.BlockedLeft;
            }
            if(_raycastHitDistances[4] != 0 && _raycastHitDistances[5] != 0)
            {
                return PlayerProximity.BlockedRight;
            }

            return PlayerProximity.Jump;
        }

        #endregion

        #region Unused methods

        public override void OnStateExit(PlayerController controller)
        {
        }
        public override void SimpleInteract(PlayerController controller, int index = 0)
        {
        }


        #endregion


    }

}
