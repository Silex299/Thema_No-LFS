using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


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


        private GuardState currentState;
        [SerializeField, BoxGroup("States")] private SurveillanceState surveillanceState = new SurveillanceState();
        [SerializeField, BoxGroup("States")] private ChaseState chaseState = new ChaseState();




        private void Start()
        {
            ChangeState(currentGuardState);
            currentState.StateEnter(this);
        }

        private void Update()
        {
            currentState.StateUpdate(this);
        }

        public void ChangeState(GuardStateEnum newState)
        {

            currentState?.StateExit(this);
            currentGuardState = newState;

            switch (currentGuardState)
            {
                case GuardStateEnum.GUARD:
                    currentState = surveillanceState;
                    currentState.StateEnter(this);
                    break;
                case GuardStateEnum.CHASE:
                    currentState = chaseState;
                    currentState.StateEnter(this);
                    break;
            }
        }



        internal void Rotate(Vector3 rotateTowards)
        {
            Vector3 position = transform.position;
            rotateTowards.y = position.y;

            Vector3 newForward = rotateTowards - position;
            transform.forward = Vector3.Lerp(transform.forward, newForward, Time.deltaTime * rotationSpeed);
        }

    }

    [System.Serializable]
    public enum GuardStateEnum
    {
        GUARD,
        CHASE,
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
                if (_nexPointCoroutine == null)
                {
                    _nexPointCoroutine = guard.StartCoroutine(ChangeGuardPoint());
                }


                string idle = "Idle " + Random.Range(0, 2);

                if (_walk)
                {
                    guard.animator.CrossFade(idle, 0.5f, 0);
                    _walk = false;
                }

            }
            else
            {
                if (!_walk)
                {
                    guard.animator.CrossFade("Walk", 0.5f, 0);
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
        [SerializeField] private float _distance;

        [SerializeField] private float attackInterval;


        private float _lastAttackTime;
        private Transform target;
        private bool _playerDead;


        

        public override void StateEnter(Guard guard)
        {
            _playerDead = false;
            guard.animator.CrossFade("Chase", 0.2f);
            target = Player_Scripts.PlayerMovementController.Instance.transform;
            Player_Scripts.PlayerMovementController.Instance.player.Health.OnDeath += StopChasing;
        }

        public override void StateExit(Guard guard)
        {
        }

        public override void StateUpdate(Guard guard)
        {

            if (_playerDead)
            {
                return;
            }


            Vector3 targetPos = target.position;
            Vector3 guardPos = guard.transform.position;

            targetPos.y = guardPos.y;

            float distance = Vector3.Distance(targetPos, guardPos);
            _distance = distance;


            if (distance > chaseDistance)
            {
                guard.animator.SetFloat("Speed", 2, 0.2f, Time.deltaTime);
            }
            else if (distance < attackDistance)
            {
                Attack(guard);
            }

            if (distance < 0.7f)
            {
                guard.animator.SetFloat("Speed", 0, 0.2f, Time.deltaTime);
            }

            guard.Rotate(target.position);
        }


        private void Attack(Guard guard)
        {

            if (Time.time < _lastAttackTime + attackInterval) return;

            _lastAttackTime = Time.time;
            //TODO Change
            guard.animator.CrossFade("Attack", 0.3f, 1);
        }

        private void StopChasing()
        {
            _playerDead = true;
            Player_Scripts.PlayerMovementController.Instance.player.Health.OnDeath -= StopChasing;
        }

    }

}
