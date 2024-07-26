using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;

namespace Thema_Camera
{

    public class ChangeOffset : MonoBehaviour
    {

        [SerializeField, BoxGroup] private CameraFollowInfo info;
        [SerializeField, BoxGroup] private float transitionTime;
        
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
                
                if(cameraFollow.m_AudioListener)
                    info.audioListenerLocalPosition = cameraFollow.m_AudioListener.transform.localPosition;
            }
        }

#endif
        

        public void ChangeCameraOffset()
        {
            CameraFollow.Instance.ChangeOffset(info, transitionTime);
        }
        
        public void ChangeCameraOffset(float transitionTimeOverride)
        {
            print(gameObject.name);
            CameraFollow.Instance.ChangeOffset(info, transitionTimeOverride);
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
