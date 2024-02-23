using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class RotatingObject : MonoBehaviour
    {


        [SerializeField, BoxGroup("Movement Variables")] private float rotatingSpeed;
        [SerializeField, BoxGroup("Movement Variables"), Space(5)]
        private axis rotationAxis;
        [SerializeField, BoxGroup("Movement Variables")] private Space rotationSpace;


        [SerializeField, BoxGroup("Misc"), InfoBox("If roation is limit. Use world space", InfoMessageType.Warning)]
        private bool limitRotation;
        [SerializeField, ShowIf("limitRotation"), BoxGroup("Misc")]
        private float minimumAngle;
        [SerializeField, ShowIf("limitRotation"), BoxGroup("Misc")]
        private float maximumAngle;

        public bool stopRotating;
        private float _currentSpeed;
        private void Update()
        {
            if (limitRotation && stopRotating) return;

            switch (rotationAxis)
            {
                case axis.x:
                    transform.Rotate(_currentSpeed * Time.deltaTime, 0, 0, rotationSpace);
                    break;
                case axis.y:
                    transform.Rotate(0, _currentSpeed * Time.deltaTime, 0, rotationSpace);
                    break;
                case axis.z:
                    transform.Rotate(0, 0, _currentSpeed * Time.deltaTime, rotationSpace);
                    break;
            }

            if (limitRotation)
            {
                var eularAngles = transform.eulerAngles;

                switch (rotationAxis)
                {
                    case axis.x:
                        if (eularAngles.x > 180)
                        {
                            eularAngles.x -= 360;
                        }

                        if (eularAngles.x > maximumAngle || eularAngles.x < minimumAngle)
                        {
                            stopRotating = true;
                        }
                        break;
                    case axis.y:

                        if (eularAngles.y > 180)
                        {
                            eularAngles.y -= 360;
                        }
                        if (eularAngles.y > maximumAngle || eularAngles.y < minimumAngle)
                        {
                            stopRotating = true;
                        }
                        break;
                    case axis.z:
                        if (eularAngles.z > 180)
                        {
                            eularAngles.z -= 360;
                        }
                        print(eularAngles.z);
                        if (eularAngles.z > maximumAngle || eularAngles.z < minimumAngle)
                        {
                            stopRotating = true;
                        }
                        break;
                }

                //TODO: Call Event

            }


            _currentSpeed = stopRotating ? Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * 3) : Mathf.Lerp(_currentSpeed, rotatingSpeed, Time.deltaTime * 3);
        }

        public void SetRotation(bool value)
        {
            stopRotating = value;
        }
    }
}
