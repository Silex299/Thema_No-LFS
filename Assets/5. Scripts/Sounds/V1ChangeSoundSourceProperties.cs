using System.Collections;
using Thema;
using UnityEngine;

namespace Sounds
{
    public class V1ChangeSoundSourceProperties : MonoBehaviour
    {
        public SoundSource source;
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

            _soundCoroutine = StartCoroutine(ChangeSourceProperties(source.maximumVolume, defaultTransitionTime));
        }
        
        private IEnumerator ChangeSourceProperties(float targetVolume, float transitionTime = 1f)
        {
            
            if (!IsPlaying)
            {
                source.source.Play();
            }
            
            float initialVolume = source.source.volume;
            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                source.source.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / transitionTime);
                yield return null;
            }
            
            source.source.volume = targetVolume;
            IsPlaying = !Mathf.Approximately(targetVolume, 0);
            if (!IsPlaying)
            {
                source.source.Stop();
            }

            _soundCoroutine = null;

        }
    }
}
