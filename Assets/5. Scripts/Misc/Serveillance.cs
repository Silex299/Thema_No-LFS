 using System.Collections;
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

        private bool _isPlayerInTrigger = false;
        private bool _objectFound = false;

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

        /// <summary>
        /// Checks for the player while the player is in the trigger. If the player is found, it triggers the object found sequence.
        /// </summary>
        private IEnumerator CheckForPlayer()
        {
            while (isMachineOn && _isPlayerInTrigger)
            {
                var playerTransform = PlayerMovementController.Instance.transform;
                Vector3 direction = (playerTransform.position + Vector3.up * 0.5f) - transform.position;
                Ray ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, rayCastMask))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    ObjectFound(hit.collider.tag);
                }

                yield return null;
            }
        }


        /// <summary>
        /// Handles the actions to be taken when an object is found. If the object is the player, it triggers the player found sequence.
        /// </summary>
        /// <param name="tag">The tag of the found object.</param>
        private void ObjectFound(string tag)
        {
            if (_objectFound) return;

            if (tag == "Player_Main")
            {
                PlayerFound();
                _objectFound = true;
                mover.enabled = false;
                visuals.PowerUp(this);
            }
        }

        private void PlayerFound()
        {
            PlayerMovementController.Instance.player.Health.Kill("RAY");
        }

        /// <summary>
        /// Toggles the state of the machine and its visuals based on the provided boolean value.
        /// </summary>
        /// <param name="turnOff">If true, the machine will be turned off. If false, the machine will be turned on.</param>
        private void TurnOffMachine(bool turnOff)
        {
            isMachineOn = !turnOff;
            mover.StopMover(turnOff);

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