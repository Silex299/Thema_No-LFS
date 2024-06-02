using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;


namespace Thema_Camera
{

    public class TP_CameraFollow : MonoBehaviour
    {
        public Transform target; // The player the camera will follow
        public Vector3 offset; // The offset from the player
        public float mouseSensitivity = 100f; // Sensitivity of the mouse movement
        public float smoothSpeed = 0.125f; // Speed of the camera smoothing

        private float pitch = 0f; // Vertical rotation
        private float yaw = 0f; // Horizontal rotation

        private bool _canMove;

        IEnumerator Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            yaw = target.eulerAngles.y;
            pitch = target.eulerAngles.x;
            Vector3 initialPosition = target.position + offset;
            transform.position = initialPosition;
            // Lock the cursor to the center of the screen
            yield return new WaitForSeconds(1f);

            _canMove = true;
        }

        void LateUpdate()
        {

            if (!_canMove) return;

            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Update the yaw and pitch values
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -40f, 20f); // Clamp the pitch value to avoid flipping

            // Calculate the new camera position and rotation
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + rotation * offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Apply the position and rotation to the camera
            transform.position = smoothedPosition;
            transform.LookAt(target.position + Vector3.up * 2f); // Adjust the LookAt target as needed
        }

    }
}

