using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Misc
{
    [RequireComponent(typeof(ProjectileShooter))]
    public class ServeillanceWithProjectile : MonoBehaviour
    {
        
        [Required] public ProjectileShooter projectileShooter;
        public SurveillanceVisuals visuals;

        
        
        
        
    }
}
