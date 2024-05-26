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

        public IEnumerator ChangeSpeed(float setSpeed, Action action = null)
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

            _changingSpeedCoroutine = null;
            action?.Invoke();
        }

        public void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, points[_nextPointIndex],
                _currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, points[_nextPointIndex]) < distanceThreshold)
            {
                if (_changingSpeedCoroutine == null)
                {
                    _changingSpeedCoroutine = StartCoroutine(ChangeSpeed(0, () =>
                    {
                        //set next point
                        _nextPointIndex = (_nextPointIndex + 1) % points.Count;
                        _changingSpeedCoroutine = null;
                    }));
                }
                
            }
            else if (_currentSpeed == 0)
            {
                _changingSpeedCoroutine = StartCoroutine(ChangeSpeed(speed));
            }
            
        }
        
        
    }
}