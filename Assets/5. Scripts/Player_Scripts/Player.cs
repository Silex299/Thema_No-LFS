using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Player_Scripts.States;
using Player_Scripts.Interactables;
using Health;

namespace Player_Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        #region Variables

        [SerializeField, BoxGroup("References")]
        private Animator animator;

        [SerializeField, BoxGroup("References")]
        private PlayerMovementController movementController;

        [SerializeField, BoxGroup("References")]
        private CharacterController characterController;

        [SerializeField, BoxGroup("References")]
        private PlayerEffectsManager effectsManager;

        [SerializeField, BoxGroup("References")]
        private PlayerHealth health;

        [field: SerializeField, BoxGroup("References")]
        public PlayerRigController RigController { get; private set; }



        [SerializeField, BoxGroup("Player Movement")]
        private bool useHorizontal = true;

        [SerializeField, BoxGroup("Player Movement")]
        private float rotationSmoothness = 10f;

        [SerializeField, BoxGroup("Player Movement")]
        private float jumpVelocity = 6f;

        [SerializeField, BoxGroup("Player Movement")]
        private float jumpingForwardVelocity = 6f;

        [SerializeField, BoxGroup("Player Movement")]
        internal bool enabledDirectionInput;

        [SerializeField, BoxGroup("Player Movement")]
        private bool canJump;

        [SerializeField, BoxGroup("Player Movement")]
        private bool canRotate;
        /// <summary>
        /// Used for sprint and other boosting mechanisms
        /// </summary>
        [SerializeField, BoxGroup("Player Movement")]
        private bool canBoost;
        
        /// <summary>
        /// Used for sprint and other boosting mechanisms
        /// </summary>
        [SerializeField, BoxGroup("Player Movement")]
        private bool forceBoost;

        /// <summary>
        /// Resets the direction only to forward direction if not moving
        /// </summary>
        [SerializeField, BoxGroup("Player Movement")]
        internal bool oneWayRotation;


        [SerializeField, BoxGroup("PlayerRaycast")]
        internal LayerMask groundMask;

        [SerializeField, BoxGroup("PlayerRaycast")]
        internal float sphereCastRadius;

        [SerializeField, BoxGroup("PlayerRaycast")]
        internal float sphereCastOffset;

        [SerializeField, BoxGroup("PlayerRaycast")]
        internal float groundOffset;


        //MARKER : States
        /// <summary>
        /// NOT AN ENUM. Abstract class for current State
        /// </summary>
        internal PlayerBaseStates currentState;

        [SerializeField, BoxGroup("Player States")]
        internal BasicMovementSate basicMovementState = new BasicMovementSate();

        [SerializeField, BoxGroup("Player States")]
        internal LadderMovementState ladderMovementState = new LadderMovementState();

        [SerializeField, BoxGroup("Player States")]
        internal WaterMovement waterMovement = new WaterMovement();

        [SerializeField, BoxGroup("Player States")]
        internal RopeMovement ropeMovement = new RopeMovement();

        [SerializeField, BoxGroup("Player States"), Space(10)]
        internal FreeBasicMovement freeBasicMovement = new FreeBasicMovement();


        /// <summary>
        /// ENUM for current movement state
        /// </summary>
        [SerializeField, FoldoutGroup("Other")] internal PlayerMovementState eCurrentState = PlayerMovementState.BasicMovement;

        [SerializeField, FoldoutGroup("Other")] internal int previousStateIndex = -1;
        [SerializeField, FoldoutGroup("Other")] internal int currentStateIndex = -1;


        [SerializeField, FoldoutGroup("Other")] private bool disablePlayerMovement;

        internal Interactable interactable;
        internal bool isInteracting;
        internal Vector3 playerVelocity = Vector3.zero;

        #endregion


        #region Getter And Setters

        public Animator AnimationController => animator;
        public PlayerMovementController MovementController => movementController;
        public CharacterController CController => characterController;
        public PlayerHealth Health => health;
        public PlayerEffectsManager EffectsManager => effectsManager;

        
        public float JumpVelocity => jumpVelocity;
        public float JumpForwardVelocity => jumpingForwardVelocity;
        public float RotationSmoothness => rotationSmoothness;
        
        [field: SerializeField, FoldoutGroup("Other")]public string GroundTag { get; set; }

        #region Flags
        
        public bool OverrideFlags { get; set; }
        public bool CanPlayAlternateMovement { get; set; } = true;

        public bool IsOverridingAnimation => !AnimationController.GetCurrentAnimatorStateInfo(1).IsName("Default");

        public bool IsGrounded { get; set; }

        public bool CanJump
        {
            get => canJump;
            set
            {
                if (!OverrideFlags)
                {
                    canJump = value;
                }
            }
        }

        public bool CanBoost
        {
            get => canBoost;
            set
            {
                if (!OverrideFlags)
                {
                    canBoost = value;
                }
            }
        }

        public bool CanRotate
        {
            get => canRotate;
            set
            {
                if (!OverrideFlags)
                {
                    canRotate = value;
                }
            }
        }

        public bool ForceBoost
        {
            get => forceBoost;
            set
            {
                if(!OverrideFlags)
                {
                    forceBoost = value;
                }
            }
        }

        public bool OneWayRotation
        {
            set => oneWayRotation = value;
        }

        public bool DisabledPlayerMovement
        {
            get => disablePlayerMovement;
            set => disablePlayerMovement = value;
        }

        public bool UseHorizontal
        {
            get => useHorizontal;
        }
        
        #endregion


        #endregion
    }


    [System.Serializable]
    public enum PlayerMovementState
    {
        //MARKER : State
        BasicMovement,
        Ladder,
        Water,
        Rope,
        FreeBasicMovement
    }

    /// <summary>
    /// Player Interaction like Push and Trigger and all
    /// </summary>
    [System.Serializable]
    public enum PlayerInteractionType
    {
        NONE,
        PUSH,
    }
}