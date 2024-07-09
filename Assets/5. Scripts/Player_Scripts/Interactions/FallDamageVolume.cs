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

        #endregion

        #region  Checks if player is in trigger

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {

                Player player = PlayerMovementController.Instance.player;

                if(player.IsGrounded && accelerationThreshold < Mathf.Abs(player.verticalAcceleration))
                {
                    onThresholdExceeded.Invoke();
                }
            }

        }

        #endregion

        #region This is subscribed to the player's death event
        private void OnEnable()
        {
            if (PlayerMovementController.Instance)
            {
                PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
            }
        }
        private void Start()
        {
            if (PlayerMovementController.Instance)
            {
                PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
            }
        }

        #endregion

        #region This is unsubscribed to the player's death event
        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;
        }
        #endregion

        private void OnPlayerDeath()
        {
            gameObject.SetActive(false);
        }

    }
}
