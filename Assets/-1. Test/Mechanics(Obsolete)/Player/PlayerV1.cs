using Mechanics.Player.Controllers;
using Mechanics.Player.Interactable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player
{
    public class PlayerV1 : MonoBehaviour
    {
        [FoldoutGroup("References")] public CharacterController characterController;
        [FoldoutGroup("References")] public Animator animator;


        [FoldoutGroup("Properties")] public float groundDistance = 0.4f;
        [FoldoutGroup("Properties")] public float proximityThreshold = 1f;
        [FoldoutGroup("Properties")] public LayerMask groundMask;

        [FoldoutGroup("Misc")] public Controller controller;


        #region Flags

        #region Status Flags
        
        public bool IsGrounded { get; private set; }
        private bool IsInProximity { get; set; }
        public bool InWater { get; set; }
        public bool AtSurface { get; set; }
        public bool AtBottom { get; set; }

        public bool IsInteracting { get; set; }
        
        #endregion

        #region Movement Flags

        public bool DisableAllMovement { get; set; }
        public bool DisableInput { get; set; }
        public bool CanJump { get; set; } = true;
        public bool CanBoost { get; set; } = true;
        public bool CanAltMovement { get; set; } = true;
        public bool CanInteract { get; set; } = true;

        public bool Boost { get; set; }
        private bool _altMovement;

        public bool AltMovement
        {
            get => _altMovement;
            set
            {
                _altMovement = value;
                animator.SetBool(AltMovementInt, value);
            }
        }

        #endregion

        #endregion


        public InteractableBase Interactable { get; set; }

        public Controller MovementController
        {
            get => controller;

            set
            {
                if (value == controller) return;

                controller.ControllerExit(this);
                controller = value;
                value.ControllerEnter(this);
            }
        }

        #region Private and Static variables

        public Vector3 PlayerVelocity { get; private set; }
        private float _lastCalculationTime;
        private Vector3 _lastPosition;
        private Vector3 _desiredMoveDirection;
        private static readonly int IsGroundedInt = Animator.StringToHash("IsGrounded");
        private static readonly int IsInProximityInt = Animator.StringToHash("IsInProximity");
        private static readonly int AltMovementInt = Animator.StringToHash("AltMovement");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        private void Update()
        {
            CalculateVelocity();

            if (DisableAllMovement) return;
            controller.ControllerUpdate(this);
            if (CanInteract) Interactable?.InteractionUpdate(this);
        }

        private void FixedUpdate()
        {
            if (DisableAllMovement) return;
            controller.ControllerFixedUpdate(this);
            if (CanInteract) Interactable?.InteractionFixedUpdate(this);
        }

        private void LateUpdate()
        {
            if (DisableAllMovement) return;
            controller.ControllerLateUpdate(this);
            if (CanInteract) Interactable?.InteractionLateUpdate(this);
        }


        private void CalculateVelocity()
        {
            if (Time.time - _lastCalculationTime < 0.2f) return;

            PlayerVelocity = (transform.position - _lastPosition) / (Time.time - _lastCalculationTime);
            _lastPosition = transform.position;
            _lastCalculationTime = Time.time;
        }

        public void AddForce(Vector3 force)
        {
            _desiredMoveDirection = force;
        }

        public void ApplyGravity(float gravity = -9.8f)
        {
            GroundCheck();
            if (IsGrounded && _desiredMoveDirection.y < 0)
            {
                _desiredMoveDirection = new Vector3(0, -2f, 0);
            }

            _desiredMoveDirection.y += gravity * Time.deltaTime;

            characterController.Move((_desiredMoveDirection) * Time.deltaTime);
        }

        public void GroundCheck()
        {
            IsGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
            IsInProximity = Physics.CheckSphere(transform.position, proximityThreshold, groundMask);

            animator.SetBool(IsGroundedInt, IsGrounded);
            animator.SetBool(IsInProximityInt, IsInProximity);


            //REMOVE
            Debug.DrawLine(transform.position,
                transform.position + Vector3.down * (IsGrounded ? groundDistance : proximityThreshold),
                IsGrounded ? Color.green : IsInProximity ? Color.blue : Color.red, 1f);
        }

        public void ResetAnimator()
        {
            animator.ResetTrigger(Jump);
            animator.SetBool(AltMovementInt, false);
            animator.SetFloat(Speed, 0);
        }

        public void ResetMovement()
        {
            _desiredMoveDirection = Vector3.zero;
            ResetAnimator();
        }
    }
}