using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class VelocityTrigger : MonoBehaviour
    {
        
        
        public float velocityThreshold = 1;
        public float checkInterval = 0.5f;
        [field: SerializeField] public bool IsEnabled { get; set; } = true;

        public UnityEvent trigger;
        private Vector3 _lastPosition;


        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (!IsEnabled) return;
            
            if(_lastPosition == transform.position) return;
            
            var velocity = (transform.position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;
            
            if(Mathf.Abs(velocity.magnitude)> velocityThreshold)
            {
                Debug.LogError(velocity.magnitude);
                print("TRIGGERED");
                trigger?.Invoke();
            }

        }
    }
}
