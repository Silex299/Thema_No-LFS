using System;
using UnityEngine;

public class WeightPressureTrigger : MonoBehaviour
{
    
    [SerializeField] public float requiredMass = 50f; // Set the required mass to activate the trigger
    [SerializeField] private TMPro.TextMeshProUGUI pressureWeightText;
    
    private float _currentMass = 0f;
    private bool _isActivated = false;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Rigidbody rb = other.attachedRigidbody;
            _currentMass += rb.mass;
            _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
            UpdateWeightVisual();
        }
        else if (other.CompareTag("Player_Main"))
        {
            _currentMass += 25f;
            _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
            UpdateWeightVisual();
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
        }
        else if (other.CompareTag("Player_Main"))
        {
            _currentMass -= 25f;
            _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
            UpdateWeightVisual();
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
