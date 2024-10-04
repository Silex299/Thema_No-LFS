using Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.Canine
{
    public class CanineHealth : HealthBaseClass
    {

        [FoldoutGroup("Ragdoll")] public Rigidbody[] rigidBodies;
        [FoldoutGroup("Ragdoll")] public Collider[] colliders;
        [FoldoutGroup("References")] public Animator animator;


        protected override void Start()
        {
            base.Start();
            Reset();
        }
        
        public override void Kill(string message)
        {
            currentHealth = 0;
            animator.Play(message, 1);
        }
        protected override void Death(string message = "")
        {
            base.Death(message);
            RagdollDeath();
        }
        private void  RagdollDeath()
        {
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            foreach (var rb in rigidBodies)
            {
                rb.isKinematic = false;
            }
            animator.enabled = false;
        }
        public void Reset()
        {
            currentHealth = maximumHealth;
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            foreach (var rb in rigidBodies)
            {
                rb.isKinematic = true;
            }
            animator.enabled = true;
        }
    }
    
}
