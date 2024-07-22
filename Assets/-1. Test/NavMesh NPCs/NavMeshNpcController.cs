using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using Unity.SharpZipLib;
using UnityEngine;
using UnityEngine.AI;


namespace NavMesh_NPCs
{
    public class NavMeshNpcController : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField]
        private Transform target;

        [BoxGroup("References")] public NavMeshAgent agent;
        [BoxGroup("References")] public Animator animator;
        
        
        [BoxGroup("GroundCheck")] public float sphereCastRadius;
        [BoxGroup("GroundCheck")] public float sphereCastOffset;
        [BoxGroup("GroundCheck")] public float groundOffset;
        [BoxGroup("GroundCheck")] public LayerMask layerMask;

        [BoxGroup("Movement")] public float velocityThreshold;
        [BoxGroup("Movement")] public float rotationSmoothness = 10f;

        [BoxGroup("Movement"), Space(10)] public float waitTime = 1f;
        [BoxGroup("Movement")] public float surveillanceThreshold;

        [TabGroup("State", "Surveillance")] public Vector3[] surveillancePoints;
        [TabGroup("State", "Chase")] public float attackDistance;

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
        
        private bool  _playedDead;
        private int _currentSurveillancePoint;
        private Coroutine _changeSurveillancePointCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Chase = Animator.StringToHash("Chase");
        private static readonly int AfterDeath = Animator.StringToHash("AfterDeath");
        private static readonly int Unreachable = Animator.StringToHash("Unreachable");
        private static readonly int Grounded = Animator.StringToHash("IsGrounded");


        public Transform Target
        {
            set
            {
                target = value;
                animator.SetBool(Chase, target);
            }
        }

        private void Start()
        {
            animator.SetBool(Chase, target);
            agent.updateRotation = false;
            agent.speed = velocityThreshold;
            agent.SetDestination(surveillancePoints[_currentSurveillancePoint]);

            PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
        }
        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;
        }

        public void Update()
        {
            
            var velocityFraction = agent.velocity.magnitude / velocityThreshold;
            animator.SetFloat(Speed, velocityFraction, 0.2f, Time.deltaTime);
            Rotate(agent.desiredVelocity);
            
            if (target)
            {
                agent.SetDestination(target.position);
                
                float realDistance = Vector3.Distance(transform.position, target.position);
                animator.SetBool(Attack, (realDistance < attackDistance && !_playedDead));
                
            }
            else
            {
                if (PlannerDistance(transform.position, surveillancePoints[_currentSurveillancePoint]) <
                    surveillanceThreshold)
                {
                    _changeSurveillancePointCoroutine ??= StartCoroutine(ChangeSurveillancePoint(waitTime));
                }
            }
            
            GroundCheck();
        }


        
        private void OnPlayerDeath()
        {
            if (target)
            {
                _playedDead = true;
                animator.SetBool(AfterDeath, true);
            }
            else
            {
                //Don't change anything
            }
        }
        
        private IEnumerator ChangeSurveillancePoint(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            _currentSurveillancePoint = (_currentSurveillancePoint + 1) % surveillancePoints.Length;
            agent.SetDestination(surveillancePoints[_currentSurveillancePoint]);
            _changeSurveillancePointCoroutine = null;
        }

        private static float PlannerDistance(Vector3 pos1, Vector3 pos2)
        {
            pos2.y = 0;
            pos1.y = 0;
            return Vector3.Distance(pos1, pos2);
        }

        private void Rotate(Vector3 desiredVelocity)
        {
            desiredVelocity.y = 0;
            
            if (desiredVelocity.magnitude > 0 )
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredVelocity),
                    Time.deltaTime * rotationSmoothness);
            }
            else if (target)
            {
                var direction = target.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    Time.deltaTime * rotationSmoothness);
            }
        }

        
        public void GroundCheck()
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
        
    }
}