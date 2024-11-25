using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misc
{
    public class JoystickController : MonoBehaviour
    {

        public float transitionTime = 2f;
        private float _currentIntensity;
        private Coroutine _transitionCoroutine;
        
        public void ShakeController(float intensity = 1)
        {
            if(Gamepad.current==null) return;
            
            _currentIntensity = intensity;
            Gamepad.current.SetMotorSpeeds(intensity, intensity);
        }

        public void ShakeControllerDirectional(bool left)
        {
            if(Gamepad.current==null) return;
            _currentIntensity = 1;
            if (left)
            {
                Gamepad.current.SetMotorSpeeds(1, 0);
            }
            else
            {
                Gamepad.current.SetMotorSpeeds(0, 1);
            }
        }
        
        public void StartShaker(float intensity)
        {
            if(Gamepad.current==null) return;
            if(_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionShaker(intensity));
        }

        public void StopShaker()
        {
            if(Gamepad.current==null) return;
            if(_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionShaker(0));
        }

        private IEnumerator TransitionShaker(float targetIntensity)
        {
            float timeElapsed = 0;
            float initIntensity = _currentIntensity;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float intensity = Mathf.Lerp(initIntensity, targetIntensity, timeElapsed / transitionTime);
                ShakeController(intensity);
                yield return null;
            }
            ShakeController(targetIntensity);
            
            _transitionCoroutine = null;
        }
        
        
        
        
        

    }
}
