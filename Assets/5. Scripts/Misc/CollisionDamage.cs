using Thema_Type;
using UnityEngine;

namespace Misc
{

    public class CollisionDamage : MonoBehaviour
    {

        [SerializeField] private float maximumDamage;
        [SerializeField] private AudioSource source;
        [SerializeField] private SoundClip hitClip;
        [field: SerializeField] public bool CanDamage { get; set; } = true;

        private void OnCollisionEnter(Collision collision)
        {
            if(!CanDamage) return;
            
            if (collision.gameObject.CompareTag("Player_Main") || collision.gameObject.CompareTag("Player"))
            {
               
                Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(maximumDamage);
                PlaySound();
            }
            
        }

        
        private void PlaySound() => source.PlayOneShot(hitClip.clip, hitClip.volume);
        
       
    }

}