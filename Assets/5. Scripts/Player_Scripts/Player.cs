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

        [SerializeField, BoxGroup("References")] private Animator animator;
        [SerializeField, BoxGroup("References")] private PlayerMovementController movementController;
        [SerializeField, BoxGroup("References")] private CharacterController characterController;
        [SerializeField, BoxGroup("References")] private PlayerEffectsManager effectsManager;
        [SerializeField, BoxGroup("References")] private PlayerHealth health;

        [SerializeField, BoxGroup("Player Movement")] private bool useHorizontal = true;
        [SerializeField, BoxGroup("Player Movement")] private float rotationSmoothness = 10f;
        [SerializeField, BoxGroup("Player Movement")] private float jumpVelocity = 6f;
        [SerializeField, BoxGroup("Player Movement")] private float jumpingForwardVelocity = 6f;

        [SerializeField, BoxGroup("Player Movement")] internal bool enabledDirectionInput;
        [SerializeField, BoxGroup("Player Movement")] private bool canJump;
        /// <summary>
        /// Used for sprint and other other boosing mechanisms
        /// </summary>
        [BoxGroup("Player Movement")] public bool canBoost;

        /// <summary>
        /// Resets the direction only to forward direction if not moving
        /// </summary>
        [SerializeField, BoxGroup("Player Movement")] internal bool oneWayRotation;


        [SerializeField, BoxGroup("Misc")]
        internal LayerMask groundMask;


        //STATES

        /// <summary>
        /// NOT AN ENUM. Absract class for current State
        /// </summary>
        internal PlayerBaseStates currentState;
        [SerializeField, BoxGroup("Player States")] internal BasicMovementSate basicMovementState = new BasicMovementSate();
        [SerializeField, BoxGroup("Player States")] internal LadderMovementState ladderMovementState = new LadderMovementState();
        [SerializeField, BoxGroup("Player States")] internal WaterMovement waterMovement = new WaterMovement();


        /// <summary>
        /// ENUM for current movement state
        /// </summary>
        [SerializeField, BoxGroup] internal PlayerMovementState e_currentState = PlayerMovementState.BasicMovement;
        [SerializeField, BoxGroup] internal int previousStateIndex = -1;
        [SerializeField, BoxGroup] internal int currentStateIndex = -1;


        [SerializeField] private bool disablePlayerMovement;

        internal Interactable interactable;
        internal bool isInteracting;


        #endregion


        #region Getter And Setters

        public Animator AnimationController => animator;
        public PlayerMovementController MovementController => movementController;
        public CharacterController CController => characterController;
        public PlayerHealth Health => health;
        public PlayerEffectsManager EffectsManager => effectsManager;

        public bool IsGrounded { get; set; }

        public bool CanJump => canJump;


        public bool DisabledPlayerMovement
        {
            get => disablePlayerMovement;
            set => disablePlayerMovement = value;
        }

        public bool UseHorizontal { get => useHorizontal; }

        public float JumpVelocity { get => jumpVelocity; }
        public float JumpForwardVelocity { get => jumpingForwardVelocity; }
        public float RotationSmoothness { get => rotationSmoothness; }



        #endregion

    }



    [System.Serializable]
    public enum PlayerMovementState
    {
        BasicMovement,
        Ladder,
        Water
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