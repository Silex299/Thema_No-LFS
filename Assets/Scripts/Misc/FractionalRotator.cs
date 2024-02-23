using UnityEngine;
namespace Misc
{
    public class FractionalRotator : MonoBehaviour
    {

        [SerializeField] private axis localRotationAxis;
        [SerializeField] private float minimumAngle;
        [SerializeField] private float maximumAngle;


        private void Update()
        {

        }

        public void FractionalRotation(float fraction)
        {
            float angle = (maximumAngle - minimumAngle) * fraction + minimumAngle;

            var rotation = transform.localEulerAngles;

            if (localRotationAxis == axis.y)
            {
                transform.localEulerAngles = new Vector3(rotation.x, angle, rotation.z);
            }
            else if (localRotationAxis == axis.x)
            {
                transform.localEulerAngles = new Vector3(angle, rotation.y, rotation.z);
            }
            else if (localRotationAxis == axis.z)
            {
                transform.localEulerAngles = new Vector3(rotation.x, rotation.y, angle);
            }
        }

    }
    public enum axis
    {
        x, y, z
    }
}

