using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class HealthBaseClass : MonoBehaviour
    {
        

        [SerializeField, FoldoutGroup("Base Health")] protected float maximumHealth = 100;
        [SerializeField, FoldoutGroup("Base Health")] protected UnityEvent deathAction;

        protected float _currentHealth;

        private void Start()
        {
            _currentHealth = maximumHealth;
        }


        public virtual void ResetHealth()
        {
            _currentHealth = maximumHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Death();
            }
        }

        public virtual void Kill(string message)
        {
            Death();
        }

        protected virtual void Death(string message = "")
        {
            deathAction?.Invoke();
        }


    }
}