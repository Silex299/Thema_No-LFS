using System.Collections.Generic;
using System.Linq;
using Health;
using UnityEngine;

namespace Misc.New
{
    public class TriggerDamage : MonoBehaviour
    {
        [SerializeField] private List<string> tags;
        [SerializeField] private float maximumDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Any(other.CompareTag)) return;
            
            
            if (other.TryGetComponent<HealthBaseClass>(out var health))
            {
                health.TakeDamage(maximumDamage);
            }
        }
        
        
    }
}