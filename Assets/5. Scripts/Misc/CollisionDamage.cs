using UnityEngine;

namespace Misc
{

    public class CollisionDamage : MonoBehaviour
    {

        [SerializeField] private float maximumDamage;
        [SerializeField] private bool enable = true;


        private void OnCollisionEnter(Collision collision)
        {

            Debug.LogError("Working");
            if (!enable) return;
            if (collision.gameObject.CompareTag("Player_Main") || collision.gameObject.CompareTag("Player"))
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