using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class SimpleSwitch : MonoBehaviour
    {


        [SerializeField, BoxGroup("Trigger")] private bool isEnabled;
        [SerializeField, BoxGroup("Trigger")] private string triggerString;
        [SerializeField, BoxGroup("Trigger")] private float secondActionDelay;
        [SerializeField, BoxGroup("Trigger")] private UnityEvent<bool> action;


        [SerializeField, BoxGroup("Visual")] private int materialIndex;
        [SerializeField, BoxGroup("Visual")] private Material triggeredMaterial;
        [SerializeField, BoxGroup("Visual")] private Material defaultMaterial;



        private bool _playerIsInTrigger;
        private bool _triggered;
        private float _lastTriggerTime;

        private void OnTriggerEnter(Collider other)
        {
            if (!isEnabled) return;
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

            if (!isEnabled) return;

            if (Input.GetButtonDown(triggerString))
            {
                if (_lastTriggerTime + secondActionDelay < Time.time)
                {
                    action?.Invoke(_triggered);
                    _lastTriggerTime = Time.time;
                    _triggered = !_triggered;
                    UpdateSwitchVisual(_triggered);
                }
            }
        }

        public void UpdateSwitchVisual(bool status)
        {
            _triggered = status;

            var renderer = GetComponent<MeshRenderer>();
            var materials = renderer.materials;

            if (status)
            {
                materials[materialIndex] = triggeredMaterial;
            }
            else
            {
                materials[materialIndex] = defaultMaterial;
            }

            renderer.materials = materials;
        }



    }

}