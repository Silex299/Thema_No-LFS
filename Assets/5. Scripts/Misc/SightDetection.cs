using System;
using System.Collections;
using Player_Scripts;
using TMPro;
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

        private bool _inSight;
        private Coroutine _triggerCoroutine;
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (_triggerCoroutine != null)
                {
                    StopCoroutine(_triggerCoroutine);
                    _triggerCoroutine = null;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _triggerCoroutine ??= StartCoroutine(CheckForPlayer());
            }
        }

        private IEnumerator CheckForPlayer()
        {
            while (true)
            {
                print("Calling");
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
                            if (PlayerMovementController.Instance.player.Health.IsDead)
                            {
                                _triggerCoroutine = null;
                                yield break;
                            }
                            
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

                yield return null;
            }
            
        }

    }
}