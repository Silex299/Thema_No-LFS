using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class Guard : MonoBehaviour
    {
        [SerializeField] internal Animator animator;
        [SerializeField] internal float rotationSpeed;
        [SerializeField] private GuardStateEnum currentGuardState;

#if UNITY_EDITOR


        [Button("SetState", ButtonSizes.Large)]
        public void ChangeState()
        {
            ChangeState(currentGuardState);
        }

#endif

        private GuardState _currentState;
        [SerializeField, BoxGroup("States")] private SurveillanceState surveillanceState = new SurveillanceState();
        [SerializeField, BoxGroup("States")] private ChaseState chaseState = new ChaseState();


        private void Start()
        {
            ChangeState(currentGuardState);
            _currentState.StateEnter(this);
        }

        private void Update()
        {
            _currentState.StateUpdate(this);
        }

        public void StartChasing()
        {
            ChangeState(GuardStateEnum.Chase);
        }

        public void StopChasing()
        {
            if (currentGuardState == GuardStateEnum.Chase)
            {
                chaseState.StopChasing();
            }
        }
        
        public void ChangeState(GuardStateEnum newState)
        {
            _currentState?.StateExit(this);
            currentGuardState = newState;

            switch (currentGuardState)
            {
                case GuardStateEnum.Guard:
                    _currentState = surveillanceState;
                    _currentState.StateEnter(this);
                    break;
                case GuardStateEnum.Chase:
                    _currentState = chaseState;
                    _currentState.StateEnter(this);
                    break;
            }
        }

        internal void Rotate(Vector3 rotateTowards)
        {
            var transform1 = transform;

            Vector3 position = transform1.position;
            rotateTowards.y = position.y;

            Vector3 newForward = rotateTowards - position;
            transform.forward = Vector3.Lerp(transform1.forward, newForward, Time.deltaTime * rotationSpeed);
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
        [SerializeField] private Transform[] guardPoints;
        [SerializeField] private float stopDistance;
        [SerializeField] private float stopDelay;

        private int _currentGuardPoint;
        private Coroutine _nexPointCoroutine;
        private bool _walk;

        public override void StateEnter(Guard guard)
        {
            guard.animator.CrossFade("Walk", 0.4f, 0);
            _walk = true;
        }

        public override void StateExit(Guard guard)
        {
        }

        public override void StateUpdate(Guard guard)
        {
            Vector3 nextPoint = guardPoints[_currentGuardPoint].position;

            float distance = Vector3.Distance(guard.transform.position, nextPoint);


            if (distance < stopDistance)
            {
                _nexPointCoroutine ??= guard.StartCoroutine(ChangeGuardPoint());


                string idle = "Idle " + Random.Range(0, 2);

                if (_walk)
                {
                    guard.animator.CrossFade(idle, 0.3f, 0);
                    _walk = false;
                }
            }
            else
            {
                if (!_walk)
                {
                    guard.animator.CrossFade("Walk", 0.3f, 0);
                    _walk = true;
                }

                guard.Rotate(nextPoint);
            }
        }

        private IEnumerator ChangeGuardPoint()
        {
            yield return new WaitForSeconds(stopDelay);

            _currentGuardPoint = (_currentGuardPoint + 1) % guardPoints.Length;
            _nexPointCoroutine = null;
        }
    }


    [System.Serializable]
    public class ChaseState : GuardState
    {
        [SerializeField] private float chaseDistance;
        [SerializeField] private float attackDistance;

        [SerializeField] private float attackInterval;


        private float _lastAttackTime;
        private Transform _target;
        private bool _playerDead;
        private bool _stopChasing;
        private float _chasingSpeed;
        private static readonly int Speed = Animator.StringToHash("Speed");


        public override void StateEnter(Guard guard)
        {
            _playerDead = false;
            _stopChasing = false;
            guard.animator.CrossFade("Chase", 0.2f);
            _target = Player_Scripts.PlayerMovementController.Instance.transform;
            Player_Scripts.PlayerMovementController.Instance.player.Health.OnDeath += StopChasing;
        }

        public override void StateExit(Guard guard)
        {
        }

        public override void StateUpdate(Guard guard)
        {
            if (_stopChasing) return;

            if (_playerDead)
            {
                guard.StartCoroutine(StopChaseAnimationUpdate(guard));
                _stopChasing = true;
                return;
            }


            Vector3 targetPos = _target.position;
            Vector3 guardPos = guard.transform.position;

            targetPos.y = guardPos.y;

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

            guard.Rotate(_target.position);
        }


        private void Attack(Guard guard)
        {
            if (Time.time < _lastAttackTime + attackInterval) return;

            _lastAttackTime = Time.time;
            //TODO Change
            guard.animator.CrossFade("Attack", 0.3f, 1);
        }

        public void AttackCallback(Guard guard)
        {
            var distance = Vector3.Distance(guard.transform.position, _target.position);
            Debug.LogError(distance);
            if (distance < 1.7f)
            {
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(100f);
            }
        }

        internal void StopChasing()
        {
            _playerDead = true;
            Player_Scripts.PlayerMovementController.Instance.player.Health.OnDeath -= StopChasing;
        }

        private IEnumerator StopChaseAnimationUpdate(Guard guard)
        {
            yield return new WaitForSeconds(1f);
            guard.animator.CrossFade("Basic Idle", 1f, 0);
        }
    }
}