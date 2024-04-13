using System.Collections;
using UnityEngine;


namespace NPCs
{
    public class Guard : MonoBehaviour
    {


        [SerializeField] private GuardState currentGuardState;

        [Space(10), SerializeField] internal Animator animator;

        [Space(10), SerializeField] internal Transform[] guardPoints;
        [SerializeField] internal Transform attackPoint;
        [SerializeField] internal float rotationSpeed;
        [SerializeField] internal float stopDelay;

        [SerializeField] internal float stopDistance;
        [SerializeField] internal float chaseDistance;
        [SerializeField] internal float attackDistance;

        [SerializeField, Space(10)] private Transform target;

        private int _currentGuardPoint;
        private Coroutine _nexPointCoroutine;
        internal bool _walk;




        private void Update()
        {
            switch (currentGuardState)
            {
                case GuardState.GUARD:
                    Surveill();
                    break;
                case GuardState.CHASE:
                    Chase();
                    break;
                case GuardState.ATTACK:
                    Attack();
                    break;
            }
        }


        private void Surveill()
        {


        }

        private void Chase()
        {
            if (!target)
            {
                currentGuardState = GuardState.GUARD;
                return;
            }


            float distance = Vector3.Distance(target.position, transform.position);


            _walk = distance > chaseDistance;
            animator.SetBool("FastChase", _walk);

            if (distance < attackDistance)
            {
                currentGuardState = GuardStateEnum.ATTACK;
            }

            Rotate(target.position);
            //Follow the target, 

            //If Targets not visible move to the chase point?

        }


        private void Attack()
        {

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
        ATTACK
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
            guard._walk = true;

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

}
