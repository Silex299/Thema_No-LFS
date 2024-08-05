using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player
{
    public class PlayerV1 : MonoBehaviour
    {
        [FoldoutGroup("References")] public CharacterController characterController;
        [FoldoutGroup("References")] public Animator animator;


        [FoldoutGroup("Misc")] public bool isGrounded;
        [FoldoutGroup("Misc")] public bool isInGroundProximity;
        [FoldoutGroup("Misc")] public Controller controller;

        public Vector3 PlayerVelocity { get; private set; }
        private float _lastCalculationTime;
        private Vector3 _lastPosition;

        private void Update()
        {
            controller.UpdateController(this);
            CalculateVelocity();
        }
        
        private void CalculateVelocity()
        {
            if (Time.time - _lastCalculationTime < 0.2f) return;
        
            PlayerVelocity = (transform.position - _lastPosition) / (Time.time - _lastCalculationTime);
            _lastPosition = transform.position;
            _lastCalculationTime = Time.time;
        }
        
        
        
    }
}
