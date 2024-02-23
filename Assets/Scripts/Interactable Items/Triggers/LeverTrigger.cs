using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Interactable_Items.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class LeverTrigger : MonoBehaviour
    {
        [SerializeField, BoxGroup("References")]
        private Animator animatorController;

        [SerializeField] private Transform actionTransform;
        [SerializeField] private float actionSmoothness = 10;
        [SerializeField] private float triggerActionDelay = 0.5f;

        [FormerlySerializedAs("TriggerString")] [SerializeField] private string triggerString;

        [SerializeField, BoxGroup("Event")] private UnityEvent onTrigger;
        [SerializeField, BoxGroup("Event")] private UnityEvent onTriggerReset;

        private bool _isPlayerInTrigger;
        private bool _moveToPosition;

        [SerializeField, Space(20)] private bool triggerEnable;
        private bool _triggerIsOn;
        public void EnableTrigger(bool status)
        {
            triggerEnable = status;
            
            if (_triggerIsOn && !triggerEnable)
            {
                ResetTrigger();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = false;
            }
        }

        private void Update()
        {
            if (!_isPlayerInTrigger) return;

            if (Input.GetButtonDown(triggerString))
            {
                StartCoroutine(PlayLeverAction());
            }
        }

        private void LateUpdate()
        {
            if (!_moveToPosition) return;
            
            
            Transform playerTransform;
                
            (playerTransform = PlayerController.Instance.transform).position = Vector3.Lerp(
                PlayerController.Instance.transform.position, actionTransform.position,
                Time.deltaTime * actionSmoothness);
                
            PlayerController.Instance.transform.rotation = Quaternion.Lerp(
                playerTransform.rotation, actionTransform.rotation, Time.deltaTime * actionSmoothness);
        }

        private IEnumerator PlayLeverAction()
        {
            if(_triggerIsOn) yield break;
            
            
            //move player to correct position
            _moveToPosition = true;
            PlayerController.Instance.Player.PlayerController.enabled = false;

            yield return new WaitForSeconds(0.5f);
            _moveToPosition = false;

            // ReSharper disable once Unity.InefficientPropertyAccess
            PlayerController.Instance.Player.PlayerController.enabled = true;
            
            
            PlayerController.Instance.CrossFadeAnimation("Pull Lever");
            yield return new WaitForSeconds(0.5f);

            _triggerIsOn = true;
            animatorController.Play("Trigger");

            if (!triggerEnable)
            {
                yield return new WaitForSeconds(1f);
                ResetTrigger();
            }
            else
            {
                yield return new WaitForSeconds(triggerActionDelay);
                onTrigger?.Invoke();
            }
        }

        public void ResetTrigger()
        {
            onTriggerReset?.Invoke();
            animatorController.Play("Reset Trigger");
            _triggerIsOn = false;
        }
    }
}
