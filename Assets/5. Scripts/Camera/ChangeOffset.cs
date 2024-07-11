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

        [SerializeField, BoxGroup("Setup")] private Camera previewCamera;
        [SerializeField, BoxGroup("Setup")] private Transform previewTarget;

        [Button("Setup Offset", ButtonSizes.Large)]
        public void SetOffset()
        {
            var offset = previewCamera.transform.position - previewTarget.position;
            var rotation = previewCamera.transform.rotation.eulerAngles;
            var fov = previewCamera.fieldOfView;

            info = new CameraFollowInfo
            {
                offset = offset,
                rotation = rotation,
                FOV = fov
            };
        }

#endif
        

        public void ChangeCameraOffset()
        {
            CameraFollow.Instance.ChangeOffset(info, transitionTime);
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
