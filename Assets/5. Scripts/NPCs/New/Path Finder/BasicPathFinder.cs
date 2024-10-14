using System.Collections.Generic;
using UnityEngine;

namespace NPCs.New.Path_Finder
{
    public class BasicPathFinder : PathFinderBase
    {
        
        
        
        public override bool GetPath(Vector3 from, Vector3 to, out List<int> path)
        {

            if(IsDirectPathPossible(from, to))
            {
                Debug.DrawLine(from, to, Color.green);
                path = null;
                return true;
            }
            
            float distance = float.MaxValue;
            
            int closestIndex = -1;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if(!(IsDirectPathPossible(from, waypoints[i].position) && IsDirectPathPossible(waypoints[i].position, to))) continue;
                float newDistance = Vector3.Distance(from, waypoints[i].position) + Vector3.Distance(waypoints[i].position, to);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestIndex = i;
                }
            }
            
            
            if(closestIndex != -1)
            {
                path = new List<int> {closestIndex};
                Debug.DrawLine(from, waypoints[path[0]].position, Color.cyan);
                Debug.DrawLine(to, waypoints[path[0]].position, Color.cyan);
                return true;
            }
            else
            {
                path = null;
                return false; 
            }
            
        }

        public override Vector3 GetDesiredPosition(int index)
        {
            return waypoints[index].position;
        }
        
    }
}