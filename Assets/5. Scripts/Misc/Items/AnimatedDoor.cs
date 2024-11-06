using Thema_Type;
using UnityEngine;

namespace Misc.Items
{
    public class AnimatedDoor : MonoBehaviour
    {
        
        public Animator animator;

        public AudioSource source;
        public SoundClip openSoundClip;
        public SoundClip closeSoundClip;

        [field: SerializeField] public bool IsOpen { get; private set; } = false;

        private void Start()
        {
            animator.Play(IsOpen ? "Open" : "Close", 0, 1);
        }

        public void OpenDoor(bool open)
        {
            print(open);
            if(open == IsOpen) return;

            animator.Play(open ? "Open" : "Close");
            PlaySound(open);
            IsOpen = open;
        }


        private void PlaySound(bool open)
        {
            if (source)
            {
                source.PlayOneShot(open ? openSoundClip.clip : closeSoundClip.clip, open ? openSoundClip.volume : closeSoundClip.volume);

            }
        }
        
    }
}
