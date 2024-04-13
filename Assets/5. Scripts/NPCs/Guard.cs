using System.Collections;
using UnityEngine;


namespace NPCs
{
    public class Guard : MonoBehaviour
    {


        [SerializeField] private GuardState currentGuardState;

        [Space(10), SerializeField] private Animator animator;

        [Space(10), SerializeField] private Transform[] guardPoints;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float stopDelay;
        [SerializeField] private float stopDistance;

        private int _currentGuardPoint;
        private Coroutine _nexPointCoroutine;


        private static readonly int Speed = Animator.StringToHash("Speed");


        private void Update()
        {
            switch (currentGuardState)
            {
                case GuardState.GUARD:
                    Surveill();
                    break;
            }
        }

        private void Surveill()
        {

            Vector3 nextPoint = guardPoints[_currentGuardPoint].position;

            float distance = Vector3.Distance(transform.position, nextPoint);
            print(distance);
            if (distance < stopDistance)
            {
                if (_nexPointCoroutine == null)
                {
                    _nexPointCoroutine = StartCoroutine(ChangeGuardPoint());
                }


                float speed = (distance - 0.3f) / (stopDistance - 0.3f);

                animator.SetFloat(Speed, speed);
            }
            else
            {
                animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                Rotate(nextPoint);
            }

        }

        private void Chase()
        {

        }


        private void Attack()
        {

        }

        private IEnumerator ChangeGuardPoint()
        {

            yield return new WaitForSeconds(stopDelay);

            _currentGuardPoint = (_currentGuardPoint + 1) % guardPoints.Length;
            _nexPointCoroutine = null;

        }

        private void Rotate(Vector3 rotateTowards)
        {
            Vector3 position = transform.position;
            rotateTowards.y = position.y;

            Vector3 newForward = rotateTowards - position;
            transform.forward = Vector3.Lerp(transform.forward, newForward, Time.deltaTime * rotationSpeed);
        }

    }

    [System.Serializable]
    public enum GuardState
    {
        GUARD,
        CHASE,
        ATTACK
    }


}
