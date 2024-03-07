using UnityEngine;

namespace Sounds
{
    //[RequireComponent(typeof(BoxCollider))]
    public class SoundVolumeTrigger : MonoBehaviour
    {


        [SerializeField] private string volumeName;
        [SerializeField] private float volumeMultiplier = 1;

        [Space(10), SerializeField] private AudioSettings[] sources;
        [SerializeField] private float volumeTransitionTime;

        private bool _changeVolume;
        private float _transitionTimeElapsed;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Player_Main"))
            {
                VolumeAction();
            }
        }

        public void VolumeAction()
        {
            var playerEffects = Player_Scripts.PlayerEffectsManager.Instance;
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
                    source.source.volume = Mathf.Lerp(source.initialVolume, source.preferedVolume, fraction);
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
            public float preferedVolume;
            [HideInInspector] public float initialVolume;
        }
    }


}