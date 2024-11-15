using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions.Animation
{
    public class AnimationAction : MonoBehaviour
    {
        [SerializeField] private string entryAnimation;
        [SerializeField] private string exitAnimation;
        [SerializeField] private string input;
        [SerializeField] private bool disablePlayerMovment = true;

        public float exitActionDelay = 2f;
        public float entryDelay = 2f;
        [SerializeField, Space(10)] private UnityEvent entryAction;
        [SerializeField] private UnityEvent exitAction;
        private bool _playerEngaged;
        private bool _enabled = true;

        private void OnTriggerEnter(Collider other)
        {
            if(!_enabled) return;
            if (other.CompareTag("Player_Main"))
            {
                StartCoroutine(EnterAction());
            }
        }

        private void Update()
        {
            if (!_playerEngaged) return;

            if (Input.GetButton(input))
            {
                _playerEngaged = false;
                _enabled = false;
                StartCoroutine(ExitAction());
            }
        }

        private IEnumerator EnterAction()
        {
            entryAction?.Invoke();
            PlayerMovementController.Instance.PlayAnimation(entryAnimation, 0.1f, 1);

            if (disablePlayerMovment)
            {
                yield return new WaitForSeconds(0.3f);
                PlayerMovementController.Instance.DisablePlayerMovement(true);
            }

            yield return new WaitForSeconds(entryDelay);
            
            _playerEngaged = true;
        }

        private IEnumerator ExitAction()
        {
            PlayerMovementController.Instance.PlayAnimation(exitAnimation, 0.4f, 1);
            yield return new WaitForSeconds(exitActionDelay);
            exitAction?.Invoke();
            yield return new WaitForSeconds(0.5f);
            PlayerMovementController.Instance.DisablePlayerMovement(false);
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            _playerEngaged = false;
            _enabled = true;
        }
    }
    
}
