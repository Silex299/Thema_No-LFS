

using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Player_Scripts.Player_States
{

    [System.Serializable]
    public class RestrictedUnderWaterMovement : PlayerBaseState
    {

        #region Variables


        [SerializeField, BoxGroup("Movement Variables")] private float movementSmoothness = 10f;
        [SerializeField, BoxGroup("Movement Variables")] private float playerRotationSmoothness;
        [SerializeField, BoxGroup("Movement Variables")] private float speed = 10f;

        [SerializeField, BoxGroup("Misc")] private float lockedLeftPosition;
        [SerializeField, BoxGroup("Misc")] private float underWaterThresold = 1.5f;
        [SerializeField, BoxGroup("Misc")] private float breathLostFactor = 1;
        [SerializeField, BoxGroup("Misc")] private Vector3 climbOffset;


        private float breath = 100;
        private bool _isUnderWater = true;
        private Vector3 _surfacePoint;
        private Vector3 _movePlayerPosition;

        private float _playerDepth;
        private float _waterDepth;
        private float turnSmoothVelocity;



        private bool _movePlayer;
        private bool _rotate;

        private static readonly int StateIndex = Animator.StringToHash("State Index");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int Push = Animator.StringToHash("Push");


        #endregion


        #region mostly unsused methods

        public override void OnGizmos(PlayerController controller)
        {
            //DepthCheck(controller);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_movePlayerPosition, 0.3f);
        }

        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.5F)
        {

            controller.Player.AnimationController.CrossFade("UW", 0.2f);
            controller.Player.AnimationController.SetInteger(StateIndex, index);
            controller.Player.PlayerController.height = 0.33f;

        }

        public override void OnStateExit(PlayerController controller)
        {
            controller.Player.PlayerController.height = 1.43f;
        }


        public override void SimpleInteract(PlayerController controller, int value = 0)
        {
        }


        #endregion



        #region overriden Methods


        public override void OnStateUpdate(PlayerController controller)
        {

            //Get inputs
            var HorizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");


            //Rotate player
            if (Input.GetButton("Horizontal") && !_rotate)
            {
                RotatePlayer(controller, HorizontalInput);
            }

            if (_rotate)
            {
                RotatePlayer(controller, _movePlayerPosition);
            }


            //Move player
            if (controller.Player.PlayerController.enabled)
            {
                var direction = new Vector3(HorizontalInput, verticalInput, 0);
                controller.Player.PlayerController.Move(direction * (speed * (_isUnderWater ? 1 : 0.3f)) * Time.deltaTime);
            }



            //Update in animation controller
            controller.Player.AnimationController.SetInteger(StateIndex, _isUnderWater ? 0 : 1);
            controller.Player.AnimationController.SetFloat(Speed, Mathf.Abs(HorizontalInput), 0.2f, Time.deltaTime);
            controller.Player.AnimationController.SetFloat(Direction, verticalInput, 0.1f, Time.deltaTime);


            //Player Breath
            if (_isUnderWater)
            {
                LostBreath(controller);
            }
            else
            {
                breath = 100;
            }


            //Climb or Jump
            if (Input.GetButtonDown("Jump"))
            {


                switch (ProximityCheck(controller))
                {
                    case PlayerProximity.Vault:
                        controller.StartCoroutine(Climb(controller));
                        break;
                    case PlayerProximity.VaultLeft:
                        controller.StartCoroutine(Climb(controller, true, false));
                        break;
                    case PlayerProximity.VaultRight:
                        controller.StartCoroutine(Climb(controller, false, false));
                        break;
                }
            }

        }


        public override void OnStateLateUpdate(PlayerController controller)
        {
            



        }

        public override void OnStateFixedUpdate(PlayerController controller)
        {                
            
            //Player movement

            //Maintain same depth if player is above underwater level


            var transform = controller.transform;
            var position = transform.position;

            if (_playerDepth < 1.4f && !_movePlayer)
            {

                if (Input.GetAxis("Vertical") >= 0)
                {
                    var newPos = position;
                    newPos.y = (_surfacePoint + Vector3.down * 1.2f).y;
                    newPos.z = lockedLeftPosition;


                    transform.position = Vector3.MoveTowards(position, newPos, Time.fixedDeltaTime * movementSmoothness);
                }
            }

            //Move Player for climb
            if (_movePlayer)
            {
                transform.position = Vector3.MoveTowards(position, _movePlayerPosition, Time.fixedDeltaTime * movementSmoothness);

                if (Vector3.Distance(position, _movePlayerPosition) < 0.1f)
                {
                    _movePlayer = false;
                }

            }
            DepthCheck(controller);
        }


        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {
            if(type == InteractionType.Push)
            {

                if (!status)
                {
                    controller.Player.AnimationController.SetBool(Push, false);
                    return false;
                }

                switch (PushProximity(controller))
                {

                    case PlayerProximity.Vault:
                        controller.Player.AnimationController.SetBool(Push, true);
                        controller.Player.AnimationController.SetFloat(Direction, 0, 0.1f, Time.deltaTime);
                        return true;
                    case PlayerProximity.VaultLeft:
                        if (Input.GetButton("f"))
                        {
                            controller.Player.AnimationController.SetBool(Push, true);
                            var direction = Mathf.MoveTowards(controller.Player.AnimationController.GetFloat(Direction), -1, Time.deltaTime * 10f);
                            controller.Player.AnimationController.SetFloat(Direction, direction);
                            return true;
                        }
                        break;
                    case PlayerProximity.VaultRight:
                        if (Input.GetButton("f"))
                        {
                            controller.Player.AnimationController.SetBool(Push, true);
                            var direction = Mathf.MoveTowards(controller.Player.AnimationController.GetFloat(Direction), 1, Time.deltaTime * 10f);
                            controller.Player.AnimationController.SetFloat(Direction, direction);
                            return true;
                        }
                        break;
                    default:
                        return false;

                }

                return false;
            }

            return false;

        }


        #endregion


        #region Custom methods

        private void LostBreath(PlayerController controller)
        {
            breath -= Time.deltaTime * breathLostFactor;

            if (breath <= 0)
            {
                controller.Player.Health.TakeDamage(100);
            }
        }

        private void RotatePlayer(PlayerController controller, float angle)
        {

            var tangentAngle = Mathf.Asin(angle) * Mathf.Rad2Deg;

            float yAngle = Mathf.SmoothDampAngle(controller.transform.eulerAngles.y, tangentAngle, ref turnSmoothVelocity, playerRotationSmoothness);
            controller.transform.rotation = Quaternion.Euler(0, yAngle, 0);

        }

        private void RotatePlayer(PlayerController controller, Vector3 lookAt)
        {
            var direction = lookAt - controller.transform.position;

            float tangentAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(controller.transform.eulerAngles.y, tangentAngle, ref turnSmoothVelocity, playerRotationSmoothness);
            controller.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        private void DepthCheck(PlayerController controller)
        {

            var position = controller.transform.position;

            //Collide only with water
            if (Physics.Raycast(position, Vector3.up, out RaycastHit hit, Mathf.Infinity, 1 << 4))
            {
                _playerDepth = hit.distance;
                _surfacePoint = hit.point;
                //TODO: REMOVE
                Debug.DrawLine(position, hit.point, Color.cyan, 1f);
            }


            //Collide only with ground
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit1, Mathf.Infinity, 1 << 7))
            {
                _waterDepth = _playerDepth + hit1.distance;
                //TODO: REMOVE
                Debug.DrawLine(position, hit1.point, Color.blue, 1f);
            }


            _isUnderWater = _playerDepth > underWaterThresold;


            if (_waterDepth < underWaterThresold)
            {
                Debug.Log("ChangeState");
            }


        }

        private PlayerProximity PushProximity(PlayerController controller)
        {
            var mask = ~(1 << 7) & controller.Player.raycastMask;

            var transform = controller.transform;
            var position = transform.position + controller.Player.firstRaycastHeight * transform.up;
            var forward = transform.forward;
            var right = transform.right;

            if (Physics.Raycast(position, forward, out RaycastHit hit1, 1f, mask))
            {
                Debug.DrawLine(position, hit1.point, Color.green, 1f);
                return PlayerProximity.Vault;
            }
            else
            {
                Debug.DrawLine(position, position + forward, Color.red, 1f);
            }

            if(Physics.Raycast(position, right, out RaycastHit hit2, 1f, mask))
            {
                Debug.DrawLine(position, hit2.point, Color.green, 1f);
                return PlayerProximity.VaultRight;
            }
            else
            {
                Debug.DrawLine(position, position + right, Color.red, 1f);
            }

            if (Physics.Raycast(position, -right, out RaycastHit hit3, 1f, mask))
            {
                Debug.DrawLine(position, hit3.point, Color.green, 1f);
                return PlayerProximity.VaultLeft;
            }

            return PlayerProximity.Jump;

        }

        private PlayerProximity ProximityCheck(PlayerController controller)
        {
            var trasnform = controller.transform;
            var position = trasnform.position;
            var forward = trasnform.forward;
            var up = trasnform.up;
            var right = trasnform.right;

            LayerMask mask = ~(1<<7);
            Debug.Log(mask);
            mask = mask & controller.Player.raycastMask;



            // TODO REMOVE DEBUG

            RaycastHit forwardHit;
            RaycastHit downwardHit;

            //Forward raycast
            if (!Physics.Raycast(position + controller.Player.secondRaycastHeight * up, forward, out forwardHit, controller.Player.raycastDistance, mask))
            {
                if (Physics.Raycast(position + controller.Player.secondRaycastHeight * up + forward * 1f, -up, out downwardHit, controller.Player.raycastDistance, mask))
                {
                    Debug.DrawLine(position + controller.Player.secondRaycastHeight * up + forward * 1f, downwardHit.point, Color.red, 2f);
                    Debug.Log(downwardHit.collider);
                    _movePlayerPosition = downwardHit.point;
                    return PlayerProximity.Vault;
                }
            }

            //Left Raycast
            if (!Physics.Raycast(position + controller.Player.secondRaycastHeight * up, -right, out forwardHit, controller.Player.raycastDistance, mask))
            {
                if (Physics.Raycast(position + controller.Player.secondRaycastHeight * up - right * 1f, -up, out downwardHit, controller.Player.raycastDistance, mask))
                {
                    Debug.DrawLine(position + controller.Player.secondRaycastHeight * up + forward * 1f, downwardHit.point, Color.red, 2f);
                    _movePlayerPosition = downwardHit.point;
                    return PlayerProximity.VaultLeft;
                }
            }

            //Right Raycast
            if (!Physics.Raycast(position + controller.Player.secondRaycastHeight * up, right, out forwardHit, controller.Player.raycastDistance, mask))
            {
                if (Physics.Raycast(position + controller.Player.secondRaycastHeight * up + right * 1f, -up, out downwardHit, controller.Player.raycastDistance, mask))
                {
                    Debug.DrawLine(position + controller.Player.secondRaycastHeight * up + forward * 1f, downwardHit.point, Color.red, 2f);
                    _movePlayerPosition = downwardHit.point;
                    return PlayerProximity.VaultRight;
                }
            }


            return PlayerProximity.Jump;

        }

        private IEnumerator Climb(PlayerController controller, bool isLeft = false, bool direct = true)
        {
            if (!direct)
            {
                _rotate = true;

                yield return new WaitForSeconds(0.5f);

                _rotate = false;
            }
            controller.Player.PlayerController.enabled = false;

            _movePlayerPosition = GetClimbPosition(controller);
            _movePlayer = true;

            yield return new WaitUntil(() => { return !_movePlayer; });


            controller.CrossFadeAnimation("climb");

            yield return new WaitForSeconds(1f);

            controller.ChangeState(PlayerController.PlayerStates.BasicFreeMovement);
        }


        private Vector3 GetClimbPosition(PlayerController controller)
        {

            var transform = controller.transform;
            var position = transform.position;
            var forward = transform.forward;
            var up = transform.up;



            if(Physics.Raycast(position + controller.Player.firstRaycastHeight * up, forward, out RaycastHit hit, controller.Player.raycastDistance, ~(1<<7) & controller.Player.raycastMask))
            {
                Debug.DrawLine(position + controller.Player.firstRaycastHeight * up, hit.point);

                var pos = position + transform.forward * hit.distance;

                pos.y = hit.point.y;
                return pos + (climbOffset.x * forward + climbOffset.y * up + climbOffset.z * transform.right);

            }
            else
            {
                return _movePlayerPosition + (climbOffset.x * forward + climbOffset.y * up + climbOffset.z * transform.right);
            }

        }

        #endregion



    }
}