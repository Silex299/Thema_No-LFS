using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New
{
    public class PreviewPathFinder : MonoBehaviour
    {
        public PathFinderBase pathFinder;
        public Transform source;
        public Transform target;
        public float targetOffset;
        public float sourceOffset;

        [Space(10)] public bool drawPathFinder;
        public bool drawPath;

        private void OnDrawGizmos()
        {
            if (drawPathFinder && pathFinder)
            {
                DrawMesh();
            }

            if (drawPath)
            {
                List<int> path;
                
                if (pathFinder.GetPath(source.position + Vector3.up * sourceOffset, target.position + Vector3.up * targetOffset, out path))
                {
                    Gizmos.color = Color.white;
                    if (path == null)
                    {
                        //drawline from source to target
                        Gizmos.DrawLine(source.position + Vector3.up * sourceOffset, target.position + Vector3.up * targetOffset);
                        return;
                    }
                    
                    //draw line from target to start point
                    Gizmos.DrawLine(source.position + Vector3.up * sourceOffset, pathFinder.waypoints[path[0]].position);
                    
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (i > 0)
                        {
                            Gizmos.DrawLine(pathFinder.waypoints[path[i]].position, pathFinder.waypoints[path[i - 1]].position);
                        }
                    }
                    
                    //draw line from source to end point
                    Gizmos.DrawLine(target.position + Vector3.up * targetOffset, pathFinder.waypoints[path[path.Count - 1]].position);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(source.position + Vector3.up * sourceOffset, target.position + Vector3.up * targetOffset);
                }
            }
            
        }

        [ShowIf(nameof(drawPathFinder))] public Color meshColor = Color.green;

        private void DrawMesh()
        {
            //draw lines from each point to every other point in pathfinder waypoint
            Gizmos.color = meshColor;
            for (int i = 0; i < pathFinder.waypoints.Length; i++)
            {
                for (int j = 0; j < pathFinder.waypoints.Length; j++)
                {
                    if (i == j) continue;
                    Gizmos.DrawLine(pathFinder.waypoints[i].position, pathFinder.waypoints[j].position);
                }
            }
        }
     
    }
}