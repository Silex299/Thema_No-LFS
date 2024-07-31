using System;
using System.Collections;
using JetBrains.Annotations;
using Player_Scripts;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;
using UnityEngine.AI;
using Weapons.NPC_Weapon;


namespace NavMesh_NPCs
{
    public class NavMeshNpcController : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform target;

        [FoldoutGroup("References")] public NavMeshAgent agent;
        [FoldoutGroup("References")] public Animator animator;


        [FoldoutGroup("GroundCheck")] public bool checkGround;
        [FoldoutGroup("GroundCheck")] public float sphereCastRadius;
        [FoldoutGroup("GroundCheck")] public float sphereCastOffset;
        [FoldoutGroup("GroundCheck")] public float groundOffset;
        [FoldoutGroup("GroundCheck")] public LayerMask layerMask;

        [FoldoutGroup("Movement")] public float velocityThreshold;
        [FoldoutGroup("Movement")] public float rotationSmoothness = 10f;

        [TabGroup("State", "Surveillance")] public Vector3[] surveillancePoints;
        [TabGroup("State", "Chase")] public float attackDistance;
        [TabGroup("State", "Chase")] public WeaponBase weapon;


        [TabGroup("State", "Surveillance"), SerializeField]
        private NavMeshAgentServeillance surveillance = new NavMeshAgentServeillance();

        [TabGroup("State", "Chase"), SerializeField]
        private NavMeshAgentChase chase = new NavMeshAgentChase();

        [TabGroup("State", "AfterDeath"), SerializeField]
        private NavMeshAgentAfterDeath afterDeath = new NavMeshAgentAfterDeath();

        [SerializeField] private NavMeshAgentState _currentState;

        internal int currentSurveillancePoint;
        private Coroutine _changeSurveillancePointCoroutine;
        private Coroutine _changeSpeedCoroutine;
        private static readonly int Grounded = Animator.StringToHash("IsGrounded");
        private float _currentRotateSpeed = 0;


        public enum States
        {
            Serveillance,
            Chase,
            AfterDeath
        }

        #region Editor Specific

#if UNITY_EDITOR

        [Button("Get Positions", ButtonSizes.Large), GUIColor(1, 0.3f, 0.1f)]
        public void GetPositions(Transform[] transforms)
        {
            surveillancePoints = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                surveillancePoints[i] = transforms[i].position;
            }
        }

