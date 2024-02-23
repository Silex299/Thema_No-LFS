using System;
using MyCamera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactions
{
    public class ChangeCameraOffset : MonoBehaviour
    {
        [SerializeField] private float smoothness;
        [SerializeField] private Vector3 newOffset;

        [SerializeField, Space(10)] private bool changeRotation;
        [SerializeField, ShowIf("changeRotation")] private Vector3 newRotation;


        public Vector3 PositionOffset
        {
            get => newOffset;
            set => newOffset = value;
        }

        public Vector3 RotationOffset
        {
            get => newRotation;
            set => newRotation = value;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                ChangeOffset();
            }
        }

        public void ChangeOffset()
        {
            if (newRotation == Vector3.zero || !changeRotation)
            {
                CameraManager.Instance.ChangeCameraFollowOffset(newOffset, smoothness);
                return;
            }
            CameraManager.Instance.ChangeCameraFollowOffset(newOffset, newRotation, smoothness);
        }
        
        
        
#if UNITY_EDITOR

        public Vector3 Offset()
        {
            return newOffset;
        }

        public void SetOffset(Vector3 offset)
        {
            newOffset = offset;
        }
        
#endif 
        
    }
}
