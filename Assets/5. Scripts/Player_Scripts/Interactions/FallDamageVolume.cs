using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class FallDamageVolume : MonoBehaviour
    {

        #region Variables
        [SerializeField] private float accelerationThreshold = 10f;

        [SerializeField] private UnityEvent onThresholdExceeded;

        private bool _playerInTrigger;
        private Coroutine _resetCoroutine;

        #endregion

        #region  Checks if player is in trigger

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

        #endregion

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

        /// <summary>
        /// Checks if the player is in the trigger and applies fall damage if necessary.
        /// </summary>
        private void FixedUpdate()
        {
            // If the player is not in the trigger, exit the function.
            if (!_playerInTrigger) return;

            // Get the instance of the player.
            Player player = PlayerMovementController.Instance.player;

            // If the player is grounded and their vertical acceleration exceeds the threshold, apply damage.
            if (player.IsGrounded && accelerationThreshold < Mathf.Abs(player.verticalAcceleration))
            {
                onThresholdExceeded.Invoke();
            }
        }

        private void OnPlayerDeath()
        {
            _playerInTrigger = false;
            gameObject.SetActive(false);
        }

    }
}
