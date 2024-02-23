using System.Collections.Generic;
using Health;
using UnityEngine;

namespace Scripts.Health
{
    public class DamageOnHit : MonoBehaviour
    {

        [SerializeField] private string hitTag;
        [SerializeField] private float maximumDamage;

        public bool isActive = true;

        [SerializeField] private List<HealthBase> damagableObject;


        private void OnCollisionEnter(Collision collision)
        {

            if (!isActive) return;

            if (collision.collider.CompareTag(hitTag))
            {
               
                foreach(var health in damagableObject)
                {
                    health.TakeDamage(maximumDamage);
                }
            }
        }

        public void Activate(bool active)
        {
            isActive = active;
        }

    }
}