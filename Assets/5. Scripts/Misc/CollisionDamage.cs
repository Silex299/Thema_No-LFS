using UnityEngine;

namespace Misc
{

    public class CollisionDamage : MonoBehaviour
    {

        [SerializeField] private float maximumDamage;

        private void OnCollisionEnter(Collision collision)
        {

            if (!enabled) return;
            print(collision.gameObject.name);
            if (collision.gameObject.CompareTag("Player_Main") || collision.gameObject.CompareTag("Player"))
            {
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
            }
            
        }

       
    }

}