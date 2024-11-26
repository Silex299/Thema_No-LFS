using System.Collections.Generic;
using Misc.New;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;

namespace Misc.Custom_Object_Scripts
{
    public class BoxInExperiments : MonoBehaviour
    {

        public Animator animator;
        public RopeRenderer[] ropes;
        public AudioSource audioSource;
        public List<SoundClip> soundClips;
        private static readonly int Drop = Animator.StringToHash("Drop");

        [Button]
        public void DropBox()
        {
            animator.SetTrigger(Drop);
        }

        public void BreakFirstRope()
        {
            ropes[0].BreakRope(6);
        }
        public void BreakSecondRope()
        {
            ropes[1].BreakRope(4);
        }
        public void PlaySound(int index)
        {
            audioSource.PlayOneShot(soundClips[index].clip);
        }
        
        public void Reset()
        {
            animator.enabled = true;
            animator.Play("Idle");
            foreach (var rope in ropes)
            {
                rope.Reset();
            }
        }

        public void Set()
        {
            animator.Play("Box Fall", 0, 1f);
            ropes[0].BreakRope(6);
            ropes[1].BreakRope(4);
        }
        
        
    }
    
}