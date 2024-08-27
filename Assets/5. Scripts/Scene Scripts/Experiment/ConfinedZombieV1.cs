using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace Scene_Scripts.Experiment
{
    public class ConfinedZombieV1 : MonoBehaviour
    {
        #region ExposeedVariables

        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public Transform[] waypoints;
        [FoldoutGroup("References")] public Transform target;

        [FoldoutGroup("Movement")] public float speed = 1.0f;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10f;
        [FoldoutGroup("Movement")] public float stopDistance = 1f;

        #endregion

        #region States

        private ConfinedZombieBase _currentState;
        private ZombieState _currentStateType;

        [SerializeField] private ConfinedZombieIdle idleState;
        private readonly ConfinedZombieFollow _followState = new ConfinedZombieFollow();
        private readonly ConfinedZombieScream _screamState = new ConfinedZombieScream();

        #endregion

        #region Getter & Setter
        public Transform Target
        {
            get=>target;
            set
            {
                target = value;
                ChangeState(ZombieState.Follow);
            }
        }

        #endregion

        #region Built-in Methods

        private void Start()
        {
            ChangeState(ZombieState.Follow);
        }

        private void Update()
        {
            _currentState.UpdateState(this);
        }

        #endregion

        #region Other Methods

        public void ChangeState(int stateIndex)
        {
            ChangeState((ZombieState)stateIndex);
        }

        public void ChangeState(ZombieState newState)
        {
            if (_currentStateType == newState)
            {
                return;
            }

            _currentStateType = newState;
            _currentState = newState switch
            {
                ZombieState.Idle => idleState,
                ZombieState.Follow => _followState,
                ZombieState.Scream => _screamState,
                _ => _currentState
            };
            _currentState.EnterState(this);
        }

        public void Rotate(Vector3 lookAt, float dir = 0)
        {
            Vector3 forward;
            if (dir == 0)
            {
                forward = transform.forward * 10;
            }
            else if (dir > 0)
            {
                forward = lookAt - transform.position;
            }
            else
            {
                
                forward = transform.position - lookAt;
            }
            
            forward.y = 0;
            //Rotate the zombie to look at the target, but only on the Y axis
            Quaternion desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        public Vector3 GetDesiredPos()
        {
            return ThemaVector.GetClosestPointToLine(waypoints[0].position, waypoints[1].position, target.position);
        }

        #endregion
    }
    
    #region Zombie States & Type

    public abstract class ConfinedZombieBase
    {
        public abstract void EnterState(ConfinedZombieV1 zombie);
        public abstract void UpdateState(ConfinedZombieV1 zombie);
    }

    [System.Serializable]
    public class ConfinedZombieIdle : ConfinedZombieBase
    {
        public string entryAnimation;

        public override void EnterState(ConfinedZombieV1 zombie)
        {
            zombie.animator.CrossFade(entryAnimation, 0.2f);
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
        }
    }

    public class ConfinedZombieFollow : ConfinedZombieBase
    {
        private static readonly int State = Animator.StringToHash("State");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void EnterState(ConfinedZombieV1 zombie)
        {
            if (!zombie.target)
            {
                zombie.ChangeState(ZombieState.Idle);
                return;
            }

            zombie.animator.SetInteger(State, 0);
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
            Vector3 desiredPos = zombie.GetDesiredPos();

            float plannerDistance = ThemaVector.PlannerDistance(desiredPos, zombie.transform.position);
            
            float speed  = 0;

            if (plannerDistance > zombie.stopDistance)
            {
                if (desiredPos.z > zombie.transform.position.z)
                {
                    speed = 1;
                }
                else
                {
                    speed = -1;
                }
            }
            
            zombie.Rotate(desiredPos, speed);
            zombie.animator.SetFloat(Speed, speed, 0.1f, Time.deltaTime);
        }
    }

    public class ConfinedZombieScream : ConfinedZombieBase
    {
        private static readonly int State = Animator.StringToHash("State");

        public override void EnterState(ConfinedZombieV1 zombie)
        {
            if (!zombie.target)
            {
                zombie.ChangeState(ZombieState.Idle);
                return;
            }

            zombie.animator.SetInteger(State, 2);
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
            zombie.Rotate(zombie.target.position);
        }
    }

    public enum ZombieState
    {
        Idle,
        Follow,
        Scream
    }

    #endregion
}