using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Health
{
    public class HealthBase : MonoBehaviour
    {
        [SerializeField, BoxGroup("Base Variables")] protected float maximumHealth = 100f;
        [SerializeField, BoxGroup("Base Variables")] protected float currentHealth;


        protected virtual void Start()
        {
            currentHealth = maximumHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Death();
            }
        }

        protected virtual void Death()
        {

        }

        public virtual void DeathByGodRay()
        {
            
        }

        public virtual void DeathByParticles()
        {
            
        }
    }
}