#endif

        #endregion


        public Transform Target
        {
            set
            {
                target = value;
                StateChange(target ? States.Chase : States.Serveillance);
            }
            get => target;
        }

        private void Start()
        {
            agent.updateRotation = false;
            _currentRotateSpeed = rotationSmoothness;

            StateChange(States.Serveillance);
            PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;
        }

        public void Update()
        {
            _currentState?.Update(this);
            if (checkGround) GroundCheck();
        }


        private void OnPlayerDeath()
        {
            print("DAAAAAAAAAAAAAA");
            PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;   
            if (!Target) return;

            StateChange(States.AfterDeath);
            if (weapon) weapon.ResetWeapon();
        }

        public States enumState;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Chase = Animator.StringToHash("Chase");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int AfterDeath = Animator.StringToHash("AfterDeath");


        public void StateChange(int index)
        {
            StateChange((States) index);
        }
        
        public void StateChange(States state)
        {
            enumState = state;

            _currentState?.Exit(this);

            _currentState = null;
            
            _currentState = state switch
            {
                States.Serveillance => surveillance,
                States.Chase => chase,
                States.AfterDeath => afterDeath,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            _currentState?.Entry(this);
        }

        public void ResetNpc()
        {

            if (_changeSpeedCoroutine != null)
            {
                StopCoroutine(_changeSpeedCoroutine);
                _changeSpeedCoroutine = null;
            }
            
            animator.SetBool(Chase, false);
            animator.SetBool(Attack, false);
            animator.SetBool(AfterDeath, false);
            animator.SetFloat(Speed, 0);

            animator.Play("Default", 0);
            animator.Play("Default", 1);

            target = null;
            
            StateChange(States.Serveillance);
        }
        
        public static float PlannerDistance(Vector3 pos1, Vector3 pos2)
        {
            pos2.y = 0;
            pos1.y = 0;
            return Vector3.Distance(pos1, pos2);
        }

        public void Rotate(Vector3 desiredVelocity)
        {
            desiredVelocity.y = 0;

            if (desiredVelocity.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredVelocity),
                    Time.deltaTime * _currentRotateSpeed);
            }
            else if (target)
            {
                var direction = target.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    Time.deltaTime * _currentRotateSpeed);
            }
        }

        private void GroundCheck()
        {
            Ray ray = new Ray(transform.position + Vector3.up * sphereCastOffset, Vector3.down);

            bool isGrounded = false;

            if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, 2f, layerMask))
            {
                isGrounded = hit.distance < groundOffset + sphereCastOffset;
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f, Color.red);
            }

            animator.SetBool(Grounded, isGrounded);
        }

        public void ChangeSpeed(float agentSpeed, float animatorSpeed, float rotationSpeed, float time)
        {
            _changeSpeedCoroutine ??=
                StartCoroutine(ChangeSpeedCoroutine(agentSpeed, animatorSpeed, rotationSpeed, time));
        }

        private IEnumerator ChangeSpeedCoroutine(float speed, float animatorSpeed, float rotationSpeed, float time)
        {
            float elapsedTime = 0;
            float startSpeed = agent.speed;
            float startAnimatorSpeed = animator.GetFloat(Speed);

            while (elapsedTime <= time)
            {
                agent.speed = Mathf.Lerp(startSpeed, speed, elapsedTime / time);
                animator.SetFloat(Speed, Mathf.Lerp(startAnimatorSpeed, animatorSpeed, elapsedTime / time));
                _currentRotateSpeed = Mathf.Lerp(_currentRotateSpeed, rotationSpeed * rotationSmoothness,
                    elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _changeSpeedCoroutine = null;
        }
    }
    
    public class NavMeshAgentState
    {
        public virtual void Entry(NavMeshNpcController controller)
        {
        }

        public virtual void Update(NavMeshNpcController controller)
        {
        }


        public virtual void Exit(NavMeshNpcController controller)
        {
        }
    }

    [Serializable]
    public class NavMeshAgentServeillance : NavMeshAgentState
    {
        public float stopDistance = 2f;
        public float waitTime = 2f;

        private Coroutine _changeSurveillancePointCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void Entry(NavMeshNpcController controller)
        {
            if (controller.surveillancePoints.Length == 0)
            {
                
                controller.ChangeSpeed(0, 0, 0, 1);
            }
            else
            {
                controller.currentSurveillancePoint = 0;
                controller.agent.SetDestination(controller.surveillancePoints[controller.currentSurveillancePoint]);
                controller.ChangeSpeed(controller.velocityThreshold, 1, 1, 1);
            }
        }

        public override void Update(NavMeshNpcController controller)
        {
            
            //do nothing if there is no serveillance points
            if (controller.surveillancePoints.Length == 0) return; //FIX : NOT RETURNING

            var transform = controller.transform;

            if (NavMeshNpcController.PlannerDistance(transform.position,
                    controller.surveillancePoints[controller.currentSurveillancePoint]) < stopDistance)
            {
                _changeSurveillancePointCoroutine ??= controller.StartCoroutine(ChangeSurveillancePoint(controller));
                controller.ChangeSpeed(0, 0, 0, 1f);
            }
            else
            {
                controller.ChangeSpeed(controller.velocityThreshold, 1, 1, 1f);
            }

            controller.Rotate(controller.agent.desiredVelocity);
        }

        public override void Exit(NavMeshNpcController controller)
        {
        }

        private IEnumerator ChangeSurveillancePoint(NavMeshNpcController controller)
        {
            yield return new WaitForSeconds(waitTime);
            controller.currentSurveillancePoint =
                (controller.currentSurveillancePoint + 1) % controller.surveillancePoints.Length;

            controller.agent.SetDestination(controller.surveillancePoints[controller.currentSurveillancePoint]);
            _changeSurveillancePointCoroutine = null;
        }
    }

    [Serializable]
    public class NavMeshAgentChase : NavMeshAgentState
    {
        public float stopDistance = 0;

        private static readonly int Chase = Animator.StringToHash("Chase");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void Entry(NavMeshNpcController controller)
        {
            if (controller.Target == null)
            {
                controller.StateChange(NavMeshNpcController.States.Serveillance);
                return;
            }

            controller.animator.SetBool(Chase, true);
            controller.ChangeSpeed(controller.velocityThreshold, 1, 1, 1f);
        }

        public override void Exit(NavMeshNpcController controller)
        {
            controller.animator.SetBool(Chase, false);
            controller.animator.SetBool(Attack, false);
            attack = false;
        }


        //REMOVE
        public bool attack;
        public bool unreachable;
        public float realDistance;
        public override void Update(NavMeshNpcController controller)
        {
            controller.agent.SetDestination(controller.Target.position);

            realDistance = Vector3.Distance(controller.transform.position, controller.Target.position);
            attack = realDistance < controller.attackDistance;
            unreachable = controller.agent.desiredVelocity.magnitude < 0.09 && controller.agent.pathStatus != NavMeshPathStatus.PathComplete;

            if (attack)
            {
                Debug.LogError("Fuck me");
            }
            
            controller.animator.SetBool(Attack, attack && !unreachable);
            
            if (attack && controller.weapon) controller.weapon.Fire();
            
            
            if (realDistance < stopDistance || unreachable)
            {
                controller.ChangeSpeed(0, 0, 0, 1f);
            }
            else
            {
                controller.ChangeSpeed(controller.velocityThreshold, 1, 1, 1f);
            }


            controller.Rotate(controller.agent.desiredVelocity);
        }
    }

    [Serializable]
    public class NavMeshAgentAfterDeath : NavMeshAgentState
    {
        public float stopDistance;
        public bool returnToServeillance;
        [ShowIf(nameof(returnToServeillance))] public float waitTime = 5f;

        private static readonly int AfterDeath = Animator.StringToHash("AfterDeath");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void Entry(NavMeshNpcController controller)
        {
            if (returnToServeillance)
            {
                controller.StartCoroutine(ReturnToServeillance(controller));
                return;
            }

            controller.animator.SetBool(AfterDeath, true);
        }


        public override void Exit(NavMeshNpcController controller)
        {
            controller.animator.SetBool(AfterDeath, false);
        }

        public override void Update(NavMeshNpcController controller)
        {
            //if(returnToServeillance) return;

            controller.agent.SetDestination(controller.Target.position);

            if (NavMeshNpcController.PlannerDistance(controller.transform.position, controller.Target.position) <
                stopDistance)
            {
                controller.ChangeSpeed(0, 0, 0, 1f);
            }
            else
            {
                controller.ChangeSpeed(controller.velocityThreshold, 1, 1, 1f);
            }

            controller.Rotate(controller.agent.desiredVelocity);
            //Do nothing
        }


        public IEnumerator ReturnToServeillance(NavMeshNpcController controller)
        {
            yield return new WaitForSeconds(waitTime);
            controller.StateChange(NavMeshNpcController.States.Serveillance);
        }
    }
}