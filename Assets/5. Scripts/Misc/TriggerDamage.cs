using UnityEngine;

namespace Misc
{
    public class TriggerDamage : MonoBehaviour
    {
        [SerializeField] private float maximumDamage; 

        private void OnTriggerEnter(Collider other)
        {

            if (!enabled) return;
            if (other.gameObject.CompareTag("Player_Main") || other.gameObject.CompareTag("Player"))
            {
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
            }
            
        }

    }
}
