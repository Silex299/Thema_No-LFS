using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class CinematicScreen : MonoBehaviour
    {
        public float screenAspect;
        public new Camera camera;

        // Start is called before the first frame update
        void Start()
        {
            // Set the desired aspect ratio (16:9 in this case).
            float targetAspect = screenAspect;

            // Determine the game window's current aspect ratio.
            float windowAspect = (float)Screen.width / (float)Screen.height;

            // Calculate the scale height that is needed to adjust.
            float scaleHeight = windowAspect / targetAspect;


            // If the scale height is less than 1, add letter boxing (horizontal black bars).
            if (scaleHeight < 1.0f)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;

                camera.rect = rect;
            }
            else // If the scale height is greater than 1, add pillar boxing (vertical black bars).
            {
                float scaleWidth = 1.0f / scaleHeight;

                Rect rect = camera.rect;

                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }
        }
    }
}

