using System;
using UnityEngine;

namespace Misc.Items
{
    public class AnimatedDoor : MonoBehaviour
    {
        
        public Animator animator;

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
            IsOpen = open;
        }
        
        
    }
}
