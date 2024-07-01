using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Camera;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Guard = NPCs.Guard;

// ReSharper disable once CheckNamespace
namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class GodRayVolume : MonoBehaviour
    {
        [SerializeField] private bool isEnabled;
        [SerializeField] private LayerMask rayCastMask;
        [SerializeField] private float destroyTimer;


        [SerializeField, TabGroup("Visual", "Visual")]
        private float turnUpTime = 1f;

        [FormerlySerializedAs("light")] [SerializeField, TabGroup("Visual", "Visual")]
        private Light godLight;

        [SerializeField, TabGroup("Visual", "Visual")]
        private float powerUpIntensity;

        [SerializeField, TabGroup("Visual", "Sound")]
        private AudioSource source;

        [SerializeField, TabGroup("Visual", "Sound")]
        private AudioClip startSound;

        [SerializeField, TabGroup("Visual", "Sound")]
        private AudioClip stopSound;
        
        [SerializeField, TabGroup("Visual", "Sound")]
        private AudioClip machineSound;

        [SerializeField, TabGroup("Visual", "Sound")]
        private AudioClip explodeSound;


        [SerializeField, BoxGroup("Events")] private UnityEvent onTurningOn;
        [SerializeField, BoxGroup("Events")] private UnityEvent onTurningOff;
        [SerializeField, BoxGroup("Events")] private UnityEvent onExplode;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Dictionary<string, Coroutine> _objectCoroutines = new Dictionary<string, Coroutine>();
        private float _turnOnTime;
        private bool _destroyed;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isEnabled) return;

            TriggerEntry(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (isEnabled)
            {
                TriggerEntry(other);
            }
            else
            {
                if (_objectCoroutines.Count == 0)
                {
                    return;
                }

                if (!_destroyed)
                {
                    TriggerExit(other);
                }
                else
                {
                    TriggerDeath(other);
                }
                
            }

            
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isEnabled) return;

            TriggerExit(other);
        }


        private void TriggerEntry(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (!_objectCoroutines.TryGetValue("Player_Main", out Coroutine routine))
                {
                    Coroutine coroutine = StartCoroutine(ActOnObject("Player_Main", other.gameObject));
                    _objectCoroutines.Add("Player_Main", coroutine);
                }
                
            }
            else if (other.CompareTag("NPC"))
            {
                if (!_objectCoroutines.TryGetValue(other.name, out Coroutine routine))
                {
                    Coroutine coroutine = StartCoroutine(ActOnObject(other.name, other.gameObject));
                    _objectCoroutines.Add(other.name, coroutine);
                }
            }
        }

        private void TriggerExit(Collider other)
        {
            
            if (other.CompareTag("Player_Main"))
            {
                if (_objectCoroutines.TryGetValue("Player_Main", out Coroutine coroutine))
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    _objectCoroutines.Remove("Player_Main");
                }
            }

            if (other.CompareTag("NPC"))
            {
                if (_objectCoroutines.TryGetValue(other.name, out Coroutine coroutine))
                {
                    if (coroutine!=null)
                    {
                        StopCoroutine(coroutine);
                    }
                    var obj = other.gameObject;
                    
                    if (obj.TryGetComponent<Animator>(out Animator animator))
                    {
                        animator.CrossFade("Default", 0.01f, 2);
                    }

                    if (obj.TryGetComponent<NPCs.Guard>(out NPCs.Guard guard))
                    {
                        guard.enabled = true;
                        guard.ChangeState(1);
                    }
                    
                    _objectCoroutines.Remove(other.name);
                }
                    
            }
        }

        private void TriggerDeath(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (_objectCoroutines.TryGetValue("Death", out Coroutine coroutine))
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    PlayerMovementController.Instance.player.Health.TakeDamage(101);
                    _objectCoroutines.Remove("Player_Main");
                }
            }

            if (other.CompareTag("NPC"))
            {
                if (_objectCoroutines.TryGetValue(other.name, out Coroutine coroutine))
                {
                    if (coroutine!=null)
                    {
                        StopCoroutine(coroutine);
                    }
                    var obj = other.gameObject;
                    
                    if (obj.TryGetComponent<Animator>(out Animator animator))
                    {
                        animator.CrossFade("Death", 0.01f, 2);
                    }
                    
                    _objectCoroutines.Remove(other.name);
                }
                    
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
                                animator.CrossFade("FloatDeath", 0.5f, 2);
                            }

                            if (obj.TryGetComponent<NPCs.Guard>(out NPCs.Guard guard))
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


        private void Update()
        {
            if (!isEnabled) return;

            if (Time.time > _turnOnTime + destroyTimer)
            {
                SelfDestruct();
            }
        }

        public void TurnOn(bool turnOn)
        {
            StartCoroutine(turnOn ? TurnOnVolume() : TurnOffVolume());
        }
        private IEnumerator TurnOnVolume()
        {
            CameraManager.Instance.StartShaking(true);
            //Play Sound;
            if (source)
            {
                source.PlayOneShot(startSound);
            }
            
            onTurningOn.Invoke();
            isEnabled = true;
            float timeElapsed = 0;

            while (true)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / turnUpTime;

                godLight.intensity = Mathf.Lerp(0, powerUpIntensity, fraction);

                if (fraction >= 1)
                {
                    _turnOnTime = Time.time;
                    source.clip = machineSound;
                    source.Play();
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator TurnOffVolume()
        {
            isEnabled = false;
            CameraManager.Instance.StartShaking(false);

            //Play Sound;
            if (source)
            {
                source.PlayOneShot(stopSound);
            }
            onTurningOff.Invoke();

            float timeElapsed = 0;

            while (true)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / turnUpTime;

                godLight.intensity = Mathf.Lerp(powerUpIntensity, 0, fraction);

                if (fraction >= 1)
                {
                    yield break;
                }

                yield return null;
            }
        }

        public void SelfDestruct()
        {
            if (source)
            {
                source.PlayOneShot(explodeSound);
            }
            onExplode.Invoke();
            isEnabled = false;
            _destroyed = true;
        }

        
        
        private void ResetObjects()
        {
            
        }

        public void ResetVolume()
        {
            isEnabled = false;
            _destroyed = false;
            godLight.intensity = 0;
            CameraManager.Instance.StartShaking(false);
        }
    }
}