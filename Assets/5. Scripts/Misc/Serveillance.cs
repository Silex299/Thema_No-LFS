using System.Collections;
using System.Collections.Generic;
using Health;
using Sirenix.OdinInspector;
using UnityEngine;
using VisualState = Misc.SurveillanceVisuals.ServeillanceVisualState;

namespace Misc
{
    public class Serveillance : SerializedMonoBehaviour
    {
        [SerializeField] private Mover mover;
        [SerializeField, Tooltip("Delay before the surveillance system resets after detecting an object.")] private float resetDelay = 8f;
        [SerializeField, Tooltip("Hitting layer mask")] private LayerMask rayCastMask;
        [SerializeField] private SurveillanceVisuals visuals;

        private int _objectCount;
        public new bool enabled = true;

        private Dictionary<int, bool> _objectsTracking = new Dictionary<int, bool>();
        private Coroutine _powerChangeCoroutine;
        private Coroutine _resetCoroutine;

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;

            if (other.CompareTag("Player_Main") || other.CompareTag("NPC"))
            {
                if (_objectsTracking.ContainsKey(other.GetInstanceID())) return;

                if (IsInLineOfSight(other.transform.position))
                {
                    if (other.TryGetComponent(out HealthBaseClass health))
                    {
                        mover?.DisableMover(true);

                        DamageObject(other.GetInstanceID(), health);
                        StartCoroutine(visuals.PowerChange(VisualState.PowerUp));
                        StartCoroutine(visuals.AlignSpotlight(other.transform.position));
                        
                        //reset
                        if (_resetCoroutine != null) StopCoroutine(_resetCoroutine);
                        _resetCoroutine = StartCoroutine(ResetServeillanceVisual(other));
                    }
                }
            }
        }

        private bool IsInLineOfSight(Vector3 target)
        {
            Vector3 targetPos = target + 0.5f * Vector3.up;
            Vector3 direction = (targetPos - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, rayCastMask))
            {
                return hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("NPC");
            }

            return false;
        }
        private void DamageObject(int instanceId, HealthBaseClass health)
        {
            _objectsTracking.Add(instanceId, true);
            health.Kill("RAY");
        }
        private IEnumerator ResetServeillanceVisual(Collider other)
        {
            yield return new WaitForSeconds(resetDelay);

            mover?.DisableMover(false);
            TurnOffMachine(false);

            StartCoroutine(visuals.ResetAlignment());
        }

        /// <summary>
        /// Toggles the state of the machine and its visuals based on the provided boolean value.
        /// </summary>
        /// <param name="turnOff">If true, the machine will be turned off. If false, the machine will be turned on.</param>
        public void TurnOffMachine(bool turnOff)
        {
            enabled = !turnOff;

            if (mover) mover.StopMover(turnOff);
            if (_powerChangeCoroutine != null) StopCoroutine(_powerChangeCoroutine);

            if (turnOff)
            {
                _objectsTracking = null;
                _powerChangeCoroutine = StartCoroutine(visuals.PowerChange(VisualState.PowerDown));
            }
            else
            {
                _powerChangeCoroutine = StartCoroutine(visuals.PowerChange(VisualState.Default));
            }
        }


        public void Reset()
        {
            _objectsTracking = new Dictionary<int, bool>();
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

        [BoxGroup("Visual"), SerializeField, Tooltip("Time taken for the transition between visual states.")]
        private float transitionTime = 5;


        private Quaternion _defaultSpotlightRotation = Quaternion.identity;

        public IEnumerator AlignSpotlight(Vector3 lookAt)
        {
            var spotlightTransform = targetLight.transform;
            _defaultSpotlightRotation = spotlightTransform.rotation;

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

        public IEnumerator ResetAlignment()
        {
            if (_defaultSpotlightRotation == Quaternion.identity) yield break;

            var spotlightTransform = targetLight.transform;
            Quaternion initialRotation = spotlightTransform.rotation;

            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;

                spotlightTransform.rotation = Quaternion.Slerp(initialRotation, _defaultSpotlightRotation,
                    elapsedTime / transitionTime);

                yield return null;
            }
            
            spotlightTransform.rotation = _defaultSpotlightRotation;
        }

        public IEnumerator PowerChange(ServeillanceVisualState visualState)
        {
            float lightIntensity = 0;
            float beamIntensity = 0;

            switch (visualState)
            {
                case ServeillanceVisualState.Default:
                    lightIntensity = defaultIntensity;
                    beamIntensity = defaultLightBeamIntensity;
                    break;
                case ServeillanceVisualState.PowerUp:
                    lightIntensity = powerUpIntensity;
                    beamIntensity = powerUpLightBeamIntensity;
                    break;
                case ServeillanceVisualState.PowerDown:
                    lightIntensity = 0;
                    beamIntensity = 0;
                    break;
            }


            float currentLightIntensity = targetLight.intensity;
            float currentBeamIntensity = volumetricLightBeam ? volumetricLightBeam.intensityInside : 0;

            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / transitionTime;

                targetLight.intensity = Mathf.Lerp(currentLightIntensity, lightIntensity, fraction);
                if (volumetricLightBeam)
                {
                    volumetricLightBeam.intensityGlobal = Mathf.Lerp(currentBeamIntensity, beamIntensity, fraction);
                }

                yield return null;
            }
        }


        public enum ServeillanceVisualState
        {
            Default,
            PowerUp,
            PowerDown
        }
    }
}