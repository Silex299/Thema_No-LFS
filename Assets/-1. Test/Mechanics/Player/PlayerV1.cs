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

        public bool DisableAllMovement { get; set; }
        public bool DisableInput { get; set; }
        public bool DisableAnimationUpdate { get; set; }

        
        
        public Vector3 PlayerVelocity { get; private set; }
        private float _lastCalculationTime;
        private Vector3 _lastPosition;

        private void Update()
        {
            CalculateVelocity();
            
            if(DisableAllMovement) return;
            
            controller.UpdateController(this);
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
