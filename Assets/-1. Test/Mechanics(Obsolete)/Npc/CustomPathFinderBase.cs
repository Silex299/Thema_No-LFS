using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Npc
{
    public class CustomPathFinderBase : MonoBehaviour
    {
        public List<Transform> waypoints;
        public Transform target;
        public float targetOffset;
        public LayerMask layerMask;

        public Vector3 GetDesiredPosition(int index)
        {
            return waypoints[index].position;
        }
        public bool GetPath(Vector3 from, out List<int> path)
        {
            if (IsTargetInSight(from))
            {
                path = null;
                return true;
            }

            Queue<int> exploreQueue = new Queue<int>();
            Dictionary<int, int> pathMap = new Dictionary<int, int>();

            exploreQueue.Enqueue(-1); //-1 means npc position

            while (exploreQueue.Count > 0)
            {
                int current = exploreQueue.Dequeue();

                for (int point = 0; point < waypoints.Count; point++)
                {
                    if(point == current) continue;
                    if(!pathMap.ContainsKey(point) && IsDirectPathPossible((current == -1 ? from : waypoints[current].position), waypoints[point].position))
                    {
                        pathMap[point] = current;
                        exploreQueue.Enqueue(point);

                        if (IsTargetInSight(waypoints[point].position))
                        {
                            path = BuildPath(point, pathMap);
                            return true;
                        }
                    }
                }
            }
            
            path = null;
            return false;
        }
        private List<int> BuildPath(int lastIndex, Dictionary<int, int> pathMap)
        {
            List<int> path = new List<int>();
            
            while (lastIndex != -1)
            {
                path.Insert(0, lastIndex);
                lastIndex = pathMap[lastIndex];
            }
            return path;
        }
        
        private bool IsDirectPathPossible(Vector3 from1, Vector3 to)
        {
            return !Physics.Linecast(from1, to, layerMask);
        }
        private bool IsTargetInSight(Vector3 fromPosition)
        {
            return !Physics.Linecast(fromPosition, target.position + Vector3.up * targetOffset, layerMask);
        }
    }
}