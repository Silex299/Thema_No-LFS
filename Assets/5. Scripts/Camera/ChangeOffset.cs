using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace Thema_Camera
{
    public class ChangeOffset : MonoBehaviour
    {
        [SerializeField, BoxGroup] private CameraFollowInfo info;
        [SerializeField, BoxGroup] private bool preferTime = true;
        [SerializeField, BoxGroup, InfoBox("More Smoothing"), HideIf(nameof(preferTime))] private bool accelerateOnCourse = false;

        [FormerlySerializedAs("transitionTime")] [SerializeField, BoxGroup]
        private float transitionTimeOrSpeed;

#if UNITY_EDITOR


        [Button("Get Offset", ButtonSizes.Large)]
        public void GetOffset()
        {
            var cameraFollow = FindObjectOfType<CameraFollow>();

            if (cameraFollow != null)
            {
                var previewCamera = cameraFollow.GetComponent<Camera>();
                info = new CameraFollowInfo
                {
                    offset = cameraFollow.m_Offset,
                    rotation = previewCamera.transform.eulerAngles,
                    FOV = previewCamera.fieldOfView,
                    lenseShift = previewCamera.lensShift
                };

                if (cameraFollow.m_AudioListener)
                    info.audioListenerLocalPosition = cameraFollow.m_AudioListener.transform.localPosition;
            }
        }

#endif


        public void ChangeCameraOffset()
        {
            print("Changing offest");
            if (preferTime)
            {
                CameraFollow.Instance.ChangeOffset(info, transitionTimeOrSpeed);
            }
            else
            {
                CameraFollow.Instance.ChangeOffsetWithSpeed(info, transitionTimeOrSpeed, accelerateOnCourse);
            }
        }

        public void ChangeCameraOffset(float transitionTimeOverride)
        {
            print(gameObject.name);
            if (preferTime)
            {
                CameraFollow.Instance.ChangeOffset(info, transitionTimeOverride);
            }
            else
            {
                CameraFollow.Instance.ChangeOffsetWithSpeed(info, transitionTimeOverride,accelerateOnCourse);

            }
        }

        public void ChangeCameraOffsetInstantaneous()
        {
            CameraFollow.Instance.TransitionInstant(info);
        }
    }


    [System.Serializable]
    public struct CameraFollowInfo
    {
        public Vector3 offset;
        public Vector3 rotation;
        public Vector3 audioListenerLocalPosition;
        public float FOV;
        public Vector2 lenseShift;
    }
}