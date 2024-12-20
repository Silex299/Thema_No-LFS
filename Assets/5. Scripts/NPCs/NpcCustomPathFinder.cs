using System.Collections.Generic;
using UnityEngine;

namespace NPCs
{
    public class NpcCustomPathFinder : NpcPathFinder
    {
        public float waypointThreshold;
        public float destinationThreshold;
        public Transform[] waypoints;
        public bool inverted;

        private Dictionary<int, int> _npcInstance = new Dictionary<int, int>();

        public override Vector3 GetNextPoint(GuardNpc npc, Vector3 destination)
        {
            
            
            
            int instanceId = npc.GetInstanceID();
            Vector3 origin = npc.transform.position;
            float destinationDistance = Vector3.Distance(origin, destination);
            
            
            
            if (destinationDistance < destinationThreshold)
            {
                if(InSight(origin + Vector3.up * 1f, destination + Vector3.up * 1f))
                {
                    Debug.DrawLine(origin + Vector3.up * 1f, destination + Vector3.up * 1f, Color.red);

                    if (_npcInstance.ContainsKey(instanceId))
                    {
                        _npcInstance.Remove(instanceId);
                    }
                
                    return destination;
                }
                
            }
            
                
            if (!_npcInstance.ContainsKey(instanceId))
            {
                GetInitialWaypoint(origin, instanceId);
            }
            
            Debug.DrawLine(origin + Vector3.up * 1f, waypoints[_npcInstance[instanceId]].position, Color.blue);
            
            var npcPos = origin;
            var currentWaypoint = waypoints[_npcInstance[instanceId]].position;
            npcPos.y = currentWaypoint.y;
                
            //Get distance between origin and current waypoint
            float distance = Vector3.Distance(npcPos, currentWaypoint);

            //If the distance is less than the threshold distance, move to the next waypoint
            if (distance < waypointThreshold)
            {
                if (waypoints[_npcInstance[instanceId]].name.Substring(0, 4) != "Point")
                {
                    npc.animator.CrossFade(waypoints[_npcInstance[instanceId]].name, 0.2f, 1); 
                }

                if (_npcInstance[instanceId] < waypoints.Length - 1)
                {
                    _npcInstance[instanceId] = _npcInstance[instanceId] + 1;
                }
            }
                
            return waypoints[_npcInstance[instanceId]].position;
            
            
        }

        private void GetInitialWaypoint(Vector3 npcPos, int instanceId)
        {
            //find the closest waypoint to the npc
            float minDistance = Mathf.Infinity;
            int closestWaypoint = 0;

            for (int i = 0; i < waypoints.Length; i++)
            {
                float distance = Vector3.Distance(npcPos, waypoints[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestWaypoint = i;
                }
            }

            _npcInstance.Add(instanceId, closestWaypoint);
        }
    }
}