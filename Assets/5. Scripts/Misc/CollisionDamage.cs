using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc
{

    public class CollisionDamage : MonoBehaviour
    {

        [SerializeField] private float maximumDamage;
        [field: SerializeField] public bool CanDamage { get; set; } = true;

        private void OnCollisionEnter(Collision collision)
        {
            if(!CanDamage) return;
            
            if (collision.gameObject.CompareTag("Player_Main") || collision.gameObject.CompareTag("Player"))
            {
               
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
            }
            
        }

       
    }

}