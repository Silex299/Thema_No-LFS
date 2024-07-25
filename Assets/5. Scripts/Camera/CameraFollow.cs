using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Thema_Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float m_followSmoothness;
        [SerializeField] internal Transform followTarget;
        [SerializeField] internal Vector3 m_Offset;
        [SerializeField] internal AudioListener m_AudioListener;
        [SerializeField] internal bool lookAtTarget;

        [SerializeField] internal Camera myCamera;
        private static CameraFollow instance;

        public static CameraFollow Instance
        {
            get => instance;
        }

        private Coroutine _transitionOffsetTrigger;

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
        }

        private void Update()
        {
            if (!followTarget) return;

            FollowTarget();
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


        public void TransitionInstant(CameraFollowInfo info)
        {
            if (m_AudioListener)
            {
                m_AudioListener.transform.localPosition = info.audioListenerLocalPosition;
            }

            m_Offset = info.offset;

            transform.position = followTarget.position + m_Offset;
            transform.rotation = Quaternion.Euler(info.rotation);
            myCamera.fieldOfView = info.FOV;
            myCamera.lensShift = info.lenseShift;
        }

        public void ChangeOffset(CameraFollowInfo info, float transitionTime)
        {
            if (_transitionOffsetTrigger != null)
                StopCoroutine(_transitionOffsetTrigger);

            _transitionOffsetTrigger = StartCoroutine(TransitionOffset(info, transitionTime));
        }


        private IEnumerator TransitionOffset(CameraFollowInfo info, float transitionTime)
        {
            float timeElapsed = 0;

            Vector3 initialOffset = m_Offset;
            Quaternion initialRot = transform.rotation;
            float initialFOV = myCamera.fieldOfView;
            Vector2 initialLensShift = myCamera.lensShift;
            
            Vector3 audioListenerPos = Vector3.zero;
            if (m_AudioListener)
            {
                audioListenerPos = m_AudioListener.transform.localPosition;
            }


            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;

                m_Offset = Vector3.Lerp(initialOffset, info.offset, timeElapsed / transitionTime);
                
                transform.rotation = Quaternion.Lerp(initialRot, Quaternion.Euler(info.rotation),
                    timeElapsed / transitionTime);
                
                myCamera.fieldOfView = Mathf.Lerp(initialFOV, info.FOV, timeElapsed / transitionTime);
                
                myCamera.lensShift = Vector2.Lerp(initialLensShift, info.lenseShift, timeElapsed / transitionTime);

                if (m_AudioListener)
                {
                    m_AudioListener.transform.localPosition = Vector3.Lerp(audioListenerPos,
                        info.audioListenerLocalPosition, timeElapsed / transitionTime);
                }

                yield return null;
            }
        }
    }
}