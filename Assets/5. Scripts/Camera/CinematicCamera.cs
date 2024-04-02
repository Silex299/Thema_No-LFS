using UnityEngine;


namespace Thema_Camera
{
    public class CinematicCamera : MonoBehaviour
    {

        [SerializeField] internal Transform target;
        [SerializeField] private float followSmoothness;
        [SerializeField] internal Camera cineCamera;
        [SerializeField] internal AudioListener audioListener;



        private Vector3 _offset;

        

        public void StartCamera(Transform _target)
        {
            target = _target;
            _offset = transform.position - target.position;
            
        }


        private void Update()
        {
            if (!target) return;
            var newPos = target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * followSmoothness);

        }

    }
}