using System;
using System.Collections;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class SightDetection : MonoBehaviour
    {
        [SerializeField] private LayerMask rayCastMask;
        [SerializeField] private float playerHeight = 0.8f;
        [SerializeField, Space(10)] private UnityEvent onPlayerInSight;
        [SerializeField] private UnityEvent onPlayerOutOfSight;

        [SerializeField] private bool _enabled = true;
        private bool _inSight;
        private bool _playerInTrigger;
        private Coroutine _resetCoroutine;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main") && _enabled)
            {
                
                if (!_playerInTrigger)
                {
                    _playerInTrigger = true;
                }
                
                if (_resetCoroutine != null)
                {
                    StopCoroutine(_resetCoroutine);
                }
                _resetCoroutine = StartCoroutine(ResetTrigger());
            }
        }
        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.3f);
            _playerInTrigger = false;
            
            if (_inSight)
            {
                _inSight = false;
                onPlayerOutOfSight.Invoke();
            }
            _resetCoroutine = null;
        }


        private void Update()
        {
        
            
            if(!_playerInTrigger) return;
            
            var position = transform.position;
            Vector3 direction = (PlayerMovementController.Instance.transform.position - position + Vector3.up * playerHeight );
            Ray ray = new Ray(position, direction);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCastMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
                //Debug.Log(hit.collider.gameObject.name);
                
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
        
        private void OnEnable()
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
            _enabled = true;
            _inSight = false;
            _playerInTrigger = false;
        }

        public void DisableSightDetection()
        {
            if(_resetCoroutine!=null) StopCoroutine(_resetCoroutine);
            _resetCoroutine = null;
            _enabled = false;
            _playerInTrigger = false;
        }
        
        
    }
}