using System;
using System.Collections.Generic;
using NPCs.New;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Npc
{
    public class Npc : MonoBehaviour
    {

        #region Variables

        #region Common Properties
        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public ProximityDetection proximityDetection;
        //[FoldoutGroup("References")] public InfectedRigController infectedRigController;
        
        
        [FoldoutGroup("Npc Properties")] public float stopDistance = 1f;
        [FoldoutGroup("Npc Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Properties")] public float accelerationTime = 1;
        #endregion
        
        #region Seveillance
        [FoldoutGroup("Serveillance")] public List<Vector3> serveillancePoints;
        [FoldoutGroup("Serveillance")] public float serveillanceWaitTime;
        [FoldoutGroup("Serveillance")] public String entryAnim;
        #endregion
        #region Chase
        [FoldoutGroup("Path Finder")] public CustomPathFinderBase pathFinder;
        [FoldoutGroup("Path Finder")] public float npcEyeHeight = 1.5f;
        [FoldoutGroup("Path Finder")] public float pathFindingInterval = 0.5f;
        
        [FoldoutGroup("Chase")] public float attackDistance;
        internal Action onAttack;
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

        [Button]
        public void TestPath(Color color)
        {
            var path = pathFinder.GetPath(transform.position, out List<int> pathList);
            print(path);
            
            if (path)
            {
                //if path list has more than 1 element draw lines from each element to the next
                if (pathList?.Count > 0)
                {
                    //draw line from position to first index
                    Debug.DrawLine(transform.position + transform.up * npcEyeHeight, pathFinder.GetDesiredPosition(pathList[0]), color, 15);
                    
                    for (int i = 0; i < pathList.Count - 1; i++)
                    {
                        Debug.DrawLine(pathFinder.GetDesiredPosition(pathList[i]), pathFinder.GetDesiredPosition(pathList[i + 1]), color, 15);
                    }
                    
                    //draw line from last index to target
                    Debug.DrawLine(pathFinder.GetDesiredPosition(pathList[^1]), pathFinder.target.position + pathFinder.target.up * pathFinder.targetOffset, color, 15);
                }
            }
            
        }
        
#endif
        #endregion


        private void Start()
        {
            ChangeState(NpcStates.Serveillance);
            PlayerMovementController.Instance.player.Health.onDeath += OnDeath;
        }

        private void OnDeath()
        {
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
