using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Player_Scripts;
using Sirenix.OdinInspector;
using Triggers;
using UnityEngine;

namespace Misc
{
    public class Serveillance : MonoBehaviour
    {
        [SerializeField] private bool isMachineOn = true;
        [SerializeField] private Mover mover;
        [SerializeField] private LayerMask rayCastMask;

        private bool _isPlayerInTrigger = false;

        [SerializeField] private SurveillanceVisuals visuals;

        private void OnTriggerEnter(Collider other)
        {
            if (!isMachineOn) return;
            if (!other.CompareTag("Player_Main")) return;

            _isPlayerInTrigger = true;
            StartCoroutine(CheckForPlayer());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isMachineOn) return;
            if (!other.CompareTag("Player_Main")) return;

            _isPlayerInTrigger = false;
            StopCoroutine(CheckForPlayer());
        }

        private IEnumerator CheckForPlayer()
        {
            if (!isMachineOn)
            {
                yield break;
            }

            while (_isPlayerInTrigger)
            {
                var playerTransform = PlayerMovementController.Instance.transform;

                Vector3 direction = (playerTransform.position + Vector3.up * 0.5f) - transform.position;
                Ray ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, rayCastMask))
                {
                    //Debug a line from the machine to hit point
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    Debug.Log(hit.collider.name);
                    
                    if (hit.transform.CompareTag("Player_Main"))
                    {
                        PlayerFound();
                    }
                }

                yield return null; // Adjust the wait time as needed
            }
        }

        private bool called;

        private void PlayerFound()
        {
            if (called) return;
            
            mover.enabled = false;
            visuals.PowerUp(this);
            PlayerMovementController.Instance.player.Health.Kill("RAY");
            called = true;
        }

        private void TurnOffMachine(bool turnOff)
        {
            isMachineOn = !turnOff;

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