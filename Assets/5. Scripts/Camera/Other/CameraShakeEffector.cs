using System;
using System.Collections;
using UnityEngine;

namespace Thema_Camera
{

    public class CameraShakeEffector : MonoBehaviour
    {

        [SerializeField] private float minimumEffectingDistance;
        [SerializeField] private float maximumEffectingDistance;
        [SerializeField] private float shakeMultiplier = 1;

        private CameraManager _cameraManager;
        private Coroutine _changeCameraShakeCoroutine;


        #region Editor
#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            DrawCircle(transform.position, minimumEffectingDistance, 32);
            Gizmos.color = Color.green;
            DrawCircle(transform.position, maximumEffectingDistance, 32);
        }
        
        private void DrawCircle(Vector3 center, float radius, int segments)
        {
            float angle = 2 * Mathf.PI / segments;
            Vector3 lastPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float x = Mathf.Cos(i * angle) * radius;
                float z = Mathf.Sin(i * angle) * radius;
                Vector3 newPoint = center + new Vector3(x, 0, z);
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }
        
#endif
        #endregion

        private void Start()
        {
            _cameraManager = CameraManager.Instance;
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, _cameraManager.GetCurrentTarget(out bool canShake).position);
            
            if (!canShake || Mathf.Approximately(shakeMultiplier, 0)) return;
            
            if (!(distance < maximumEffectingDistance)) return;
            
            var fraction = (maximumEffectingDistance - distance) / (maximumEffectingDistance - minimumEffectingDistance);
            fraction = Mathf.Clamp01(fraction);

            Shake shakeParams = _cameraManager.defaultShakeParameters;
            shakeParams.amplitude *= fraction * shakeMultiplier;
            shakeParams.damping *= fraction;

            _cameraManager.ShakeCamera(shakeParams);
        }

        public void TransitionCameraShake(float targetMultiplier, float time)
        {
            
            if(_changeCameraShakeCoroutine!=null) StopCoroutine(_changeCameraShakeCoroutine);
            _changeCameraShakeCoroutine = StartCoroutine(ChangeCameraShake(targetMultiplier, time));
            
        }

        private IEnumerator ChangeCameraShake(float targetMultiplier, float time)
        {
            float start = shakeMultiplier;
            float end = targetMultiplier;
            
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / time;
                shakeMultiplier = Mathf.Lerp(start, end, t);
                yield return null;
            }
        }
    }

}