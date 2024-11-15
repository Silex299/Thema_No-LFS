using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

namespace Player_Scripts
{
    public class PlayerRigController : MonoBehaviour
    {
        [SerializeField] private Rig rig;
        [SerializeField] private Transform source;
        [SerializeField] private float transitionTime = 1f;
        [SerializeField] private float targetChangeSpeed = 5f;

        private Coroutine _currentCoroutine;
        private Transform _target;
        private bool _aim;

        private void LateUpdate()
        {
            if (_aim && _target)
            {
                source.position = _target.position;
            }
        }


        public void SetAim(Transform newTarget)
        {
            SetAim(newTarget, transitionTime);
        }
        public void SetAim(Transform newTarget, float timeToTransit)
        {
            _target = newTarget;
            _aim = true;
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = StartCoroutine(SmoothTransition(1f, timeToTransit));
        }


        public void ResetAim()
        {
            ResetAim(transitionTime);
        }
        
        public void ResetAim(float timeToTransit)
        {
            _aim = false;
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = StartCoroutine(SmoothTransition(0f, timeToTransit));
        }

        private IEnumerator SmoothTransition(float targetWeight, float timeToTransit)
        {
            float initialWeight = rig.weight;
            float elapsedTime = 0f;

            while (elapsedTime < timeToTransit)
            {
                elapsedTime += Time.deltaTime;
                rig.weight = Mathf.Lerp(initialWeight, targetWeight, elapsedTime / timeToTransit);
                yield return null;
            }

            rig.weight = targetWeight;
        }

        public void Reset()
        {
            rig.weight = 0;
            _target = null;
        }
    }
}