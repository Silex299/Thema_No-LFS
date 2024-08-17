using System.Collections;
using UnityEngine;

namespace Misc
{
    public class CinematicScreen : MonoBehaviour
    {
        public float screenAspect;
        public float transitionTime = 2;
        public new Camera camera;

        private Vector2 _updatedRect;
        private Vector2 _updatedScreenSize;

        // Start is called before the first frame update
        void Start()
        {
            // Set the desired aspect ratio (16:9 in this case).
            float targetAspect = screenAspect;

            // Determine the game window's current aspect ratio.
            float windowAspect = (float)Screen.width / (float)Screen.height;
            // Calculate the scale height that is needed to adjust.
            float scaleHeight = windowAspect / targetAspect;


            // If the scale height is less than 1, add letterboxing (horizontal black bars).
            if (scaleHeight < 1.0f)
            {
                _updatedScreenSize.x = 1.0f;
                _updatedScreenSize.y = scaleHeight;

                _updatedRect.x = 0;
                _updatedRect.y = (1.0f - scaleHeight) / 2.0f;

            }
            else // If the scale height is greater than 1, add pillarboxing (vertical black bars).
            {
                float scaleWidth = 1.0f / scaleHeight;

                // Set the updated screen size
                _updatedScreenSize.x = scaleWidth;
                _updatedScreenSize.y = 1.0f;

                // Set the updated rect
                _updatedRect.x = (1.0f - scaleWidth) / 2.0f;
                _updatedRect.y = 0;

            }
            
        }

        public void ChangeScreenSize()
        {
            camera.rect = new Rect(_updatedRect.x, _updatedRect.y, _updatedScreenSize.x, _updatedScreenSize.y);
        }

      
    }
}
