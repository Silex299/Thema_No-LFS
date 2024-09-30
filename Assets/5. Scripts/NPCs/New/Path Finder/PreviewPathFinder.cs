using System.Collections.Generic;
using NPCs.New.Path_Finder;
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
                if (pathFinder.GetPath(source.position + Vector3.up * sourceOffset, target.position + Vector3.up * targetOffset, out var path))
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


        [Button]
        public void DebugPoints(int[] points)
        {
            int i = points[0];
            int j = points[1];
            var pathFinder1 = pathFinder as DijkstraPathFinder;
            if (pathFinder1)
            {
                
                if (pathFinder1.GetPath(pathFinder1.waypoints[i].position, pathFinder1.waypoints[j].position, out var path))
                {
                    if (path == null) return;
                    

                    print(string.Join("->", path));
                    
                    
                    if (path.Count > 0)
                    {
                        Debug.DrawLine(pathFinder1.waypoints[i].position, pathFinder1.waypoints[path[0]].position, Color.red, 5f);

                        if (path.Count > 1)
                        {
                            for (int k = 0; k < path.Count; k++)
                            {
                                if (k > 0)
                                {
                                    Debug.DrawLine(pathFinder1.waypoints[path[k]].position, pathFinder1.waypoints[path[k - 1]].position, Color.red, 5f);
                                }
                            }
                        }

                        Debug.DrawLine(pathFinder1.waypoints[j].position, pathFinder1.waypoints[path[^1]].position, Color.red, 5f);
                    }
                }
                else
                {
                    print("Path not possible");
                }
            }
        }
    }
}