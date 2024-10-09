using System.Collections.Generic;
using NPCs.New.V1;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.New
{
    public class V1NpcCanineSound : SerializedMonoBehaviour
    {


        public V1Npc parentNpc;
        public AudioSource audioSource;
        public Dictionary<string, SoundClip[]> soundClips;
        public Dictionary<int, SoundClip[]> stateSound;

        private void OnEnable()
        {
            parentNpc.onStateChange += OnStateChange;
            parentNpc.onNpcDeath += OnNpcDeath;
        }
        private void OnDisable()
        {
            parentNpc.onStateChange -= OnStateChange;
            parentNpc.onNpcDeath -= OnNpcDeath;
        }

        private void OnStateChange(int index)
        {
            if (!stateSound.TryGetValue(index, out var clip)) return;
            
            var sound = clip[Random.Range(0, clip.Length)];
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.loop = true;
            audioSource.Play();
        }
        public void PlayAnimSound(string key)
        {
            if (!soundClips.TryGetValue(key, out var clips)) return;
            
            var sound = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        
        
        private void OnNpcDeath()
        {
            //play from state clip with index -1
            if (!stateSound.TryGetValue(-1, out var clip)) return;
            
            var sound = clip[Random.Range(0, clip.Length)];
            audioSource.volume = 1;
            audioSource.Stop();
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        
    }
}
