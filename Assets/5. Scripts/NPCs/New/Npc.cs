using System;
using System.Collections.Generic;
using Mechanics.Npc;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New
{
    public class Npc : MonoBehaviour
    {

        #region Variables

        #region Common Properties
        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public ProximityDetection proximityDetection;
        
        
        [FoldoutGroup("Npc Properties")] public float stopDistance = 1f;
        [FoldoutGroup("Npc Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Properties")] public float accelerationTime = 1;
        
        
        [FoldoutGroup("Path Finder")] public PathFinderBase pathFinder;
        [FoldoutGroup("Path Finder")] public Transform target;
        [FoldoutGroup("Path Finder")] public float targetOffset;
        [FoldoutGroup("Path Finder")] public float npcEyeHeight = 1.5f;
        [FoldoutGroup("Path Finder")] public float pathFindingInterval = 0.5f;
        
        #endregion
        
        #region Seveillance
        [FoldoutGroup("Serveillance")] public List<Vector3> serveillancePoints;
        [FoldoutGroup("Serveillance")] public float serveillanceWaitTime;
        [FoldoutGroup("Serveillance")] public String entryAnim;
        #endregion
        #region Chase
        
        [FoldoutGroup("Chase")] public float attackDistance;
        [FoldoutGroup("Chase")] public bool returnToServeillanceOnTargetLost;
        [FoldoutGroup("Chase"), ShowIf(nameof(returnToServeillanceOnTargetLost))] public float returnInterval;
        public bool CanAttack { get; set; } = true;
        
        
        #endregion
        
        #region After Death
        [FoldoutGroup("After Death")] public bool serveillanceAfterDeath;
        #endregion
        
        #region States

        private NpcStateBase _currentState;
        private NpcStates _currentStateType;
        private readonly NpcServeillanceState _serveillanceState = new NpcServeillanceState();
        private readonly NpcChaseState _chaseState = new NpcChaseState();
        private readonly NpcAfterDeathState _afterDeath = new NpcAfterDeathState();

        #region Events
        
        internal Action onAttack;
        internal Action<int> onStateChange;
        
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        #endregion

        #endregion

        #region Getter Setter

        public float SurveillanceWaitTime
        {
            set => serveillanceWaitTime = value;
        }

        #endregion
        
        #endregion
        
        #region Editor
#if UNITY_EDITOR

        [Button]
        public void GetWaypoints(List<Transform> points)
        {
            serveillancePoints = new List<Vector3>();
            foreach (var point in points)
            {
                serveillancePoints.Add(point.position);
            }
        }
#endif
        #endregion


        private void Start()
        {
            ChangeState(NpcStates.Serveillance);
            PlayerMovementController.Instance.player.Health.onDeath += OnDeath;
            target = PlayerMovementController.Instance.transform;
        }

        private void OnDeath()
        {
            print("Calling Npc after death");
            ChangeState(NpcStates.AfterDeath);
        }

        private void Update()
        {
            _currentState?.Update();
        }
        
        public void ChangeState(int stateIndex)
        {
            ChangeState((NpcStates)stateIndex);
        }
        private void ChangeState(NpcStates state)
        {
            
            if(state == _currentStateType) return;
            
            _currentState?.Exit();
            _currentStateType = state;
            
            _currentState = state switch
            {
                NpcStates.Serveillance => _serveillanceState,
                NpcStates.Chase => _chaseState,
                NpcStates.AfterDeath => _afterDeath,
            };
            
            _currentState.Enter(this);
            onStateChange?.Invoke((int)_currentStateType);
        }

        public void Reset()
        {
            ChangeState(NpcStates.Serveillance);
            
            //Reset Animator
            animator.SetBool(Attack, false);
            animator.SetFloat(Speed, 0);
            animator.SetBool(PathBlocked, false);
            animator.SetInteger(StateIndex, 0);
            
        }
        
        #region States

        enum NpcStates
        {
            None,
            Serveillance,
            Chase,
            AfterDeath
        }
        
        #endregion
    }
    
}
