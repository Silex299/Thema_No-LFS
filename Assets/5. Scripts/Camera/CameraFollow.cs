using Sirenix.OdinInspector;
using UnityEngine;


namespace Thema_Camera
{

    public class CameraFollow : MonoBehaviour
    {

        [SerializeField] private float m_followSmoothness;
        [SerializeField] internal Transform followTarget;
        [SerializeField] private Vector3 m_Offset;
        [SerializeField] internal AudioListener m_AudioListener;
        [SerializeField] internal bool lookAtTarget;

        [SerializeField] internal Camera myCamera;


        private static CameraFollow instance;
        public static CameraFollow Instance
        {
            get => instance;
        }


        private bool _changeOffset;

        private CameraFollowInfo _initialCameraInfo;
        private CameraFollowInfo _cameraInfo;

        private float _transitionTime;
        private float _timeElapsed;


        [Button("GetOffset")]
        public void GetOffset()
        {
            if (followTarget == null) return;

            m_Offset = transform.position - followTarget.position;
        }


        private void Awake()
        {
            if (CameraFollow.Instance != null)
            {
                Destroy(this);
            }
            else
            {
                CameraFollow.instance = this;
            }

            _cameraInfo = new CameraFollowInfo()
            {
                offset = m_Offset,
                rotation = transform.rotation.eulerAngles,
                FOV = myCamera.fieldOfView
            };
        }

        private void Update()
        {
            if (!followTarget) return;

            FollowTarget();

            if (_changeOffset) { TransitionOffset(); }
        }

        private void FollowTarget()
        {
            var newPos = followTarget.position + m_Offset;

            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * m_followSmoothness);

            if (lookAtTarget)
            {
                transform.LookAt(followTarget.position);
            }
        }


        private void TransitionOffset()
        {

            _timeElapsed += Time.deltaTime;

            float fraction = _timeElapsed / _transitionTime;

            m_Offset = Vector3.Lerp(_initialCameraInfo.offset, _cameraInfo.offset, fraction);
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(_initialCameraInfo.rotation), Quaternion.Euler(_cameraInfo.rotation), fraction);
            myCamera.fieldOfView = Mathf.LerpAngle(_initialCameraInfo.FOV, _cameraInfo.FOV, fraction);
            myCamera.lensShift = Vector2.Lerp(_initialCameraInfo.lenseShift, _cameraInfo.lenseShift, fraction);

            if (fraction >= 1)
            {
                _changeOffset = false;
            }
        }

        public void TransitionInstant(CameraFollowInfo info)
        {

            if (m_AudioListener)
            {
                m_AudioListener.transform.localPosition = info.audioListenerLocalPosition;
            }

            _cameraInfo = info;
            m_Offset = info.offset;

            transform.position = followTarget.position + m_Offset;
            transform.rotation = Quaternion.Euler(info.rotation);
            myCamera.fieldOfView = info.FOV;
            myCamera.lensShift = info.lenseShift;

        }

        public void ChangeOffset(CameraFollowInfo info, float transitionTime)
        {


            _initialCameraInfo = _cameraInfo;
            _cameraInfo = info;

            if (m_AudioListener)
            {
                m_AudioListener.transform.localPosition = info.audioListenerLocalPosition;
            }
            _transitionTime = transitionTime;

            _timeElapsed = 0f;
            _changeOffset = true;
        }



    }

}
