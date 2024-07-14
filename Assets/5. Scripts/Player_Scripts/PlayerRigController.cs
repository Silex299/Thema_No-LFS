using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player_Scripts
{
    public class PlayerRigController : MonoBehaviour
    {
        public Rig lookLeft;
        public Rig lookRight;
        public float weightChangeSpeed = 10f;

        private Coroutine _directionCoroutine;
        private Coroutine _resetCoroutine;

        public void LookLeft(float value)
        {
            if (_directionCoroutine != null)
                StopCoroutine(_directionCoroutine);
            else
                _directionCoroutine = StartCoroutine(LookLeftCoroutine(value));
        }

        public void LookRight(float value)
        {
            if (_directionCoroutine != null)
                StopCoroutine(_directionCoroutine);
            else
                _directionCoroutine = StartCoroutine(LookRightCoroutine(value));
        }

        public void ResetAll()
        {
            _resetCoroutine ??= StartCoroutine(ResetAllCoroutine());
        }

        private IEnumerator LookLeftCoroutine(float value)
        {
            float rightWeight = lookRight.weight;

            if (rightWeight > 0)
            {
                while (rightWeight > 0)
                {
                    rightWeight -= Time.deltaTime * weightChangeSpeed;
                    lookRight.weight = Mathf.Clamp01(rightWeight);
                    yield return null;
                }
            }

            float leftWeight = lookLeft.weight;
            while (leftWeight < value)
            {
                leftWeight += Time.deltaTime * weightChangeSpeed;
                lookLeft.weight = Mathf.Clamp01(leftWeight);
                yield return null;
            }

            _directionCoroutine = null;
        }

        private IEnumerator LookRightCoroutine(float value)
        {
            float leftWeight = lookLeft.weight;

            if (leftWeight > 0)
            {
                while (leftWeight > 0)
                {
                    leftWeight -= Time.deltaTime * weightChangeSpeed;
                    lookLeft.weight = Mathf.Clamp01(leftWeight);
                    yield return null;
                }
            }

            float rightWeight = lookRight.weight;
            while (rightWeight < value)
            {
                rightWeight += Time.deltaTime * weightChangeSpeed;
                lookRight.weight = Mathf.Clamp01(rightWeight);
                yield return null;
            }

            _directionCoroutine = null;
        }


        private IEnumerator ResetAllCoroutine()
        {
            float leftWeight = lookLeft.weight;
            float rightWeight = lookRight.weight;

            if (leftWeight == 0 && rightWeight == 0)
            {
                _resetCoroutine = null;
                yield break;
            }
        
            while (leftWeight > 0 || rightWeight > 0)
            {
                leftWeight -= Time.deltaTime * weightChangeSpeed;
                rightWeight -= Time.deltaTime * weightChangeSpeed;
                lookLeft.weight = Mathf.Clamp01(leftWeight);
                lookRight.weight = Mathf.Clamp01(rightWeight);
                yield return null;
            }
        
            _resetCoroutine = null;
        }
    }
}