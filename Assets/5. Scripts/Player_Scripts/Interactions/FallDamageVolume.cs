using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class FallDamageVolume : MonoBehaviour
    {

        [SerializeField] private float accelerationThreshold = 10f;
        
        private bool _playerInTrigger;
        private Coroutine _resetCoroutine;
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerInTrigger = true;

                if (_resetCoroutine!=null)
                {
                    StopCoroutine(_resetCoroutine);
                }
                _resetCoroutine = StartCoroutine(ResetTrigger());
            }
            
        }
        
        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _playerInTrigger = false;
            _resetCoroutine = null;

        }


        private void FixedUpdate()
        {
            if(!_playerInTrigger) return;

            Player player = PlayerMovementController.Instance.player;

            if (player.IsGrounded)
            {
                print(player.VerticalVelocity);
            }

        }

       
        

    }
}
