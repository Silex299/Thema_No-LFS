using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    [System.Serializable]
    public class Mover : MonoBehaviour
    {
        [SerializeField, BoxGroup("Mover")] private List<Vector3> points;

        [BoxGroup("Mover"), Button("SetPoints")]
        private void SetPoints()
        {
            points.Add(transform.position);
        }

        
        [SerializeField, BoxGroup("Mover")] internal float speed;
        [SerializeField, BoxGroup("Mover")] private float distanceThreshold = 0.1f;
        [SerializeField, BoxGroup("Mover")] private float transitionTime = 1f;
        [SerializeField, BoxGroup("Mover")] private float waitTime = 1f;

        private int _nextPointIndex;
        private float _currentSpeed;
        Coroutine _changingSpeedCoroutine;

        public Mover(float speed = 1)
        {
            this.speed = speed;
            _currentSpeed = speed;
        }

        private void Update()
        {
            Move();
        }

        private IEnumerator ChangeSpeed(float setSpeed, Action action = null)
        {
            float timeElapsed = 0;
            float initialSpeed = _currentSpeed;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / transitionTime;
                _currentSpeed = Mathf.Lerp(initialSpeed, setSpeed, fraction);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
            
            action?.Invoke();
            _changingSpeedCoroutine = null;
        }

        /// <summary>
        /// Moves the object towards the next point in the list. If the object is close enough to the next point, it stops and sets the next point as the target. If the object is stopped and not close to the next point, it starts moving again.
        /// </summary>
        public void Move()
        {
            Vector3 targetPoint = points[_nextPointIndex];
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

            transform.position = Vector3.MoveTowards(transform.position, targetPoint, _currentSpeed * Time.deltaTime);

            if (distanceToTarget < distanceThreshold && _changingSpeedCoroutine == null)
            {
                _changingSpeedCoroutine = StartCoroutine(ChangeSpeed(0, () =>
                {
                    _nextPointIndex = (_nextPointIndex + 1) % points.Count;
                    _changingSpeedCoroutine = null;
                }));
            }
            else if (_currentSpeed == 0)
            {
                _changingSpeedCoroutine = StartCoroutine(ChangeSpeed(speed));
            }
        }
        

        /// <summary>
        /// Stops or starts the Mover based on the provided boolean value.
        /// </summary>
        /// <param name="stop">If true, the Mover will stop. If false, the Mover will start.</param>
        public void StopMover(bool stop)
        {
            float targetSpeed = stop ? 0 : speed;

            if (!enabled) enabled = true;

            if (_changingSpeedCoroutine != null)
            {
                StopCoroutine(_changingSpeedCoroutine);
            }
            _changingSpeedCoroutine = StartCoroutine(ChangeSpeed(targetSpeed));
        }
        
        public void StopMoverInstant(bool stop)
        {
            if (_changingSpeedCoroutine != null)
            {
                StopCoroutine(_changingSpeedCoroutine);
            }
            _currentSpeed = stop ? 0 : speed;
        }
    }
}