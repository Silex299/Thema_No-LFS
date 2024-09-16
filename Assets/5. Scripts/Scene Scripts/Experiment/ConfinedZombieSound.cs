using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace Scene_Scripts.Experiment
{
    public class ConfinedZombieSound : SerializedMonoBehaviour
    {
        public Dictionary<string, SoundClipArray> interactionClips;
        public AudioSource source;

        public void PlayInteraction(string interaction)
        {
            if (!interactionClips.TryGetValue(interaction, out var interactionClip)) return;
            
            var clip = interactionClip.clips[Random.Range(0, interactionClip.clips.Length)];
            source.PlayOneShot(clip, interactionClip.volume);
        }
    }
}