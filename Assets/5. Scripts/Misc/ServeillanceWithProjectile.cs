using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Misc
{
    public class ServeillanceWithProjectile : MonoBehaviour
    {
        [Required] public ProjectileShooter projectileShooter;
        public SurveillanceVisuals visuals;
        public Vector3 targetOffset;
        public float resetAlignmentTime = 1f;
        public bool initiallyTurnedOff = false;

        private bool _aligned;
        private Coroutine _turnOffCoroutine;

        public void TargetDetected(Collider other)
        {
           if(!_aligned) StartCoroutine(OnDetected(other));
        }
        
        private IEnumerator AlignProjectile(float transitionTime, Vector3 target)
        {
            //align projectile transform to the target
            Vector3 direction = (target + targetOffset - projectileShooter.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                projectileShooter.transform.rotation = Quaternion.Slerp(projectileShooter.transform.rotation, targetRotation, elapsedTime / transitionTime);
                yield return null;
            }
            projectileShooter.transform.rotation = targetRotation;
        }
        private IEnumerator OnDetected(Collider other)
        {
            _aligned = true;
            yield return visuals.AlignSpotlight(other.transform.position);
            yield return AlignProjectile(0.01f, other.transform.position);
            projectileShooter.Shoot();
            
            yield return new WaitForSeconds(resetAlignmentTime);
            
            yield return visuals.ResetAlignment();
            _aligned = false;
            
        }
        
        public void TurnOffMachine(bool turnOff)
        {
            if (_turnOffCoroutine != null)
            {
                StopCoroutine(_turnOffCoroutine);
            }
            _turnOffCoroutine = StartCoroutine(TurnOffEnumerator(turnOff));
        }
        private IEnumerator TurnOffEnumerator(bool turnOff)
        {
            if (turnOff)
            {
                yield return visuals.PowerChange(SurveillanceVisuals.ServeillanceVisualState.PowerDown);
            }
            else
            {
                yield return visuals.PowerChange(SurveillanceVisuals.ServeillanceVisualState.Default);
            }
            
            _turnOffCoroutine = null;
        }


        public void Reset()
        {
            
            if (_turnOffCoroutine != null)
            {
                StopCoroutine(_turnOffCoroutine);
                _turnOffCoroutine = null;
            }
            
            _aligned = false;
            visuals.InstantPowerChange(initiallyTurnedOff? SurveillanceVisuals.ServeillanceVisualState.PowerDown : SurveillanceVisuals.ServeillanceVisualState.Default);
            
        }
    }
}