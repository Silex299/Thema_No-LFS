using System;
using System.Collections;
using System.Collections.Generic;
using Health;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class Serveillance : MonoBehaviour
    {
        [SerializeField] private bool isMachineOn = true;
        [SerializeField] private Mover mover;
        [SerializeField] private LayerMask rayCastMask;


        [SerializeField] private SurveillanceVisuals visuals;

        private int _objectCount;
        private readonly Dictionary<int, Coroutine> _objectsTracking = new();

        private void OnTriggerEnter(Collider other)
        {
            
            if (!isMachineOn) return;
            if(!other.CompareTag("NPC")) return;
            
            print(other.name);
            
            if (other.TryGetComponent(out HealthBaseClass health))
            {
                print("Has Health");

                if (_objectsTracking.TryGetValue(other.GetInstanceID(), out var coroutine)) return;
                
                //FIX THIS
                
                Coroutine newCoroutine = StartCoroutine(CheckForNpc(other, health));
                _objectsTracking.Add(other.GetInstanceID(), newCoroutine);
                
            }
            
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isMachineOn) return;
            
            if(!other.CompareTag("Player_Main") || other.CompareTag("NPC")) return;
            
            if (_objectsTracking.TryGetValue(other.GetInstanceID(), out var coroutine))
            {
                StopCoroutine(coroutine);
                _objectsTracking.Remove(other.GetInstanceID());
            }
        }

        private IEnumerator CheckForNpc(Collider other, HealthBaseClass health)
        {
            while (isMachineOn)
            {
                var otherTransform = other.transform;
                Vector3 direction = (otherTransform.position + Vector3.up * 0.5f) - transform.position;
                Ray ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, rayCastMask))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    ObjectFound(health);
                    try
                    {

                        StopCoroutine(_objectsTracking[other.GetInstanceID()]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                    StopCoroutine(_objectsTracking[other.GetInstanceID()]);
                    _objectsTracking.Remove(other.GetInstanceID());
                }

                yield return null;
            }
        }
        

        /// <summary>
        /// Handles the actions to be taken when an object is found. If the object is the player, it triggers the player found sequence.
        /// </summary>
        /// <param name="health">The found object health</param>
        private void ObjectFound(HealthBaseClass health)
        {
            DamageObject(health);
            if (mover) mover.enabled = false;
            visuals.PowerUp(this);
        }

        private void DamageObject(HealthBaseClass health) => health.Kill("RAY");


        /// <summary>
        /// Toggles the state of the machine and its visuals based on the provided boolean value.
        /// </summary>
        /// <param name="turnOff">If true, the machine will be turned off. If false, the machine will be turned on.</param>
        private void TurnOffMachine(bool turnOff)
        {
            isMachineOn = !turnOff;
            if (mover) mover.StopMover(turnOff);

            if (turnOff)
                visuals.PowerDown(this);
            else
                visuals.PowerDefault(this);
        }
    }


    [System.Serializable]
    public class SurveillanceVisuals
    {
        [BoxGroup("Visual"), SerializeField] private Light targetLight;

        [BoxGroup("Visual"), SerializeField] private float defaultIntensity = 10;
        [BoxGroup("Visual"), SerializeField] private float powerUpIntensity = 10;

        [Space(10), BoxGroup("Visual"), SerializeField]
        private VLB.VolumetricLightBeam volumetricLightBeam;

        [BoxGroup("Visual"), SerializeField] private float defaultLightBeamIntensity;
        [BoxGroup("Visual"), SerializeField] private float powerUpLightBeamIntensity;

        [BoxGroup("Visual"), SerializeField] private float transitionTime = 5;


        private Coroutine _powerChangeCoroutine;

        private IEnumerator PowerChange(float lightIntensity, float beamIntensity)
        {
            float currentLightIntensity = targetLight.intensity;
            float currentBeamIntensity = volumetricLightBeam.intensityInside;

            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / transitionTime;

                targetLight.intensity = Mathf.Lerp(currentLightIntensity, lightIntensity, fraction);
                volumetricLightBeam.intensityGlobal = Mathf.Lerp(currentBeamIntensity, beamIntensity, fraction);
                yield return null;
            }
        }


        public void PowerUp(MonoBehaviour component)
        {
            if (_powerChangeCoroutine != null)
            {
                component.StopCoroutine(_powerChangeCoroutine);
            }

            _powerChangeCoroutine = component.StartCoroutine(PowerChange(powerUpIntensity, powerUpLightBeamIntensity));
            component.StartCoroutine(AlignSpotlight(PlayerMovementController.Instance.transform.position));
        }

        public void PowerDown(MonoBehaviour component)
        {
            if (_powerChangeCoroutine != null)
            {
                component.StopCoroutine(_powerChangeCoroutine);
            }

            _powerChangeCoroutine = component.StartCoroutine(PowerChange(0, 0));
        }

        //Function for turning to defaultPower
        public void PowerDefault(MonoBehaviour component)
        {
            if (_powerChangeCoroutine != null)
            {
                component.StopCoroutine(_powerChangeCoroutine);
            }

            _powerChangeCoroutine = component.StartCoroutine(PowerChange(defaultIntensity, defaultLightBeamIntensity));
        }

        private IEnumerator AlignSpotlight(Vector3 lookAt)
        {
            var spotlightTransform = targetLight.transform;

            Vector3 initialDirection = spotlightTransform.forward;
            Vector3 targetDirection = (lookAt - spotlightTransform.position).normalized;

            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                Vector3 newDirection = Vector3.Slerp(initialDirection, targetDirection, elapsedTime / transitionTime);
                spotlightTransform.rotation = Quaternion.LookRotation(newDirection);
                yield return null;
            }

            spotlightTransform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}