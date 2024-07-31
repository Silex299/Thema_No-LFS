using Misc;
using UnityEngine;
using UnityEngine.Serialization;
using TriggerDamage = Misc.New.TriggerDamage;

namespace Weapons.NPC_Weapon
{
    public class CreatureAttack : MonoBehaviour
    {
        public TriggerDamage triggerDamage;

        public void StartAttack()
        {
            triggerDamage.enabled = true;
        }
        
        public void EndAttack()
        {
            triggerDamage.enabled = false;
        }

    }
}
