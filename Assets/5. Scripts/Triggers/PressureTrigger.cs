using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class PressureTrigger : MonoBehaviour
    {

        [SerializeField] private bool canTrigger = true;
        [SerializeField, Range(1, 30)] private int pressureThreshold = 1;
        [SerializeField] private UnityEvent triggerAction;
        [SerializeField] private UnityEvent resetAction;


        [SerializeField, Space(10)] private AudioSource soundSource;
        [SerializeField, Space(10)] private AudioClip triggerSound;

        private int _objectInTrigger;
        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {

            if (!canTrigger) return;

            _objectInTrigger = _objectInTrigger + 1;
            if (_triggered) return;

            if(_objectInTrigger >= pressureThreshold)
            {
                _triggered = true;
                triggerAction?.Invoke();
                soundSource?.PlayOneShot(triggerSound);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!canTrigger) return;

            _objectInTrigger = Mathf.Clamp(_objectInTrigger-1, 0, 30);

            if (!_triggered) return;

            if(_objectInTrigger < pressureThreshold)
            {
                _triggered = false;
                resetAction?.Invoke();
            }
        }



    }
}
