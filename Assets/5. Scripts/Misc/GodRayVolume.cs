using System;
using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class GodRayVolume : MonoBehaviour
    {
        [SerializeField] private LayerMask rayCastMask;

        private Dictionary<string, Coroutine> _objectCoroutines = new Dictionary<string, Coroutine>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                Coroutine coroutine  = StartCoroutine(ActOnObject("Player_Main", other.gameObject));
                _objectCoroutines.Add("Player_Main", coroutine);
            }

            if (other.CompareTag("NPC"))
            {
                Coroutine coroutine = StartCoroutine(ActOnObject(other.name, other.gameObject));
                _objectCoroutines.Add(other.name, coroutine);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            try
            {
                print(other.tag);
                
                if (other.CompareTag("Player_Main"))
                {
                    StopCoroutine(_objectCoroutines["Player_Main"]);
                    _objectCoroutines.Remove("Player_Main");
                }

                if (other.CompareTag("NPC"))
                {
                    StopCoroutine(_objectCoroutines[other.name]);
                    _objectCoroutines.Remove(other.name);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }


        private IEnumerator ActOnObject(string objTag, GameObject obj)
        {
            if (objTag == "Player_Main")
            {
                while (true)
                {
                    Transform player = obj.transform;

                    var position = transform.position;
                    Vector3 direction = player.position - position + Vector3.up * 0.7f;
                    Ray ray = new Ray(position, direction);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCastMask))
                    {
                        if (hit.collider.CompareTag("Player_Main"))
                        {
                            Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                            
                            PlayerMovementController.Instance.DisablePlayerMovement(true);
                            PlayerMovementController.Instance.PlayAnimation("Float Death", 0.2f, 1);

                            yield break;
                        }
                        else
                        {
                            Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                        }
                    }

                    yield return null;
                }
            }
            else
            {
                while (true)
                {
                    Transform player = obj.transform;

                    var position = transform.position;
                    Vector3 direction = player.position - position + Vector3.up * 0.7f;
                    Ray ray = new Ray(position, direction);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCastMask))
                    {
                        if (hit.collider.CompareTag("NPC"))
                        {
                            Debug.DrawLine(transform.position, hit.point, Color.green, 1f);

                            if (obj.TryGetComponent<Animator>(out Animator animator))
                            {
                                animator.CrossFade("FloatDeath", 0.2f, 2);
                            }

                            if (obj.TryGetComponent<Guard>(out Guard guard))
                            {
                                guard.enabled = false;
                            }

                            yield break;
                        }
                        else
                        {
                            Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                        }
                    }

                    yield return null;
                }
            }
            
        }


        public void Activate()
        {
        }

        public void Explode()
        {
        }
    }
}