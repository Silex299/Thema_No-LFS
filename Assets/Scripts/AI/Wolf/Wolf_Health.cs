using System.Collections.Generic;
using Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AI.Wolf
{
    public class Wolf_Health : HealthBase
    {
        [SerializeField, BoxGroup("Wolf Variables")]
        private Animator animator;
        [SerializeField, BoxGroup("Wolf Variable")]
        private CharacterController wolfController;
        [SerializeField, BoxGroup("Wolf Variable")]
        private List<Component> componentsToDisable;


        protected override void Death()
        {
            wolfController.enabled = false;
            animator.enabled = false;


            DestroyComponents();

        }

        public void AnimationDeath(string animationName)
        {
            animator.CrossFade(animationName, 0.5f, 1);
            DestroyComponents();
        }

        private void DestroyComponents()
        {
            foreach (Component component in componentsToDisable)
            {
                Destroy(component);
            }
        }


    }
}
