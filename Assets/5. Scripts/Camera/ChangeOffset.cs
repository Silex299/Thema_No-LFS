using UnityEngine;
using Sirenix.OdinInspector;

namespace Thema_Camera
{

    public class ChangeOffset : MonoBehaviour
    {

        [SerializeField, BoxGroup] private CameraFollowInfo info;

        [SerializeField, BoxGroup] private float transitionTime;


#if UNITY_EDITOR

        [SerializeField, BoxGroup("Setup")] private Camera previewCamera;
        [SerializeField, BoxGroup("Setup")] private Transform previewTarget;

        [Button("Setup Offse", ButtonSizes.Large)]
        public void SetOffset()
        {
            var _offset = previewCamera.transform.position - previewTarget.position;
            var _rotation = previewCamera.transform.rotation.eulerAngles;
            var _FOV = previewCamera.fieldOfView;

            info = new CameraFollowInfo
            {
                offset = _offset,
                rotation = _rotation,
                FOV = _FOV
            };
        }

#endif


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                ChangeCameraOffset();
            }
        }

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
    }

}
