using System.Collections;
using UnityEngine;

namespace Player_Scripts.Player_States
{
    [System.Serializable]
    public class LadderMovement : PlayerBaseState
    {

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Jump = Animator.StringToHash("Jump");


        [SerializeField] private float firstForwardRaycastHeight;
        [SerializeField] private float secondForwardRaycastHeight;
        [SerializeField, Space] private float groundProximity = 3f;

        private bool _atLadderBegining;
        private bool _atLadderEnd;

        private bool _exit;
        private bool _fall;
        private bool _isGrounded;

        #region Unused Methods


        public override void OnStateLateUpdate(PlayerController controller)
        {
        }
        public override void OnStateFixedUpdate(PlayerController controller)
        {
        }
        public override void OnStateExit(PlayerController controller)
        {
        }
        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {
            return false;
        }


        #endregion

        #region Overriden Methods

        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.5f)
        {
            _fall = false;
            _exit = false;

            controller.Player.AnimationController.CrossFade("Ladder", stateTransitionTime, 0);
        }

        public override void OnStateUpdate(PlayerController controller)
        {
            if (_fall)
            {
                Gravity(controller);
                GroundCheck(controller);

                if (!_isGrounded) return;

                _fall = false;
                controller.ChangeState(PlayerController.PlayerStates.BasicRestrictedMovement);

                return;
            }
            else
            {
                ProcessProximity(controller);
            }


            var speed = Input.GetAxis("Vertical");


            if (speed > 0 && _atLadderEnd && !_exit)
            {
                controller.StartCoroutine(ClimbOffLadder(controller));
            }
            else if (speed < 0 && _atLadderBegining && !_exit)
            {
                FallOffLadder(controller);
            }
            else
            {
                controller.Player.AnimationController.SetFloat(Speed, speed, 0.001f, Time.deltaTime);
            }



            if (Input.GetButtonDown("Jump") && !_exit)
            {
                if (!_atLadderEnd)
                {
                    controller.StartCoroutine(JumpOffLadder(controller));
                }
                else if (_atLadderBegining)
                {
                    FallOffLadder(controller);
                }
                else
                {
                    controller.StartCoroutine(ClimbOffLadder(controller));
                }

            }

        }

        public override void SimpleInteract(PlayerController controller, int value = 0)
        {
            controller.StartCoroutine(JumpOffLadder(controller));
        }

        public override void OnGizmos(PlayerController controller)
        {
            ProcessProximity(controller);
        }

        #endregion


        #region Custom Methods

        private void Gravity(PlayerController controller)
        {
            if (!controller.Player.PlayerController.enabled) return;

            controller.Player.PlayerController.Move(new Vector3(0, -Time.deltaTime * 9.8f, 0));
        }

        private void GroundCheck(PlayerController controller)
        {
            if (Physics.SphereCast(new Vector3(controller.transform.position.x, controller.transform.position.y + controller.Player.sphereCastOffset, controller.transform.position.z),
                controller.Player.sphereCastRadius, -controller.transform.up, out RaycastHit hit, 5f, controller.Player.raycastMask))
            {
                _isGrounded = hit.distance <= controller.Player.groundOffset + controller.Player.sphereCastOffset + 1f;

            }
            else
            {
                _isGrounded = false;
            }
        }
        private void ProcessProximity(PlayerController controller)
        {

            var transform = controller.transform;
            var position = transform.position;

            float[] proximity = new float[3] { 0, 0, 0 };

            //TODO: REMVOVE DEBUG

            //FirstRaycast
            if (Physics.Raycast(position + Vector3.up * firstForwardRaycastHeight, transform.forward, out RaycastHit hit, controller.Player.raycastDistance, controller.Player.raycastMask))
            {
                Debug.DrawLine(position + Vector3.up * firstForwardRaycastHeight, hit.point, Color.red);
                proximity[0] = hit.distance;
            }
            else
            {
                Debug.DrawLine(position + Vector3.up * firstForwardRaycastHeight, position + Vector3.up * firstForwardRaycastHeight + transform.forward * controller.Player.raycastDistance);
            }

            //SecondRaycast
            if (Physics.Raycast(position + Vector3.up * secondForwardRaycastHeight, transform.forward, out RaycastHit hit1, controller.Player.raycastDistance, controller.Player.raycastMask))
            {
                Debug.DrawLine(position + Vector3.up * secondForwardRaycastHeight, hit1.point, Color.red);
                proximity[1] = hit1.distance;
            }
            else
            {
                Debug.DrawLine(position + Vector3.up * secondForwardRaycastHeight, position + Vector3.up * secondForwardRaycastHeight + transform.forward * controller.Player.raycastDistance, Color.yellow);
            }

            if (Physics.Raycast(position, -transform.up, out RaycastHit hit2, Mathf.Infinity, controller.Player.raycastMask))
            {
                Debug.DrawLine(position, hit2.point, Color.red);
                proximity[2] = hit2.distance;
            }
            else
            {
                Debug.DrawLine(position, position - Vector3.up * controller.Player.raycastDistance);
            }



            if (proximity[1] > 0.5f || proximity[1] == 0)
            {
                
                _atLadderEnd = true;
            }
            else
            {
                _atLadderEnd = false;
            }

            if (proximity[2] <= groundProximity)
            {
                _atLadderBegining = true;
            }
            else
            {
                _atLadderBegining = false;
            }


        }
        private IEnumerator JumpOffLadder(PlayerController controller)
        {
            if (_exit) yield break;

            _exit = true;

            if (!controller.Player.PlayerController.enabled)
            {
                yield break;
            }

            controller.Player.AnimationController.CrossFade("Jump off wall", 0.2f, 0);

            yield return new WaitForSeconds(1f);

            _fall = true;


        }

        private void FallOffLadder(PlayerController controller)
        {
            controller.Player.AnimationController.CrossFade("Ladder Fall", 0.2f, 0);
            _fall = true;
            _exit = true;
        }
        private IEnumerator ClimbOffLadder(PlayerController controller)
        {
            if (_exit) yield break;

            _exit = true;
            controller.CrossFadeAnimation("climb");
            yield return new WaitForSeconds(1.5f);
            controller.ChangeState(PlayerController.PlayerStates.BasicRestrictedMovement, 0);
        }

        #endregion




    }
}
