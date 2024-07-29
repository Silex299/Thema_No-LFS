using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class WeightPressureTrigger : MonoBehaviour
    {
    
        [SerializeField, BoxGroup("Properties")] public float requiredMass = 50f; // Set the required mass to activate the trigger
        [SerializeField, BoxGroup("Properties")] private TMPro.TextMeshProUGUI pressureWeightText;

        [BoxGroup("Events")] public UnityEvent activeTrigger;
        [BoxGroup("Events")] public UnityEvent deActiveTrigger;
    
        private float _currentMass = 0f;
        private bool _triggered;


        private void Start()
        {
            UpdateWeightVisual();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                Rigidbody rb = other.attachedRigidbody;
                _currentMass += rb.mass;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                UpdateWeightVisual();
                CheckPressure();
            }
            else if (other.CompareTag("Player_Main"))
            {
                _currentMass += 25f;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                UpdateWeightVisual();
                CheckPressure();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                Rigidbody rb = other.attachedRigidbody;
                _currentMass -= rb.mass;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                UpdateWeightVisual();
                CheckPressure();
            }
            else if (other.CompareTag("Player_Main"))
            {
                _currentMass -= 25f;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                UpdateWeightVisual();
                CheckPressure();
            }
        }



        private void CheckPressure()
        {
            if (!_triggered && _currentMass >= requiredMass)
            {
                activeTrigger.Invoke();
                _triggered = true;
            }
            else if(_triggered && _currentMass < requiredMass)
            {
                deActiveTrigger.Invoke();
                _triggered = false;
            }
        }
    
    
        private void UpdateWeightVisual()
        {
            if (_currentMass == 0)
            {
                pressureWeightText.text =  "00\n-\n" + requiredMass;
            }
            else
            {
                pressureWeightText.text = Mathf.Clamp(_currentMass, 0, requiredMass) + "\n-\n" + requiredMass;
            }
        
        }
    
    }
}
