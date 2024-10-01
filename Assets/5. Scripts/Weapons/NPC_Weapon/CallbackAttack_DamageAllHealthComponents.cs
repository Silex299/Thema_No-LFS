using Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons.NPC_Weapon
{
    // ReSharper disable once InconsistentNaming
    public class CallbackAttack_DamageAllHealthComponents : CallbackAttack
    {
        [BoxGroup("Weapon Action Params")] public int maximumCollider = 6;
        [BoxGroup("Weapon Action Params")] public LayerMask layerMask;

        protected override void AttackCallback()
        {
            Collider[] results = new Collider[maximumCollider];
            int num = Physics.OverlapSphereNonAlloc(overrideSocket ? overrideSocket.position : transform.position,
                damageDistance, results, layerMask);

            foreach (var result in results)
            {
                if(!result) continue;
                
                if (result.gameObject.TryGetComponent<HealthBaseClass>(out var health))
                {
                    health.TakeDamage(damage);
                }
            }
        }

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(overrideSocket ? overrideSocket.position : transform.position,
                damageDistance);
        }
    }
}