using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class SightDetection : MonoBehaviour
    {
        [SerializeField] private LayerMask rayCastMask;
        [SerializeField, Space(10)] private UnityEvent onPlayerInSight;
        [SerializeField] private UnityEvent onPlayerOutOfSight;

        private int _bodyCount;
        private bool _inSight;


        private void OnTriggerEnter(Collider other)
        {
            print(other.tag);
            if (other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _bodyCount++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            print("exit trigger");
            if (other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _bodyCount = Mathf.Clamp(_bodyCount - 1, 0, 99);
            }
        }

        private void Update()
        {
            if (_bodyCount <= 0) return;


            var position = transform.position;
            
            Vector3 direction = (PlayerMovementController.Instance.transform.position - position + Vector3.up * 0.8f );

            Ray ray = new Ray(position, direction);
            

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCastMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
                if (hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("Player"))
                {
                    if (!_inSight)
                    {
                        _inSight = true;
                        onPlayerInSight.Invoke();
                    }
                }
                else
                {
                    _inSight = false;
                    onPlayerOutOfSight.Invoke();
                }
            }
            else
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                if (_inSight)
                {
                    _inSight = false;
                    onPlayerOutOfSight.Invoke();
                }
            }
        }
    }
}