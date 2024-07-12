using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Scripts.Volumes
{
    //[RequireComponent(typeof(BoxCollider))]
    public class SoundVolumeTrigger : MonoBehaviour
    {
        
        
        [SerializeField] private string volumeName;
        [SerializeField] private float volumeMultiplier = 1;

        [Space(10), SerializeField] private AudioSettings[] sources;
        [SerializeField] private float volumeTransitionTime;

        private bool _changeVolume;
        private bool _playerInTrigger;
        private float _transitionTimeElapsed;
        private Coroutine _triggerCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (_triggerCoroutine != null)
                {
                    StopCoroutine(_triggerCoroutine);
                }
                _triggerCoroutine = StartCoroutine(ResetTrigger());
                
                if (_playerInTrigger) return;
                
                _playerInTrigger = true;
                VolumeAction();
            }
        }


        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _playerInTrigger = false;
            _triggerCoroutine = null;
        }
        

        public void VolumeAction()
        {
            print("ohh daddy");
            var playerEffects = PlayerEffectsManager.Instance;
            playerEffects.CurrentEffectVolume = volumeName;
            playerEffects.VolumeMultiplier = volumeMultiplier;
            ApplyAudioVolume();
        }

        private void Update()
        {
            if (_changeVolume) { ApplyAudioVolume(); }
        }

        public void ApplyAudioVolume()
        {
            if (!_changeVolume)
            {
                _changeVolume = true;
                _transitionTimeElapsed = 0;
                for (int i = 0;  i<sources.Length; i++)
                {
                    sources[i].initialVolume = sources[i].source.volume;
                }
            }
            else
            {
                _transitionTimeElapsed += Time.deltaTime;
                float fraction = _transitionTimeElapsed / volumeTransitionTime;

                foreach (var source in sources)
                {
                    source.source.volume = Mathf.Lerp(source.initialVolume, source.preferredVolume, fraction);
                }

                if (fraction >= 1)
                {
                    _changeVolume = false;
                }
            }

        }


        [System.Serializable]
        public struct AudioSettings
        {
            public AudioSource source;
            public float preferredVolume;
            [HideInInspector] public float initialVolume;
        }
    }


}