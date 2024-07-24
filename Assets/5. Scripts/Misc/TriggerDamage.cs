using System.Collections.Generic;
using System.Linq;
using Triggers;
using UnityEngine;

namespace Misc
{
    public class TriggerDamage : MonoBehaviour
    {
        [SerializeField] private float maximumDamage;

        public List<TriggerCondition> conditions;
        
        private void OnTriggerEnter(Collider other)
        {

            if (!enabled) return;
            
            if (other.gameObject.CompareTag("Player_Main") || other.gameObject.CompareTag("Player"))
            {
                if (conditions.Any(condition => !condition.Condition(other)))
                {
                    return;
                }

                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
            }
            
        }

    }
}
