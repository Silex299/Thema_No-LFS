using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class HealthBaseClass : MonoBehaviour
    {
        

        [SerializeField, FoldoutGroup("Base Health")] protected float maximumHealth = 100;
        [SerializeField, FoldoutGroup("Base Health")] protected UnityEvent deathAction;

        protected float currentHealth;
        
        public bool IsDead => currentHealth <= 0;
        
        public Action onDeath;

        protected virtual void Start()
        {
            currentHealth = maximumHealth;
        }
        
        public virtual void ResetHealth()
        {
            currentHealth = maximumHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
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
            currentHealth = 0;
            onDeath?.Invoke();
            deathAction?.Invoke();
        }


    }
}