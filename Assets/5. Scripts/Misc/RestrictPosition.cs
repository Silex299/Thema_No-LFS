using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class RestrictPosition : MonoBehaviour
    {
        [Title("Restriction Settings")]
        [Tooltip("Enable or disable restriction on each axis.")]
        public bool restrictX;
        public bool restrictY;
        public bool restrictZ;

        [ShowIf("restrictX")]
        [MinMaxSlider(-100f, 100f, true)]
        public Vector2 xRange;

        [ShowIf("restrictY")]
        [MinMaxSlider(-100f, 100f, true)]
        public Vector2 yRange;

        [ShowIf("restrictZ")]
        [MinMaxSlider(-100f, 100f, true)]
        public Vector2 zRange;

        private void LateUpdate()
        {
            Vector3 position = transform.position;

            if (restrictX)
            {
                position.x = Mathf.Clamp(position.x, xRange.x, xRange.y);
            }

            if (restrictY)
            {
                position.y = Mathf.Clamp(position.y, yRange.x, yRange.y);
            }

            if (restrictZ)
            {
                position.z = Mathf.Clamp(position.z, zRange.x, zRange.y);
            }

            transform.position = position;
        }
    }
}