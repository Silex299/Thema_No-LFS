using System.Collections;
using Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NavMesh_NPCs
{
    public class AgentNpcHealth : HealthBaseClass
    {

        [Space(10), BoxGroup("NPC Health")] public Animator animator;
        [Space(10), BoxGroup("NPC Health")] public Behaviour[] componentsToDisable;


        public override void Kill(string message)
        {
            if (message == "RAY")
            {
                StartCoroutine(RayDeath());
            }
            else
            {
                Death();
            }
            
        }


        private IEnumerator RayDeath()
        {
            
            animator.CrossFade("Float", 0.2f, 1);
            
            yield return new WaitForSeconds(4f);
            
            PlayRagdoll(true);
            Death();
            
            yield return null;
        }


        
        protected override void Death(string message = "")
        {
            base.Death(message);
            DisableComponents(true);
        }
        
        public override void ResetHealth()
        {
            base.ResetHealth();
            
            PlayRagdoll(false);
            DisableComponents(false);
            
        }
        private void DisableComponents(bool disable)
        {
            foreach (var behaviour in componentsToDisable)
            {
                behaviour.enabled = false;
            }
        }
        private void PlayRagdoll(bool playRagdoll)
        {
            if (animator)
            {
                animator.enabled = !playRagdoll;
            }
        }
        
    }
}
