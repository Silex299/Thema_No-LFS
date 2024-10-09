using System;
using NPCs.New.V1;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.New
{
    
    /**
     * This script doesn't contain any advanced logic for surface detection or footstep effect
     **/
    public class V1NpcSound : MonoBehaviour
    {
        
        public AudioSource audioSource;
        public float volumeMultiplier;
        public AudioClip[] soundClips;
        public AudioClip[] otherSoundClips;

        [Space(10)] public bool subscribeToStateChange;
        
        private void OnEnable()
        {
            if(subscribeToStateChange)  GetComponent<V1Npc>().onStateChange += PlayOtherSound;
        }
        private void OnDisable()
        {
            if(subscribeToStateChange)  GetComponent<V1Npc>().onStateChange -= PlayOtherSound;
        }

        public void PlayFootstepSound()
        {
            audioSource.PlayOneShot(soundClips[Random.Range(0, soundClips.Length)], volumeMultiplier);
        }

        public void PlayOtherSound(int index)
        {
            audioSource.PlayOneShot(otherSoundClips[index], volumeMultiplier);
        }
        
    }
}
