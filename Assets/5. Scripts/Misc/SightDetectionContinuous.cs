using System.Collections.Generic;
using Health;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class SightDetectionContinuous : MonoBehaviour
    {
        
        
        public string[] detectionTags;
        public UnityEvent<Collider> onDetection;
        [field: SerializeField] public bool IsEnabled { get; set; } = true;


        private readonly Dictionary<int, HealthBaseClass> _detectedInstanceId = new Dictionary<int, HealthBaseClass>();
        
        private void OnTriggerStay(Collider other)
        {
            if (!IsEnabled) return;
            
            if (detectionTags.Length == 0) return;
            
            foreach (var detectionTag in detectionTags)
            {
                if (other.CompareTag(detectionTag))
                {
                    if(_detectedInstanceId.ContainsKey(other.GetInstanceID()))
                    {
                        if(!_detectedInstanceId[other.GetInstanceID()].IsDead)
                        {
                            onDetection?.Invoke(other);
                        }
                    }
                    else if(other.TryGetComponent(out HealthBaseClass health))
                    {
                        _detectedInstanceId.Add(other.GetInstanceID(), health);
                        onDetection?.Invoke(other);
                    }
                }
            }
        }
        
        
        
        
        
    }
}
