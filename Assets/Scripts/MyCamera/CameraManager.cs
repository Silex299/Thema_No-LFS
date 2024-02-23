using System;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyCamera
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        public Camera mainCamera;
        public CameraFollow followCamera;
        public CinematicCamera cinematicCamera;

        private void Awake()
        {
            if (!CameraManager.Instance)
            {
                CameraManager.Instance = this;
            }
            else if (CameraManager.Instance != this)
            {
                Destroy(this);
            }
        }

        public void SwitchToCinematicCamera()
        {
            if (PlayerController.Instance)
            {
                PlayerController.Instance.Player.CanMove = false;
                PlayerController.Instance.Player.canRotate = false;
                cinematicCamera.AdjustToDefaultCamera(followCamera.transform);
                mainCamera.gameObject.SetActive(false);
                cinematicCamera.gameObject.SetActive(true);
            }
        }

        public void SwitchToFollowPlayer()
        {
            if (PlayerController.Instance)
            {
                PlayerController.Instance.Player.CanMove = true;
                PlayerController.Instance.Player.canRotate = true;
                mainCamera.gameObject.SetActive(true);
                cinematicCamera.gameObject.SetActive(false);
            }
        }

        public void Focus(Transform t)
        {
            if (cinematicCamera.gameObject.activeInHierarchy)
            {
                var transform1 = followCamera.transform;
                var transform2 = cinematicCamera.transform;

                transform1.position = transform2.position;
                transform1.rotation = transform2.rotation;

                mainCamera.gameObject.SetActive(true);
                cinematicCamera.gameObject.SetActive(false);
            }

            followCamera.Focus(t, 0.2f);
        }


        #region Follow Camera Specific

        public void ChangeCameraFollowOffset(Vector3 newOffset, float smoothness = 2)
        {
            followCamera.ChangeOffset(newOffset, smoothness);
        }

        public void ChangeCameraFollowOffset(Vector3 newOffset, Vector3 rotation, float smoothness = 2)
        {
            followCamera.ChangeOffset(newOffset, rotation, smoothness);
        }

        #endregion

        public void ShakeCamera(bool status)
        {
            followCamera.ShakeCamera = status;
        }
        
        
        
    }
}