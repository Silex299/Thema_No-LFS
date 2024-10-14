
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class WaterCreatureServeillance : V1NpcBaseState
    {

        public Vector3[] waypoints;
        public float serveillanceSpeed = 1;

        #region Editor


#if UNITY_EDITOR
        [Button]
        public void SetWaypoints(Transform[] transforms)
        {
            waypoints = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                waypoints[i] = transforms[i].position;
            }
        }
        
        public void OnDrawGizmos()
        {
            if (waypoints == null) return;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(waypoints[i], 0.5f);
            }
        }

#endif
        

        #endregion

        private int _currentCheckpointIndex;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        public override void Enter(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 0);
            _currentCheckpointIndex = GetClosestServeillancePoint(npc);
        }
        
        public override void UpdateState(V1Npc npc)
        {
            //move towards the current checkpoint
            npc.UnRestrictedRotate(waypoints[_currentCheckpointIndex], Time.deltaTime * npc.rotationSpeed);
            npc.transform.position += npc.transform.forward * (serveillanceSpeed * Time.deltaTime);
            
            CheckForNextCheckpoint(npc);
        }
        
        
        private void CheckForNextCheckpoint(V1Npc npc)
        {
            if (Vector3.Distance(npc.transform.position, waypoints[_currentCheckpointIndex]) < 0.5f)
            {
                _currentCheckpointIndex++;
                if (_currentCheckpointIndex >= waypoints.Length)
                {
                    _currentCheckpointIndex = 0;
                }
            }
        }
        private int GetClosestServeillancePoint(V1Npc npc)
        {
            float minDistance = float.MaxValue;
            int index = 0;
            
            for (var i = 0; i < waypoints.Length; i++)
            {
                
                if(Physics.Linecast(npc.transform.position, waypoints[i], out RaycastHit hit, npc.pathFinder.layerMask))
                { 
                    continue;
                }
                
                float distance = Vector3.Distance(npc.transform.position, waypoints[i]);
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                index = i;
            }
            
            return index;
        }
        
    }
}
