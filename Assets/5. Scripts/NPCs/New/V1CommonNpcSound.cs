using System.Collections.Generic;
using NPCs.New.V1;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.New
{
    public class V1CommonNpcSound : SerializedMonoBehaviour
    {
        
        public V1Npc parentNpc;
        public AudioSource audioSource;
        public Dictionary<int, SoundClip[]> stateClips;
        public Dictionary<string, SoundClip[]> actionClips;
        
        
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

        //TODO: CHANGE THE NAME AND IN THOSE ANIMATIONS TOO, IF YOU HAVE TIME
        
        /// <summary>
        /// Play sound when npc places a footstep, Called from animation event
        /// </summary>
        public void PlayFootstepSound()
        {
            if (!actionClips.TryGetValue("Step", out var soundClips)) return;
            if(soundClips.Length<=0) return;
            
            //play one random from sound clips
            var sound = soundClips[Random.Range(0, soundClips.Length)];
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        
        /// <summary>
        /// Play sound when npc performs an action, Called from animation event
        /// </summary>
        /// <param name="key"></param>
        public void PlayActionSound(string key)
        {
            if (!actionClips.TryGetValue(key, out var soundClips)) return;
            if(soundClips.Length<=0) return;
            
            //play one random from sound clips
            var sound = soundClips[Random.Range(0, soundClips.Length)];
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        
        /// <summary>
        /// Play sound when npc state changes
        /// </summary>
        /// <param name="stateIndex"></param>
        private void OnStateChange(int stateIndex)
        {
            if (!stateClips.TryGetValue(stateIndex, out var soundClip)) return;
            
            var sound = soundClip[Random.Range(0, soundClip.Length)];
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.loop = true;
            audioSource.Play();
            
        }
        
        /// <summary>
        /// Play sound when npc dies
        /// </summary>
        private void OnNpcDeath()
        {
            //play from state clip with index -1
            if (!actionClips.TryGetValue("Death", out var soundClip)) return;
            
            
            print("Calling death sound");
            var sound = soundClip[Random.Range(0, soundClip.Length)];
            audioSource.volume = 1;
            audioSource.Stop();
            audioSource.PlayOneShot(sound.clip, sound.volume);
            
        }
    }
}
