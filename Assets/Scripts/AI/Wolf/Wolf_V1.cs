using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.AI.Wolf
{
    public class Wolf_V1 : MonoBehaviour
    {

        [BoxGroup("References")] public Animator animator;
        [BoxGroup("References")] public CharacterController controller;
        [BoxGroup("References")] public PathFinder pathFinder;
        [BoxGroup("References")] public Transform target;


        [BoxGroup("Raycast variables")] public LayerMask raycastMask;
        [BoxGroup("Raycast variables")] public float groundOffset;
        [BoxGroup("Raycast variables")] public float raycastOffset;


        [Space(10)]
        public WolfState currentState;
        public WolfMovementState movementState;

        [Space(10)] public float sppedMultiplier = 1;
        public float roationSmoothness = 0.2f;

        private WolfBaseState _currentMovementState;

        [SerializeField, Space(10)] private WolfGroundMovement groundMovement = new WolfGroundMovement();
        [SerializeField] private WolfWaterMovement waterMovement = new WolfWaterMovement();


        private float turnSmoothVelocity;
        private bool _canRotate = true;


        #region built in methods


        private void Start()
        {
            animator.speed = sppedMultiplier;
            _currentMovementState = groundMovement;
            _currentMovementState.EnterState(this);
        }

        private void Update()
        {
            _currentMovementState.UpdateState(this);
        }

        private void FixedUpdate()
        {
            _currentMovementState.FixedUpdateState(this);
        }

        private void LateUpdate()
        {
            _currentMovementState.LateUpdateState(this);
        }


        #endregion

        #region custom methods     

        public void ChangeMovementState(int state)
        {
            _currentMovementState.ExitState(this);
            switch (state)
            {
                case 0:
                    movementState = WolfMovementState.GrounMovement;
                    _currentMovementState = groundMovement;
                    break;
                case 1:
                    movementState = WolfMovementState.WaterMovement;
                    _currentMovementState = waterMovement;
                    break;
            }

            _currentMovementState.EnterState(this);
        }
        public void ActivateWolf()
        {
            animator.SetBool("Move", true);
            currentState = WolfState.Chasing;
        }
        public void Rotate(Vector3 lookTowards)
        {

            if (!_canRotate) return;


            var position = transform.position;

            var direction = lookTowards - position;


            //Rotate the player in desired direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, roationSmoothness);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public void PlayAnimation(string animation)
        {
            controller.enabled = false;
            _canRotate = false;
            animator.CrossFade(animation, 0.2f, 1);
        }

        #endregion

        #region callbacks

        public void OnAttack()
        {
            animator.ResetTrigger("Attack");
        }

        public void ActivateRotation()
        {
            controller.enabled = true;
            _canRotate = true;
        }

        #endregion



    }

    #region wolf movement states

    public abstract class WolfBaseState
    {

        public float attackingDistance = 1f;
        public float damageDistance = 1f;

        public abstract void EnterState(Wolf_V1 master);
        public abstract void UpdateState(Wolf_V1 master);
        public abstract void FixedUpdateState(Wolf_V1 master);

        public abstract void LateUpdateState(Wolf_V1 master);

        public abstract void ExitState(Wolf_V1 master);
    }

    [System.Serializable]
    public class WolfGroundMovement : WolfBaseState
    {

        private bool _isGrounded = true;

        #region Overriden mehtods

        #region unused methods
        public override void ExitState(Wolf_V1 master)
        {

        }
        public override void LateUpdateState(Wolf_V1 master)
        {

        }

        #endregion

        public override void EnterState(Wolf_V1 master)
        {
            master.animator.SetInteger("MovementState", 0);
        }

        public override void FixedUpdateState(Wolf_V1 master)
        {
            GroundCheck(master);
            master.animator.SetBool("isGrounded", _isGrounded);
        }

        public override void UpdateState(Wolf_V1 master)
        {
            switch (master.currentState)
            {
                case WolfState.Chasing:
                    Chase(master);
                    break;
                case WolfState.Feast:
                    Feast(master);
                    break;
                case WolfState.Hawl:
                    Hawl(master);
                    break;
            }
        }



        #endregion

        #region Custom methods

        private void GroundCheck(Wolf_V1 master)
        {
            var transform = master.transform;

            if (Physics.Raycast(transform.position + master.raycastOffset * transform.up, -transform.up, out RaycastHit hit, 5f, master.raycastMask))
            {
                //TODO: Remove debug
                Debug.DrawLine(transform.position + master.raycastOffset * transform.up, hit.point, Color.blue, 1f);
                _isGrounded = hit.distance < master.groundOffset + master.raycastOffset;
            }

            else
            {
                _isGrounded = false;
            }
        }

        private void Chase(Wolf_V1 master)
        {

            master.animator.SetFloat("Speed", 2, 0.5f, Time.deltaTime);

            Vector3 nextPathPoint;

            if (master.pathFinder.NextPathPoint(master.target, out nextPathPoint))
            {
                var targetPos = master.target.position;

                var distance = Vector3.Distance(master.transform.position, targetPos);

                if (distance < attackingDistance)
                {
                    master.animator.SetTrigger("Attack");

                    if (distance < damageDistance)
                    {
                        Player_Scripts.PlayerController.Instance.Player.Health.PlayerDamage("standing_death");
                        master.currentState = WolfState.Feast;
                    }
                }


                master.Rotate(nextPathPoint);
            }
            else
            {
                //Change    
                master.animator.SetBool("TargetBlocked", true);
                master.currentState = WolfState.Hawl;
            }


        }

        private void Feast(Wolf_V1 master)
        {

            var distance = Vector3.Distance(master.transform.position, master.target.position);


            if (distance > 0.5f)
            {
                master.Rotate(master.target.position);
                master.animator.SetBool("Feast", false);
                master.animator.SetFloat("Speed", 1, 0.3f, Time.deltaTime);
            }
            else
            {
                master.animator.SetBool("Feast", true);
            }

        }

        private void Hawl(Wolf_V1 master)
        {
            master.Rotate(master.target.transform.position);

            if (master.pathFinder.NextPathPoint(master.target, out var nextPathPoint))
            {
                master.animator.SetBool("TargetBlocked", false);
                master.currentState = WolfState.Chasing;
            }
        }


        #endregion


    }

    [System.Serializable]
    public class WolfWaterMovement : WolfBaseState
    {

        #region variables

        public float firstRaycastPos;
        public float secondRaycastPos;
        public float movementSmoothness = 4f;
        public Vector3 movementOffset;

        private bool _exit;
        private Vector3 moveTo;


        #endregion

        #region overriden methods

        #region Unused Methods
        public override void ExitState(Wolf_V1 master)
        {
        }

        #endregion

        public override void EnterState(Wolf_V1 master)
        {
            //Change Wolf Movement State
            master.animator.SetInteger("MovementState", 1);
        }

        public override void FixedUpdateState(Wolf_V1 master)
        {

            if (_exit)
            {

                var position = master.transform.position;
                master.transform.position = Vector3.MoveTowards(position, moveTo, Time.deltaTime * movementSmoothness);

                if (Vector3.Distance(position, moveTo) < 0.1f)
                {
                    master.controller.enabled = true;
                    _exit = false;
                    master.ChangeMovementState(0);
                }

            }
            else
            {
                CheckProximity(master);
            }

        }

        public override void LateUpdateState(Wolf_V1 master)
        {
            GroundCheck(master);
        }

        public override void UpdateState(Wolf_V1 master)
        {
            switch (master.currentState)
            {
                case WolfState.Chasing:
                    Chase(master);
                    break;
                case WolfState.Feast:
                    Feast(master);
                    break;
                case WolfState.Hawl:
                    Hawl(master);
                    break;
            }

        }

        #endregion

        #region Custom methods
        private void CheckProximity(Wolf_V1 master)
        {
            var transform = master.transform;
            var position = transform.position;
            var up = transform.up;
            var forward = transform.forward;
            var layermask = master.raycastMask;
            layermask = layermask & ~(1 << 4);


            //TODO: Remove debug

            #region Raycasts

            //FIRST forwardCast
            if (Physics.Raycast(position + up * firstRaycastPos, forward, out RaycastHit hit, 3f, layermask))
            {
                Debug.DrawLine(position + up * firstRaycastPos, hit.point, Color.blue, 1f);
            }
            else
            {
                Debug.DrawLine(position + up * firstRaycastPos, position + up * firstRaycastPos + forward * 3f, Color.yellow, 1f);
            }

            //Second Raycast
            if (Physics.Raycast(position + up * secondRaycastPos, forward, out RaycastHit hit1, 3f, layermask))
            {

                Debug.DrawLine(position + up * secondRaycastPos, hit1.point, Color.red, 1f);
            }
            else
            {
                Debug.DrawLine(position + up * secondRaycastPos, position + up * secondRaycastPos + forward * 3f, Color.green, 1f);
            }

            //Vertical raycast
            if (Physics.Raycast(position + up * (secondRaycastPos + 1) + forward, -up, out RaycastHit hit2, 3f, layermask))
            {

                Debug.DrawLine(position + up * (secondRaycastPos + 1) + forward, hit2.point, Color.red, 1f);
            }
            else
            {
                Debug.DrawLine(position + up * (secondRaycastPos + 1), position + up * (secondRaycastPos + 1) + forward - up * 3f, Color.green, 1f);
            }
            #endregion


            /** 
             * first raycast detects obstacle
             * second raycast detects nothing
             * vertical raycast to detect if there is solid surface to jump
             * Jump and exit to ground movement state if all conditions are satisfied
             * **/

            if (hit.distance != 0 && hit1.distance == 0 && hit2.distance != 0)
            {

                moveTo = position + hit.distance * forward;
                moveTo.y = hit2.point.y;
                moveTo += forward * movementOffset.x + up * movementOffset.y + transform.right * movementOffset.z;

                master.controller.enabled = false;
                master.animator.CrossFade("exit", 0.2f, 1);
                _exit = true;
            }


        }

        private void GroundCheck(Wolf_V1 master)
        {
            var transform = master.transform;

            //Check Water depth
            Physics.Raycast(transform.position + transform.up * master.raycastOffset, -transform.up, out RaycastHit hit1, Mathf.Infinity, master.raycastMask);

            //Check wolf depth
            Physics.Raycast(transform.position + transform.up * master.raycastOffset, transform.up, out RaycastHit hit2, Mathf.Infinity, master.raycastMask);


            //move wolf to certain depth if it is not already in that depth;
            if (hit2.distance > 0.7f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hit2.point - transform.up * 0.6f, Time.deltaTime * movementSmoothness / 2);
            }

        }

        private void Chase(Wolf_V1 master)
        {

            Vector3 nextPathPoint;

            if (master.pathFinder.NextPathPoint(master.target, out nextPathPoint))
            {
                master.animator.SetFloat("Speed", 1, 0.5f, Time.deltaTime);
                master.Rotate(nextPathPoint);

                var distance = Vector3.Distance(master.transform.position, master.target.position);

                if (distance < attackingDistance)
                {
                    master.animator.SetTrigger("Attack");

                    if (distance < damageDistance)
                    {
                        Player_Scripts.PlayerController.Instance.Player.Health.PlayerDamage("standing_death");
                        master.currentState = WolfState.Feast;
                    }
                }
            }
            else
            {
                master.animator.SetFloat("Speed", 0, 0.5f, Time.deltaTime);
            }

        }

        private void Feast(Wolf_V1 master)
        {
            master.animator.SetFloat("Speed", 0);
        }
        private void Hawl(Wolf_V1 master)
        {
            master.Rotate(master.target.transform.position);

            if (master.pathFinder.NextPathPoint(master.target, out var nextPathPoint))
            {
                master.animator.SetBool("Move", true);
                master.currentState = WolfState.Chasing;
            }

        }

        #endregion      

    }

    #endregion

    #region custom types

    [System.Serializable]
    public enum WolfState
    {
        Idle,
        Patrolling,
        Chasing,
        Feast,
        Hawl
    }


    [System.Serializable]
    public enum WolfMovementState
    {
        GrounMovement,
        WaterMovement
    }


    #endregion


}