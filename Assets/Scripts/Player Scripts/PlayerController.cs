using Player_Scripts.Player_States;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Player_Scripts
{

    [RequireComponent(typeof(PlayerVariables))]
    public class PlayerController : MonoBehaviour
    {

        #region Variables

        public static PlayerController Instance;

        [SerializeField] private PlayerVariables playerVariables;
        public PlayerStates initState;


        #region States

        private PlayerBaseState _currentState;

        [SerializeField, FoldoutGroup("States")] private BasicRestrictedMovement basicRestrictedMovement = new BasicRestrictedMovement();
        [SerializeField, FoldoutGroup("States")] private BasicFreeMovement basicFreeMovement = new BasicFreeMovement();
        [SerializeField, FoldoutGroup("States")] private StealthRestrictedMovement stealthRestrictedMovement = new StealthRestrictedMovement();
        [SerializeField, FoldoutGroup("States")] private LadderMovement ladderMovement = new LadderMovement();
        [SerializeField, FoldoutGroup("States")] private UnderWaterMovement underWaterMovement = new UnderWaterMovement();
        [SerializeField, FoldoutGroup("States")] private RestrictedUnderWaterMovement restrictedUnderWaterMovement = new RestrictedUnderWaterMovement();
        [SerializeField, FoldoutGroup("States")] public RopeMovement ropeMovement = new RopeMovement();

        #endregion

        #region events

        public event Action<string> AnimationCall;

        #endregion

        public PlayerVariables Player => playerVariables;
        
        #endregion


        #region Built in methods

        private void Awake()
        {
            if (PlayerController.Instance == null)
            {
                PlayerController.Instance = this;
            }
            else if (PlayerController.Instance != this)
            {
                Destroy(PlayerController.Instance);
            }
        }

        private void Start()
        {
            ChangeState(initState);
        }

        private void Update()
        {
            if (!playerVariables.CanMove
            ) return;

            _currentState?.OnStateUpdate(this);
        }

        private void FixedUpdate()
        {
            if (!playerVariables.CanMove
            ) return;

            _currentState?.OnStateFixedUpdate(this);
        }

        private void LateUpdate()
        {
            if (!playerVariables.CanMove
            ) return;

            _currentState?.OnStateLateUpdate(this);
        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                switch (initState)
                {
                    case PlayerStates.LadderMovement:
                        ladderMovement.OnGizmos(this);
                        break;
                    case PlayerStates.StealthMovement:
                        stealthRestrictedMovement.OnGizmos(this);
                        break;
                    case PlayerStates.UnderWaterMovement:
                        underWaterMovement.OnGizmos(this);
                        break;
                    case PlayerStates.RestrictedUnderWaterMovement:
                        restrictedUnderWaterMovement.OnGizmos(this);
                        break;
                    case PlayerStates.RopeMovement:
                        ropeMovement.OnGizmos(this);
                        break;
                    default:
                        basicRestrictedMovement.OnGizmos(this);
                        break;
                }

            }
            else
            {
                _currentState?.OnGizmos(this);
            }
        }

        #endregion

        #region Custom methods

        public void SimpleInteraction(int i)
        {
            _currentState?.SimpleInteract(this, i);
        }
        public bool Interact(PlayerBaseState.InteractionType type, bool value = false)
        {
            return _currentState.Interact(this, type, value) && playerVariables.CanMove;
        }

        public void ChangeState(PlayerStates state, int index = 0, float transitionTime = 0.2f)
        {
            _currentState?.OnStateExit(this);

            _currentState = state switch
            {
                PlayerStates.BasicRestrictedMovement => basicRestrictedMovement,
                PlayerStates.StealthMovement => stealthRestrictedMovement,
                PlayerStates.BasicFreeMovement => basicFreeMovement,
                PlayerStates.LadderMovement => ladderMovement,
                PlayerStates.UnderWaterMovement => underWaterMovement,
                PlayerStates.RestrictedUnderWaterMovement => restrictedUnderWaterMovement,
                PlayerStates.RopeMovement => ropeMovement,
                _ => basicRestrictedMovement
            };

            _currentState.OnStateEnter(this, index, transitionTime);

            initState = state;
        }



        public void CrossFadeAnimation(string animationName)
        {
            playerVariables.AnimationController.CrossFade(animationName, 0.5f, 1);
            playerVariables.CanMove
             = false;
            playerVariables.canRotate = false;
            playerVariables.PlayerController.enabled = false;
        }

        public void CrossFadeAnimation(string animationName, float normalizedDuration)
        {
            playerVariables.AnimationController.CrossFade(animationName, normalizedDuration, 1);
            playerVariables.PlayerController.enabled = false;
            playerVariables.canRotate = false;
        }

        public void AnimationComplete()
        {
            playerVariables.canRotate = true;
            playerVariables.CanMove
             = true;
            playerVariables.PlayerController.enabled = true;
        }

        public void AnimationGlobalCall(string message)
        {
            AnimationCall?.Invoke(message);
        }

        #endregion


        #region Custom types

        [System.Serializable]
        public enum PlayerStates
        {
            BasicRestrictedMovement, //0
            StealthMovement, //1
            LadderMovement, //2
            BasicFreeMovement, //3
            UnderWaterMovement, //4
            RestrictedUnderWaterMovement, // 5
            RopeMovement //6
        }

        #endregion


    }
}
