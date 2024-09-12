using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class FallVolume : MonoBehaviour
    {
        #region Variables

        public List<ThresholdEvent> fallEvents;
        
        private bool _triggered;
        private Coroutine _playerInTriggerCoroutine;
        private Coroutine _exitTriggerCoroutine;


        [System.Serializable]
        public class ThresholdEvent
        {
            [SerializeField] private float velocityThreshold;
            [Tooltip("If true, the velocity must be greater than the threshold, otherwise it must be less than the threshold")]
            [SerializeField] private bool isGreater;
            [SerializeField] private UnityEvent onThresholdExceeded;
            
            
            public void CheckThreshold(float velocity)
            {
                if (isGreater)
                {
                    if (velocity > velocityThreshold)
                    {
                        print("hello motherfucker");
                        onThresholdExceeded.Invoke();
                    }
                }
                else
                {
                    if (velocity < velocityThreshold)
                    {
                        onThresholdExceeded.Invoke();
                    }
                }
            }
            
        }
        
        #endregion

        #region Checks if player is in trigger

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (!PlayerMovementController.Instance.player.IsGrounded)
                {
                    _playerInTriggerCoroutine??= StartCoroutine(PlayerInTrigger());
                }
                
                if(_exitTriggerCoroutine!=null)
                    StopCoroutine(_exitTriggerCoroutine);

                _exitTriggerCoroutine = StartCoroutine(ExitPlayerTrigger());

            }
        }


        private IEnumerator ExitPlayerTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            
            if (_playerInTriggerCoroutine != null)
            {
                StopCoroutine(_playerInTriggerCoroutine);
                _playerInTriggerCoroutine = null;
            }
            
            _playerInTriggerCoroutine = null;
        }
        
        private IEnumerator PlayerInTrigger()
        {
            float velocity = 0;

            while (!PlayerMovementController.Instance.player.IsGrounded)
            {
                velocity = PlayerVelocityCalculator.Instance.velocity.magnitude;
                yield return null;
            }
            
            Debug.LogWarning(velocity);

            foreach (var fallEvent in fallEvents)
            {
                fallEvent.CheckThreshold(velocity);
            }
            
        }
        
        
        #endregion

        public void ResetVolume()
        {
            gameObject.SetActive(true);
        }


    }
}