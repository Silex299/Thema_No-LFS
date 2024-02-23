
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Player_Scripts
{
    public class PlayerVariables : MonoBehaviour
    {


        [TabGroup("Movement Variables", "Ground Check")]
        public float sphereCastRadius;
        [TabGroup("Movement Variables", "Ground Check")]
        public float sphereCastOffset;
        [TabGroup("Movement Variables", "Ground Check")]
        public float groundOffset;
        [TabGroup("Movement Variables", "Ground Check")]
        public LayerMask raycastMask;


        [TabGroup("Movement Variables", "Proximity Variables")]
        public float raycastDistance;
        [TabGroup("Movement Variables", "Proximity Variables")]
        public float firstRaycastHeight;
        [TabGroup("Movement Variables", "Proximity Variables")]
        public float secondRaycastHeight;
        [TabGroup("Movement Variables", "Proximity Variables")]
        public float verticalRaycastOffset;
        [TabGroup("Movement Variables", "Proximity Variables")]
        public float verticalRaycastDistance;


        [TabGroup("Movement Variables", "Misc")]
        public Vector3 climbOffset;
        [TabGroup("Movement Variables", "Misc")]
        public float climbSmoothness;

        [FoldoutGroup("References"), SerializeField] private CharacterController characterController;
        [FoldoutGroup("References"), SerializeField] private Animator animationController;
        [FoldoutGroup("References"), SerializeField] private PlayerHealth health;
        [FoldoutGroup("References"), SerializeField] private PlayerRigController rig;



        [FoldoutGroup("Utility", false), SerializeField] private bool canMove;

        public bool CanMove
        {
            set
            {
                canMove = value;
                animationController.SetFloat("Speed", 0);
            }
            get => canMove;
        }
        
        [FoldoutGroup("Utility")] public bool canRotate = true;
        [FoldoutGroup("Utility")] public bool invertedAxis;

        [FoldoutGroup("Effects")] public VisualEffect bloodSpill;
        [FoldoutGroup("Effects")] public Material mainMaterial;

        #region Getters

        public CharacterController PlayerController => characterController;
        public Animator AnimationController => animationController;

        public PlayerHealth Health => health;
        public PlayerRigController Rig => rig;
        [HideInInspector] public bool canSprint = true;

        #endregion

        #region Custom methods

        public void InvertAxis(bool value)
        {
            invertedAxis = value;
        }

        #endregion

    }
}

