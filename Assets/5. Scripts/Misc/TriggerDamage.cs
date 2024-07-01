using UnityEngine;

namespace Misc
{
    public class TriggerDamage : MonoBehaviour
    {
        [SerializeField] private float maximumDamage;
        [SerializeField] private bool enable = true;


        private void OnTriggerEnter(Collider other)
        {

            if (!enable) return;
            if (other.gameObject.CompareTag("Player_Main") || other.gameObject.CompareTag("Player"))
            {
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
            }
            
        }


        public void Enable(bool status)
        {
            enable = status;
        }

    }
}
