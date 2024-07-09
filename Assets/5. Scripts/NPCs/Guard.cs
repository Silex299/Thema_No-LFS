using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Specialized;
using Player_Scripts;
using Triggers;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class Guard : MonoBehaviour
    {
        protected GuardState currentState;

        [SerializeField, BoxGroup("States")] private SurveillanceState surveillanceState = new SurveillanceState();

        [SerializeField, BoxGroup("States")] private ChaseState chaseState = new ChaseState();


        [SerializeField] internal Animator animator;
        [SerializeField] internal float rotationSpeed;
        [SerializeField] protected GuardStateEnum currentGuardState;

#if UNITY_EDITOR


        [Button("SetState", ButtonSizes.Large)]
        public void ChangeState()
        {
            ChangeState(currentGuardState);
        }

#endif

        protected virtual void Start()
        {
            ChangeState(currentGuardState);
            currentState.StateEnter(this);
        }

        private void Update()
        {
            currentState.StateUpdate(this);
        }

        public void StopChasing()
        {
            if (currentGuardState == GuardStateEnum.Chase)
            {
                chaseState.ImmediateStop(this);
            }
        }

        public virtual void ChangeState(GuardStateEnum newState)
        {
            
            currentState?.StateExit(this);
            currentGuardState = newState;

            switch (currentGuardState)
            {
                case GuardStateEnum.Guard:
                    currentState = surveillanceState;
                    currentState.StateEnter(this);
                    break;
                case GuardStateEnum.Chase:
                    currentState = chaseState;
                    currentState.StateEnter(this);
                    break;
            }
        }

        public void ChangeState(int index)
        {
            switch (index)
            {
                case 0:
                    ChangeState(GuardStateEnum.Guard);
                    break;
                case 1:
                    ChangeState(GuardStateEnum.Chase);
                    break;
            }
        }

        internal void Rotate(Vector3 rotateTowards, float overrideSpeed = 1)
        {
            var transform1 = transform;

            Vector3 position = transform1.position;
            rotateTowards.y = position.y;

            Vector3 newForward = rotateTowards - position;
            transform.forward = Vector3.Lerp(transform1.forward, newForward,
                Time.deltaTime * rotationSpeed * overrideSpeed);
        }

        public void AttackCallback()
        {
            if (currentGuardState == GuardStateEnum.Chase)
            {
                chaseState.AttackCallback(this);
            }
        }
    }

    [System.Serializable]
    public enum GuardStateEnum
    {
        Guard,
        Chase,
    }


    public abstract class GuardState
    {
        public abstract void StateEnter(Guard guard);
        public abstract void StateExit(Guard guard);
        public abstract void StateUpdate(Guard guard);
    }


    [System.Serializable]
    public class SurveillanceState : GuardState
    {
        [SerializeField] protected Transform[] guardPoints;
        [SerializeField] protected float stopDistance;
        [SerializeField] protected float stopDelay;

        protected int currentGuardPoint;
        protected Coroutine nexPointCoroutine;
        protected bool walk;

        public override void StateEnter(Guard guard)
        {
            guard.animator.CrossFade("Walk", 0.4f, 0);
            walk = true;
        }

        public override void StateExit(Guard guard)
        {
        }

        public override void StateUpdate(Guard guard)
        {
            Vector3 nextPoint = guardPoints[currentGuardPoint].position;

            float distance = Vector3.Distance(guard.transform.position, nextPoint);


            if (distance < stopDistance)
            {
                nexPointCoroutine ??= guard.StartCoroutine(ChangeGuardPoint());


                string idle = "Idle " + Random.Range(0, 2);

                if (walk)
                {
                    guard.animator.CrossFade(idle, 0.3f, 0);
                    walk = false;
                }
            }
            else
            {
                if (!walk)
                {
                    guard.animator.CrossFade("Walk", 0.3f, 0);
                    walk = true;
                }

                guard.Rotate(nextPoint);
            }
        }

        protected IEnumerator ChangeGuardPoint()
        {
            yield return new WaitForSeconds(stopDelay);

            currentGuardPoint = (currentGuardPoint + 1) % guardPoints.Length;
            nexPointCoroutine = null;
        }
    }


    [System.Serializable]
    public class ChaseState : GuardState
    {
        [SerializeField] protected float chaseDistance;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected float attackInterval;

        [SerializeField] protected bool advancedPathFinding;

        [SerializeField, ShowIf(nameof(advancedPathFinding))]
        protected Transform[] pathFindingPoints;

        [SerializeField, ShowIf(nameof(advancedPathFinding))]
        protected LayerMask rayCastMask;


        protected float lastAttackTime;
        protected Transform target;
        protected bool playerDead;
        protected bool stopChasing;
        private float _chasingSpeed;
        protected static readonly int Speed = Animator.StringToHash("Speed");


        public override void StateEnter(Guard guard)
        {
            playerDead = false;
            stopChasing = false;
            
            guard.animator.CrossFade("Chase", 0.2f);
            target = PlayerMovementController.Instance.transform;
            PlayerMovementController.Instance.player.Health.onDeath += StopChasing;
        }

        
        
        public override void StateExit(Guard guard)
        {
        }

        public override void StateUpdate(Guard guard)
        {
            if (stopChasing) return;

            if (playerDead)
            {
                guard.StartCoroutine(StopChaseAnimationUpdate(guard));
                stopChasing = true;
                return;
            }


            Vector3 targetPos = target.position;
            Vector3 guardPos = guard.transform.position;

            targetPos.y = guardPos.y;

            if (advancedPathFinding)
            {
                if (!AdvancedPathFinding(guard))
                {
                    ChaseAction(guard, targetPos, guardPos);
                }
                else
                {
                    guard.animator.SetFloat(Speed, _chasingSpeed, 0.2f, Time.deltaTime);
                }
            }
            else
            {
                ChaseAction(guard, targetPos, guardPos);
            }

            guard.Rotate(targetPos);
        }

        protected virtual void ChaseAction(Guard guard, Vector3 targetPos, Vector3 guardPos)
        {
            float distance = Vector3.Distance(targetPos, guardPos);

            if (distance > chaseDistance)
            {
                _chasingSpeed = 2;
            }
            else if (distance < chaseDistance && distance > 1f)
            {
                if (_chasingSpeed < 1)
                {
                    _chasingSpeed = 1;
                }
            }
            else if (distance < 1f)
            {
                _chasingSpeed = 0;
            }


            guard.animator.SetFloat(Speed, _chasingSpeed, 0.2f, Time.deltaTime);


            if (distance < attackDistance)
            {
                Attack(guard);
            }
        }


        protected virtual void Attack(Guard guard)
        {
            if (Time.time < lastAttackTime + attackInterval) return;

            lastAttackTime = Time.time;
            //TODO Change
            guard.animator.CrossFade("Attack", 0.3f, 1);
        }

        public void AttackCallback(Guard guard)
        {
            var distance = Vector3.Distance(guard.transform.position, target.position);
            if (distance < 1.7f)
            {
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(100f);
            }
        }

        public virtual void ImmediateStop(Guard guard)
        {
            guard.animator.CrossFade("Basic Idle", 0.6f, 0);
            stopChasing = true;
        }

        private void StopChasing()
        {
            playerDead = true;
            PlayerMovementController.Instance.player.Health.onDeath -= StopChasing;
        }

        protected IEnumerator StopChaseAnimationUpdate(Guard guard)
        {
            yield return new WaitForSeconds(1f);
            ImmediateStop(guard);
        }


        protected virtual bool AdvancedPathFinding(Guard guard)
        {
            if (!InSight((guard.transform.position + Vector3.up * 0.75f), (target.position + Vector3.up)))
            {
                Debug.DrawLine(guard.transform.position + Vector3.up * 0.75f, target.position + Vector3.up, Color.cyan,
                    10f);

                foreach (Transform point in pathFindingPoints)
                {
                    Debug.DrawLine(guard.transform.position + Vector3.up, point.position, Color.red, 10f);
                    if (InSight((guard.transform.position + Vector3.up), point.position))
                    {
                        //Debug draw a line from guard position to point position

                        if (InSight(target.position + Vector3.up, point.position))
                        {
                            Debug.DrawLine(target.position + Vector3.up, point.position, Color.green, 2f);
                            target = point;
                            return true;
                        }
                    }
                }
            }

            target = PlayerMovementController.Instance.transform;

            return false;
        }

        protected virtual bool InSight(Vector3 obj1, Vector3 obj2)
        {
            return !Physics.Linecast(obj1, obj2, rayCastMask);
        }
    }
}