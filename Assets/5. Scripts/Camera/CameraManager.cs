using Sirenix.OdinInspector;
using UnityEngine;


namespace Thema_Camera
{

    public class CameraManager : MonoBehaviour
    {

        [SerializeField, BoxGroup("Camera Info")] private CameraFollow mainCamera;
        [SerializeField, BoxGroup("Camera Info")] private CinematicCamera cinematicCamera;

        [SerializeField, BoxGroup("Camera Movement")] private float focusDuration;
        [BoxGroup("Camera Movement")] public Shake defaultShakeParameters;

        [SerializeField, BoxGroup("Camera Movement Effectors")] private float maximumThresold;
        [SerializeField, BoxGroup("Camera Movement Effectors")] private float minimumThresold;

        private bool _isMainCameraActive = true;
        [SerializeField] private bool _shake;
        private bool _focus;
        private float _startFOV;
        private float _targetFOV;
        private float _focusTimeElapsed;
        private Vector3 _shakeInitialPosition;
        private Quaternion _startRotation;

        private static CameraManager instance;

        public static CameraManager Instance
        {
            get
            {
                return instance;
            }
        }



        private void Awake()
        {
            if (CameraManager.Instance == null)
            {
                instance = this;
            }
            else if (CameraManager.Instance != this)
            {
                Destroy(this);
            }

        }


        public void SwitchToCinematic(Transform target)
        {
            //Make the transform of cinematic same as the main camera
            var cinematicCameraTransform = cinematicCamera.transform;
            var mainCameraTransform = mainCamera.transform;

            cinematicCameraTransform.position = mainCameraTransform.position;
            cinematicCameraTransform.rotation = mainCameraTransform.rotation;
            

            //Set settings Same
            cinematicCamera.cineCamera.fieldOfView = mainCamera.myCamera.fieldOfView;

            //activate cinematic camera
            //call start camera in cinematicCamera
            cinematicCamera.gameObject.SetActive(true);
            cinematicCamera.StartCamera(target);


            //Deactivate main camera
            cinematicCamera.audioListener.transform.position = mainCamera.m_AudioListener.transform.position;
            mainCamera.gameObject.SetActive(false);


            _isMainCameraActive = false;
        }

        public void SwitchToMainCamera()
        {
            //Activate main camera,
            mainCamera.gameObject.SetActive(true);

            //Deactivate cinematic Camera
            cinematicCamera.gameObject.SetActive(false);
            _isMainCameraActive = true;
        }

        public void FocusOnTarget(float fov)
        {
            _focus = true;
            if (_isMainCameraActive)
            {
                _startRotation = mainCamera.transform.rotation;
                _startFOV = mainCamera.myCamera.fieldOfView;
            }
            else
            {
                _startRotation = cinematicCamera.transform.rotation;
                _startFOV = cinematicCamera.cineCamera.fieldOfView;
            }
            _targetFOV = fov;
            _focusTimeElapsed = 0;
        }

        private void Focus()
        {
            _focusTimeElapsed += Time.deltaTime;
            float fraction = _focusTimeElapsed / focusDuration;


            if (_isMainCameraActive)
            {
                mainCamera.myCamera.fieldOfView = Mathf.Lerp(_startFOV, _targetFOV, fraction);
                var lookRotation = Quaternion.LookRotation((mainCamera.followTarget.position - mainCamera.transform.position), Vector3.up);
                mainCamera.transform.rotation = Quaternion.Lerp(_startRotation, lookRotation, fraction);
            }
            else
            {
                cinematicCamera.cineCamera.fieldOfView = Mathf.Lerp(_startFOV, _targetFOV, fraction);
                var lookRotation = Quaternion.LookRotation((cinematicCamera.target.position - cinematicCamera.transform.position), Vector3.up);
                cinematicCamera.transform.rotation = Quaternion.Slerp(_startRotation, lookRotation, fraction);
            }


            if (fraction >= 1)
            {
                _focus = false;
            }

        }


        public void StartShaking(bool shake)
        {
            _shake = shake;

            if (_shake)
            {
                _shakeInitialPosition = transform.position;

            }
        }
        public void ShakeCamera(Shake shakeParams)
        {

            if (_shakeInitialPosition == Vector3.zero)
            {
                _shakeInitialPosition = transform.position;
            }

            var shakeVelocity = Vector3.one * shakeParams.amplitude;

            // Calculate the amount to shake based on perlin noise
            float shakeAmountX = Mathf.PerlinNoise(Time.time * shakeParams.frequency, 0) * shakeParams.amplitude;
            float shakeAmountY = Mathf.PerlinNoise(0, Time.time * shakeParams.frequency) * shakeParams.amplitude;

            // Update the shake velocity
            shakeVelocity.x = Mathf.Lerp(shakeVelocity.x, shakeAmountX, Time.deltaTime * shakeParams.damping);
            shakeVelocity.y = Mathf.Lerp(shakeVelocity.y, shakeAmountY, Time.deltaTime * shakeParams.damping);

            // Apply the shake to the object's position
            transform.position = _shakeInitialPosition + shakeVelocity;
        }

        private void Update()
        {
            if (_focus) { Focus(); }
            if (_shake) { ShakeCamera(defaultShakeParameters); }
        }

        

        public Transform GetCurrentTarget(out bool canShake)
        {
            canShake = !_shake && !_focus;
            return _isMainCameraActive ? mainCamera.followTarget : cinematicCamera.target;
        }

        public void Reset()
        {
            _focus = false;
            _shake = false;
            SwitchToMainCamera();
        }


    }


    [System.Serializable]
    public struct Shake
    {
        public float amplitude;
        public float frequency;
        public float damping;
    }


}