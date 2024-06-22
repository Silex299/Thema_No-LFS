using Sirenix.OdinInspector;

namespace Weapons
{
    public class WeaponBase : SerializedMonoBehaviour
    {

        [BoxGroup("Base class param")] public float damage = 101;

        public virtual void Fire()
        {
            
        }

    }
}
