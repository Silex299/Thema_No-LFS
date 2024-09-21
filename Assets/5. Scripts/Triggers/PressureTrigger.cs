using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class PressureTrigger : MonoBehaviour
    {

        [SerializeField] private List<string> tags;
        [SerializeField] private bool canTrigger = true;
        [SerializeField, Range(1, 30)] private int pressureThreshold = 1;
        [SerializeField] private UnityEvent triggerAction;
        [SerializeField] private UnityEvent resetAction;


        [SerializeField, Space(10)] private AudioSource soundSource;
        [SerializeField, Space(10)] private AudioClip triggerSound;

        [field: SerializeField] public int ObjectInTrigger { get; set; }
        [field: SerializeField] public bool Triggered { get; set; }

        private void OnTriggerEnter(Collider other)
        {

            if (!canTrigger) return;

            if (!tags.Contains(other.tag)) return;
            
            ObjectInTrigger = ObjectInTrigger + 1;
            if (Triggered) return;

            if(ObjectInTrigger >= pressureThreshold)
            {
                Triggered = true;
                triggerAction?.Invoke();
                soundSource?.PlayOneShot(triggerSound);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!canTrigger) return;
            if (!tags.Contains(other.tag)) return;

            ObjectInTrigger = Mathf.Clamp(ObjectInTrigger-1, 0, 30);

            if (!Triggered) return;

            if(ObjectInTrigger < pressureThreshold)
            {
                Triggered = false;
                resetAction?.Invoke();
            }
        }


    }
}
