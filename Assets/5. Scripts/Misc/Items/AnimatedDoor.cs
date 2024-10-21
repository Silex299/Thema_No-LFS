using UnityEngine;

namespace Misc.Items
{
    public class AnimatedDoor : MonoBehaviour
    {
        
        public Animator animator;

        [field: SerializeField] public bool IsOpen { get; private set; } = false;
        
        public void OpenDoor(bool open)
        {
            print(open);
            if(open == IsOpen) return;

            animator.Play(open ? "Open" : "Close");
            IsOpen = open;
        }
        
        
    }
}
