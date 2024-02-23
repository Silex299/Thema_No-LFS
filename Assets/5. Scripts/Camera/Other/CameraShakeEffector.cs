using UnityEngine;

namespace Thema_Camera
{

    public class CameraShakeEffector : MonoBehaviour
    {

        [SerializeField] private float minimumEffectingDistance;
        [SerializeField] private float maximumEffectingDistance;
        [SerializeField] private float shakeMultiplier = 1;

        private CameraManager _cameraManager;


        private void Start()
        {
            _cameraManager = CameraManager.Instance;
        }


        private void Update()
        {

            try
            {
                float distance = Vector3.Distance(transform.position, _cameraManager.GetCurrentTarget(out bool canShake).position);

                if (!canShake) return;

                if (distance < maximumEffectingDistance)
                {
                    var fraction = (maximumEffectingDistance - distance) / (maximumEffectingDistance - minimumEffectingDistance);

                    fraction = Mathf.Clamp01(fraction);

                    Shake shakeParams = _cameraManager.defaultShakeParameters;
                    shakeParams.amplitude *= fraction * shakeMultiplier;
                    shakeParams.damping *= fraction;

                    _cameraManager.ShakeCamera(shakeParams);
                }
            }
            catch
            {

            }

        }
    }

}