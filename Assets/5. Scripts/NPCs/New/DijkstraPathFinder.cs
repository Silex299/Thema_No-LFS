using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace NPCs.New
{
    public class DijkstraPathFinder : MonoBehaviour
    {

        public Transform[] waypoints;
        public List<int>[] adjacencyList;
        
        public List<BakedPath> bakedPaths;
        
        
        
        [Button]
        public void BakeAdjacencyList()
        {
            adjacencyList = new List<int>[waypoints.Length];
           
            for (int i = 0; i < waypoints.Length; i++)
            {
                adjacencyList[i] = new List<int>();
           
                for (int j = 0; j < waypoints.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (Physics.Linecast(waypoints[i].position, waypoints[j].position))
                    {
                        continue;
                    }
                    adjacencyList[i].Add(j);
                }
            }
        }
        
        
        [Button]
        public void BakePath()
        {
            int sourceIndex = 0;
            
            //Create an array of float with all values at max value
            float[] distances = new float[waypoints.Length];
            int[] previousPathIndex = new int[waypoints.Length];
            
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = float.MaxValue;
                previousPathIndex[i] = -1;
            }
            PriorityQueue<int> priorityQueue = new PriorityQueue<int>(); //(weight, index)
            
            
            //implementing Dijkstra's algorithm
            distances[sourceIndex] = 0;
            priorityQueue.Enqueue(0, sourceIndex);
            
            while (priorityQueue.Count > 0)
            {
                int currentWaypointIndex = priorityQueue.Dequeue().Value;
                for (int i = 0; i < adjacencyList[currentWaypointIndex].Count; i++)
                {
                    int neighbourIndex = adjacencyList[currentWaypointIndex][i];
                    float distanceToNeighbour = Vector3.Distance(waypoints[currentWaypointIndex].position, waypoints[neighbourIndex].position);
                    float tentativeDistance = distances[currentWaypointIndex] + distanceToNeighbour;
                    
                    if (tentativeDistance < distances[neighbourIndex])
                    {
                        distances[neighbourIndex] = tentativeDistance;
                        previousPathIndex[neighbourIndex] = currentWaypointIndex;
                        priorityQueue.Enqueue(tentativeDistance, neighbourIndex);
                    }
                }
            }
            
            BakedPath bakedPath = new BakedPath
            {
                distances = distances,
                previousPathIndex = previousPathIndex
            };

            bakedPaths.Add(bakedPath);
        }
            

        }
        
        
        public struct BakedPath
        {
            public int[] previousPathIndex;
            public float[] distances;
        }
        
}
