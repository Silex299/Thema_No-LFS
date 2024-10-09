using UnityEngine;

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
