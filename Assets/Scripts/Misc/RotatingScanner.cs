using NPC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class RotatingScanner : MonoBehaviour
    {

        [SerializeField, BoxGroup("References")] private AISense sensor;

        [SerializeField, BoxGroup("Movement Properties")] private float minimumAngle;
        [SerializeField, BoxGroup("Movement Properties")] private float maximumAngle;
        [SerializeField, BoxGroup("Movement Properties")] private float rotationSpeed;

        private float _currentSpeed;

        private void Start()
        {
            _currentSpeed = rotationSpeed;
        }
        private void Update()
        {

            if (sensor.targetFound)
            {
                this.transform.LookAt(sensor.target);
                return;
            }

            Vector3 currentRotation = this.transform.localRotation.eulerAngles;

            
            float yRotation = currentRotation.y is >= 0 and <= 180 ? currentRotation.y : currentRotation.y - 360;

            if (yRotation < minimumAngle)
            {
                _currentSpeed = rotationSpeed;
            }

            if (yRotation > maximumAngle)
            {
                _currentSpeed = -rotationSpeed;
            }



            this.transform.Rotate(this.transform.up, _currentSpeed * Time.deltaTime);



        }

    }
}
