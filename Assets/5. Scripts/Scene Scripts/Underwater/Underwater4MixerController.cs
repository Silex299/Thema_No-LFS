using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Scene_Scripts.Underwater
{
    public class Underwater4MixerController : MonoBehaviour
    {
        public AudioMixer underwater4Mixer;

        private Coroutine _envVolumeCoroutine;
        

        
        
        public void PauseEnvironment(float transitionTime)
        {
            _envVolumeCoroutine = StartCoroutine(TransitionEnvVolume("Env_Volume", -80, transitionTime));
        }
        public void PlayEnvironment(float transitionTime)
        {
            _envVolumeCoroutine = StartCoroutine(TransitionEnvVolume("Env_Volume", 0, transitionTime));
        }

        public void PauseMusic(float transitionTime)
        {
            _envVolumeCoroutine = StartCoroutine(TransitionEnvVolume("Music_Volume", -80, transitionTime));
        }
        public void PlayMusic(float transitionTime)
        {
            _envVolumeCoroutine = StartCoroutine(TransitionEnvVolume("Music_Volume", 0, transitionTime));
        }
        

        private IEnumerator TransitionEnvVolume(string property, float targetVolume, float transitionTime)
        {
            
            if (!underwater4Mixer.GetFloat(property, out float value)) yield break;
            float initVolume = value;
            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float volume = Mathf.Lerp(initVolume, targetVolume, timeElapsed / transitionTime);
                underwater4Mixer.SetFloat(property, volume);
                yield return null;
            }
            
            underwater4Mixer.SetFloat(property, targetVolume);

            _envVolumeCoroutine = null;
        }
    }
}
