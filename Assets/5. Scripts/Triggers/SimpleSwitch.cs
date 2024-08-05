using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class SimpleSwitch : MonoBehaviour
    {
        
        [SerializeField, BoxGroup("Trigger")] private string triggerString;
        [SerializeField, BoxGroup("Trigger")] private float secondActionDelay;

        [SerializeField, BoxGroup("Visual")] private MeshRenderer meshRenderer;
        [SerializeField, BoxGroup("Visual")] private int materialIndex;
        [SerializeField, BoxGroup("Visual")] private Material triggeredMaterial;
        [SerializeField, BoxGroup("Visual")] private Material defaultMaterial;


        [SerializeField, BoxGroup("Sound")] private AudioSource source;
        [SerializeField, BoxGroup("Sound")] private AudioClip triggerClip;


        [BoxGroup("Events")] public UnityEvent triggeredEvents;
        [BoxGroup("Events")] public UnityEvent unTriggeredEvents;


        private bool _playerIsInTrigger;
        private bool _triggered;
        private float _lastTriggerTime;

        public bool Triggered
        {
            get => _triggered;
            set => _triggered = value;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = false;
            }
        }


        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if (!enabled) return;

            if (Input.GetButtonDown(triggerString))
            {
                Trigger(!Triggered);
            }
        }


        private void Trigger(bool triggered)
        {
            print("fuck me");
            if (!(_lastTriggerTime + secondActionDelay < Time.time)) return;
            
            if(Triggered == triggered) return;
            
            Triggered = triggered;
                
            if (_triggered)
            {
                triggeredEvents.Invoke();
            }
            else
            {
                unTriggeredEvents.Invoke();
            }
                
            UpdateSwitch(Triggered);
            TriggerSound();
            
            _lastTriggerTime = Time.time;
        }
        
        
        public void UpdateSwitch(bool status)
        {
            if (!meshRenderer)
            {
                if(!TryGetComponent(out meshRenderer)) return;
            }

            var materials = meshRenderer.materials;
            
            
            if (status)
            {
                materials[materialIndex] = triggeredMaterial;
            }
            else
            {
                materials[materialIndex] = defaultMaterial;
            }

            meshRenderer.materials = materials;
        }

        private void TriggerSound()
        {
            if (source)
            {
                source.PlayOneShot(triggerClip);
            }
        }


    }

}