using Misc;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.NPC_Weapon
{
    public class CreatureAttack : MonoBehaviour
    {
        public TriggerDamage triggerDamage;

        public void StartAttack()
        {
            triggerDamage.Enable(true);
        }
        
        public void EndAttack()
        {
            triggerDamage.Enable(false);
        }

    }
}
