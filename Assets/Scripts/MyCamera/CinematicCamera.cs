using UnityEngine;

namespace MyCamera
{
    public class CinematicCamera : MonoBehaviour
    {
        private bool _focus;
        private Transform _target;
        
        public void AdjustToDefaultCamera(Transform transform1)
        {

            var myTransform = transform;
            myTransform.position = transform1.position;
            myTransform.rotation = transform1.rotation;
        }
        
        
    }
}
