using System;
using System.Collections.Generic;
using Scriptable;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player_Scripts
{
    public class PlayerEffectsManager : SerializedMonoBehaviour
    {

        [FoldoutGroup("References")] public Player player;
        
        [FoldoutGroup("Sources")] public AudioSource playerInteractionSource;
        [FoldoutGroup("Sources")] public AudioSource playerSource;
        [FoldoutGroup("Sources")] public AudioSource playerSfxSource;


        [FoldoutGroup("Clips")] public Dictionary<string, AudioClip[]> steps;
        [FoldoutGroup("Clips")] public Dictionary<string, KeyedAudioClip> interactions;
        [FoldoutGroup("Clips")] public Dictionary<string, SoundClipArray> playerSounds;

        public float PlayerInteractionVolumeMultiplier { get; set; } = 1;
        public float PlayerVolumeMultiplier { get; set; } = 1;

        public struct KeyedAudioClip
        {
            public readonly Dictionary<string, AudioClip[]> clips;
            public KeyedAudioClip(Dictionary<string, AudioClip[]> clips)
            {
                this.clips = clips;
            }
        }
        

        private void Start()
        {
            if (!player)
            {
                player = GetComponent<Player>();
            }
        }

        public void PlaySteps(Object step)
        {
            
            if(player.DisabledPlayerMovement || player.IsOverridingAnimation) return;
            
            var stepInfo = step as Step;
            PlayStepSound();
            PlayEffects();
            
            return;

            void PlayStepSound()
            {
                if (!steps.ContainsKey(player.GroundTag)) return;
                if (!steps.TryGetValue(player.GroundTag, out var audioClips)) return;
            
                var randomClip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
                playerInteractionSource.PlayOneShot(randomClip, stepInfo == null ? PlayerInteractionVolumeMultiplier : (PlayerInteractionVolumeMultiplier * stepInfo.volume));
            }
            void PlayEffects()
            {
                //TODO: implement effects
            }
        }


        public void PlayPlayerInteraction(Object interaction)
        {
            
            var playerInteraction = interaction as PlayerInteraction;
            if(playerInteraction == null) return;
            
            var groundKey = player.GroundTag;
            
            if(!interactions.ContainsKey(groundKey)) return;

            if (!interactions.TryGetValue(groundKey, out var groundInteractions)) return;
            
            if (!groundInteractions.clips.ContainsKey(playerInteraction.interactionKey)) return;
            if (!groundInteractions.clips.TryGetValue(playerInteraction.interactionKey, out var clips)) return;

            playerInteractionSource.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)], playerInteraction.volume * PlayerVolumeMultiplier);


        }
        
        public void PlayPlayerSound(string soundKey)
        {
            
            if (!playerSounds.ContainsKey(soundKey)) return;
            if (!playerSounds.TryGetValue(soundKey, out var audioClip)) return;
            
            int randomIndex = UnityEngine.Random.Range(0, audioClip.clips.Length);
            var clip = audioClip.clips[randomIndex];

            playerSource.PlayOneShot(clip, audioClip.volume * PlayerVolumeMultiplier);
        }
        
        

    }
}