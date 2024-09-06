using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace NPCs.New
{
    public class ZombieSound : SerializedMonoBehaviour
    {
        public AudioSource source;
        
        public SoundClip idleSoundClip;
        public SoundClip chaseSoundClip;
        public SoundClip eatingSoundClip;

        public Dictionary<string, SoundClipArray> actionSounds;


        
        private void OnEnable()
        {
            Npc zombie = GetComponent<Npc>();
            zombie.onStateChange += PlayStateSound;
        }
        
        private void OnDisable()
        {
            Npc zombie = GetComponent<Npc>();
            zombie.onStateChange -= PlayStateSound;
        }


        private void PlayStateSound(int stateIndex)
        {
            switch (stateIndex)
            {
                case 1:
                    if(!idleSoundClip.clip) break;
                    source.clip = idleSoundClip.clip;
                    source.volume = idleSoundClip.volume;
                    source.loop = true;
                    source.Play();
                    break;
                case 2:
                    
                    if(!chaseSoundClip.clip) break;
                    source.clip = chaseSoundClip.clip;
                    source.volume = chaseSoundClip.volume;
                    source.loop = true;
                    source.Play();
                    break;
                case 3:
                    if(!eatingSoundClip.clip) break;
                    source.clip = eatingSoundClip.clip;
                    source.volume = eatingSoundClip.volume;
                    source.loop = true;
                    break;
            }
            
        }
        
        public void PlayActionSound(string action)
        {
            if (!actionSounds.ContainsKey(action)) return;
            if (!actionSounds.TryGetValue(action, out var soundClipArray)) return;
            
            var randomClip = soundClipArray.clips[UnityEngine.Random.Range(0, soundClipArray.clips.Length)];
            
            source.PlayOneShot(randomClip, soundClipArray.volume);
        }
        
        
    }
}
