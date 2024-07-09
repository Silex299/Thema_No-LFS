using Sirenix.OdinInspector;

namespace Weapons.NPC_Weapon
{
    public class WeaponBase : SerializedMonoBehaviour
    {

        [BoxGroup("Base class param")] public float damage = 101;

        public virtual void Fire()
        {
            
        }
        
        public virtual void ResetWeapon()
        {
            
        }

    }
}
