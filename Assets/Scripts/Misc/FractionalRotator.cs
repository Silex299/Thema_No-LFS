using UnityEngine;
namespace Misc
{
    public class FractionalRotator : MonoBehaviour
    {

        [SerializeField] private Axis localRotationAxis;
        [SerializeField] private float minimumAngle;
        [SerializeField] private float maximumAngle;


        private void Update()
        {

        }

        public void FractionalRotation(float fraction)
        {
            float angle = (maximumAngle - minimumAngle) * fraction + minimumAngle;

            var rotation = transform.localEulerAngles;

            if (localRotationAxis == Axis.y)
            {
                transform.localEulerAngles = new Vector3(rotation.x, angle, rotation.z);
            }
            else if (localRotationAxis == Axis.x)
            {
                transform.localEulerAngles = new Vector3(angle, rotation.y, rotation.z);
            }
            else if (localRotationAxis == Axis.z)
            {
                transform.localEulerAngles = new Vector3(rotation.x, rotation.y, angle);
            }
        }

    }
    public enum Axis
    {
        x, y, z
    }
}

