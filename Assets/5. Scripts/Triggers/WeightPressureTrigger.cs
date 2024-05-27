using UnityEngine;

public class WeightPressureTrigger : MonoBehaviour
{
    
    [SerializeField] public float requiredMass = 50f; // Set the required mass to activate the trigger
    [SerializeField] private TMPro.TextMeshProUGUI pressureWeightText;
    
    private float _currentMass = 0f;
    private bool _isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        print("Hello");
        if (!other.CompareTag("Interactable")) return;
        
        Rigidbody rb = other.attachedRigidbody;
        
        if (rb == null) return;
        _currentMass  = Mathf.Clamp(_currentMass+ rb.mass, 0, 99999);
        CheckMass();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Interactable")) return;
        
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;
        
        _currentMass  = Mathf.Clamp(_currentMass - rb.mass, 0, 99999);
        UpdateWeightVisual();
    }

    private void CheckMass()
    {
        UpdateWeightVisual();
        if (_currentMass >= requiredMass && !_isActivated)
        {
            ActivateTrigger();
        }
    }

    private void ActivateTrigger()
    {
        _isActivated = true;
        Debug.Log("Pressure trigger activated!");
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
