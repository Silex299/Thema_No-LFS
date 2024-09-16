using System;
using System.Collections;
using UnityEngine;

namespace Misc
{
    public class RotationalVelocityEffectedSound : MonoBehaviour
    {

        public float updateInterval;
        public Vector3 axis;
        public float maximumRpm = 60;
        
        public AudioSource audioSource;
        [field: SerializeField] public float Rpm
        {
            get; private set;
        } 

        
        private float _lastCalculationTime;
        private Quaternion _lastRotation;


        private void Start()
        {
            axis.Normalize();
            _lastRotation = transform.rotation;
        }
        private void Update()
        {
            UpdateAngularVelocity();
        }
        private void UpdateAngularVelocity()
        {
            
            if(Time.time < _lastCalculationTime + updateInterval) return;
            //Calculate the angular velocity of the object around the axis
            
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(_lastRotation);
            deltaRotation.ToAngleAxis(out float angle, out axis);
            
            Rpm = (angle / 360) / (Time.time - _lastCalculationTime) * 60;
            _lastRotation = transform.rotation;
            _lastCalculationTime = Time.time;
            
            UpdateAudioSource();
        }

        private void UpdateAudioSource()
        {
            float volume = Mathf.Clamp(Rpm / maximumRpm, 0, 1);
            audioSource.volume = volume;
        }
        
        
        
    }
}
