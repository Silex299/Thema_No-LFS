using System;
using System.Collections;
using UnityEngine;

namespace Sounds
{
    public class ChangeSoundSourceProperties : MonoBehaviour
    {

        public AudioSource source;
        public float defaultTransitionTime;

        [field: SerializeField] public bool IsPlaying { get; set; }
        private Coroutine _soundCoroutine;
        
        public void StopSource()
        {
            if (_soundCoroutine != null)
            {
                StopCoroutine(_soundCoroutine);
            }

            _soundCoroutine = StartCoroutine(ChangeSourceProperties(0, defaultTransitionTime));
        }
        public void PlaySource()
        {
            if (_soundCoroutine != null)
            {
                StopCoroutine(_soundCoroutine);
            }

            _soundCoroutine = StartCoroutine(ChangeSourceProperties(1, defaultTransitionTime));
        }

        public void ChangeSourceVolume(float volume)
        {
            if (_soundCoroutine != null)
            {
                StopCoroutine(_soundCoroutine);
            }

            _soundCoroutine = StartCoroutine(ChangeSourceProperties(volume, defaultTransitionTime));
        }
        
        private IEnumerator ChangeSourceProperties(float targetVolume, float transitionTime = 1f)
        {
            
            if (!IsPlaying)
            {
                source.Play();
            }
            
            float initialVolume = source.volume;
            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                source.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / transitionTime);
                yield return null;
            }
            
            source.volume = targetVolume;
            IsPlaying = !Mathf.Approximately(targetVolume, 0);
            if (!IsPlaying)
            {
                source.Stop();
            }

            _soundCoroutine = null;

        }


        public void ResetSource(bool reset)
        {
            source.volume = !reset? 1 : 0; 
            IsPlaying = !reset;
        }
        
    }
}
