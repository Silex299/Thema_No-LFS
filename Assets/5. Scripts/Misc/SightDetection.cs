using System;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class SightDetection : MonoBehaviour
    {
        [SerializeField] private LayerMask rayCastMask;
        [SerializeField, Space(10)] private UnityEvent onPlayerInSight;
        [SerializeField] private UnityEvent onPlayerOutOfSight;

        private bool _inSight;
     

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main") && enabled)
            {
                
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

        private void Start()
        {
            PlayerMovementController.Instance.player.Health.onDeath += DisableSightDetection;
            PlayerMovementController.Instance.player.Health.onRevive += EnableSightDetection;
        }

        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= DisableSightDetection;
            PlayerMovementController.Instance.player.Health.onRevive -= EnableSightDetection;
        }


        public void EnableSightDetection()
        {
            enabled = true;
        }

        public void DisableSightDetection()
        {
            enabled = false;
            StopAllCoroutines();
        }
        
        
    }
}