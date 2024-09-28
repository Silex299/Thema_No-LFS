using System;
using Mechanics.Npc;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New
{
    public class V1Npc : MonoBehaviour
    {

        #region Variables


        #region Editor Exposed Variables
        
        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public ProximityDetection proximityDetection;
        
        [FoldoutGroup("Npc Movement Properties")] public float stopDistance = 1f;
        [FoldoutGroup("Npc Movement Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Movement Properties")] public float accelerationTime = 1;


        [FoldoutGroup("Path Finder")] public PathFinderBase pathFinder;
        [FoldoutGroup("Path Finder")] public Transform target;
        [FoldoutGroup("Path Finder")] public float targetOffset;
        [FoldoutGroup("Path Finder")] public float npcEyeHeight = 1.5f;
        [FoldoutGroup("Path Finder")] public float pathFindingInterval = 0.5f;
        
        [FoldoutGroup("States")] public NpcStates initState = NpcStates.Serveillance;
        [FoldoutGroup("States"), SerializeReference] public INpcBaseState idleState = new V1NpcIdleState();
        [FoldoutGroup("States"), SerializeReference] public INpcBaseState chaseState = new V1NpcChaseState();
        [FoldoutGroup("States"), SerializeReference] public INpcBaseState serveillanceState = new V1NpcServeillanceState();
        [FoldoutGroup("States"), SerializeReference] public INpcBaseState afterDeathState = new V1NpcAfterDeathState();
        
        #endregion

        #region Non Exposed Variables

        public Action onAttack;
        public Action<int> onStateChange;
        
        private INpcBaseState _currentStateRef;
        private NpcStates _currentStateEnum;
        private Transform _target;
        
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        #endregion
        

        #endregion

        #region Built in methods
        
        
        private void Start()
        {
            ChangeState((int) initState);
            _target = PlayerMovementController.Instance.transform;
        }
        private void OnEnable()
        {
            PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
        }
        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;
        }

        private void Update()
        {
            _currentStateRef.Update(this);
        }

        #endregion
        
        #region Custom Methods
        
        public void ChangeState(int stateIndex)
        {
            ChangeState((NpcStates) stateIndex);
        }
        public void ChangeState(NpcStates state)
        {
            if(_currentStateEnum == state) return;
            
            _currentStateRef.Exit(this);
            _currentStateEnum = state;
            
            _currentStateRef = state switch
            {
                NpcStates.Serveillance => serveillanceState,
                NpcStates.Chase => chaseState,
                NpcStates.AfterDeath => afterDeathState,
                NpcStates.Idle => idleState,
                _ => idleState,
            };

            _currentStateRef.Enter(this);

        }
        public void Reset()
        {
            ChangeState(initState);
            
            //Reset Animator
            animator.SetBool(Attack, false);
            animator.SetFloat(Speed, 0);
            animator.SetBool(PathBlocked, false);
            animator.SetInteger(StateIndex, 0);
            
        }
        private void OnPlayerDeath()
        {
            ChangeState(NpcStates.AfterDeath);
        }
        
        #endregion
        
        #region Types
        
        [System.Serializable]
        public enum NpcStates
        {
            None, //0
            Serveillance, //1
            Chase, //2
            AfterDeath, //3
            Idle //4
        }
        #endregion
        
    }
}
