using System.Collections;
using UnityEngine;

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

                if (_resetCoroutine != null)
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



        #region This is subscribed to the player's death event
        private void OnEnable()
        {
            if (PlayerMovementController.Instance)
            {
                PlayerMovementController.Instance.player.Health.OnDeath += OnPlayerDeath;
            }
        }
        private void Start()
        {
            if (PlayerMovementController.Instance)
            {
                PlayerMovementController.Instance.player.Health.OnDeath += OnPlayerDeath;
            }
        }

        #endregion

        #region This is unsubscribed to the player's death event
        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath -= OnPlayerDeath;
        }
        #endregion

        private void FixedUpdate()
        {
            if (!_playerInTrigger) return;

            Player player = PlayerMovementController.Instance.player;

            if (player.IsGrounded)
            {
                if(accelerationThreshold < Mathf.Abs(PlayerMovementController.Instance.player.verticalAcceleration))
                {
                    player.Health.TakeDamage(100);
                }
            }

        }

        private void OnPlayerDeath()
        {
            _playerInTrigger = false;
            gameObject.SetActive(false);
        }



    }
}
