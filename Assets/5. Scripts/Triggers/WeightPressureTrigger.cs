using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class WeightPressureTrigger : MonoBehaviour
    {
    
        [SerializeField, FoldoutGroup("Properties")] public float requiredMass = 50f; // Set the required mass to activate the trigger
        [SerializeField, FoldoutGroup("Properties")] private TMPro.TextMeshProUGUI pressureWeightText;

        
        [FoldoutGroup("Sounds")] public AudioSource soundSource;
        [FoldoutGroup("Sounds")] public float maximumWeightThreshold = 25;
        [FoldoutGroup("Sounds")] public SoundClip itemPlacedSound;
        [FoldoutGroup("Sounds")] public SoundClip itemRemovedSound;
        [FoldoutGroup("Sounds")] public SoundClip triggerSound;
        
        [FoldoutGroup("Events")] public UnityEvent activeTrigger;
        [FoldoutGroup("Events")] public UnityEvent deActiveTrigger;
    
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
                PlayItemUpdatedSound(true, rb.mass);
                UpdateWeightVisual();
                CheckPressure();
            }
            else if (other.CompareTag("Player_Main"))
            {
                _currentMass += 25f;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                PlayItemUpdatedSound(true, 25);
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
                PlayItemUpdatedSound(false, rb.mass);
                UpdateWeightVisual();
                CheckPressure();
            }
            else if (other.CompareTag("Player_Main"))
            {
                _currentMass -= 25f;
                _currentMass = Mathf.Clamp(_currentMass, 0, Mathf.Infinity);
                PlayItemUpdatedSound(false, 25);
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
                PlayTriggerSound();
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

        private void PlayItemUpdatedSound(bool placed, float weight)
        {
            if(!soundSource) return;
            
            if (placed && itemPlacedSound.clip && !_triggered)
            {
                soundSource.PlayOneShot(itemPlacedSound.clip, itemPlacedSound.volume * weight / maximumWeightThreshold);
            }
            else if(itemRemovedSound.clip && !_triggered)
            {
                soundSource.PlayOneShot(itemRemovedSound.clip, itemRemovedSound.volume * weight / maximumWeightThreshold);
            }
        }
        
        private void PlayTriggerSound()
        {
            if(!soundSource) return;
            soundSource.PlayOneShot(triggerSound.clip, triggerSound.volume);
        }
    
    }
}
