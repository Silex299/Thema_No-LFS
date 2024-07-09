using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class NpcHealth : HealthBaseClass
    {
    
        public CharacterController characterController;
        public Animator animator;

        public UnityEvent onKill;
    
        private bool _isDead;
    
        public override void Kill(string message)
        {
            onKill?.Invoke();
            if (message == "RAY")
            {
                AnimationDeath("Float");
            }
            else
            {
                TakeDamage(101);
            }
        }


        protected override void Death(string message = "")
        {
            if(_isDead) return;
        
            base.Death(message);
            _isDead = true;
            RagdollDeath();
        }

        private void RagdollDeath()
        {
            print("Ragdoll");
            animator.enabled = false;
            characterController.enabled = false;
        }

        private void AnimationDeath(string animationName)
        {
            animator.CrossFade(animationName, 0.2f, 1);
        }

        private void Reset()
        {
            animator.enabled = true;
            characterController.enabled = true;
            _isDead = false;
        }
    }
}
