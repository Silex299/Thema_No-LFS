using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace NPCs.New.Path_Finder
{
    public class DijkstraPathFinder : PathFinderBase
    {
        private List<int>[] _adjacencyList;
        public List<BakedPath> bakedPaths;
        

        private void BakeAdjacencyList()
        {
            _adjacencyList = new List<int>[waypoints.Length];

            for (int i = 0; i < waypoints.Length; i++)
            {
                _adjacencyList[i] = new List<int>();

                for (int j = 0; j < waypoints.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (Physics.Linecast(waypoints[i].position, waypoints[j].position, layerMask))
                    {
                        continue;
                    }

                    _adjacencyList[i].Add(j);
                }
            }
        }
        [Button]
        public void BakePath()
        {
            BakeAdjacencyList();
            bakedPaths = new List<BakedPath>();
            for (int sourceIndex = 0; sourceIndex < waypoints.Length; sourceIndex++)
            {
                //Create an array of float with all values at max value
                float[] distances = new float[waypoints.Length];
                int[] previousPathIndex = new int[waypoints.Length];

                for (int i = 0; i < waypoints.Length; i++)
                {
                    distances[i] = float.MaxValue;
                    previousPathIndex[i] = -1;
                }

                var priorityQueue = new PriorityQueue<int>(); //(weight, index)


                //implementing Dijkstra's algorithm
                distances[sourceIndex] = 0;
                priorityQueue.Enqueue(0, sourceIndex);

                while (priorityQueue.Count > 0)
                {
                    int currentWaypointIndex = priorityQueue.Dequeue().Value;
                    for (int i = 0; i < _adjacencyList[currentWaypointIndex].Count; i++)
                    {
                        int neighbourIndex = _adjacencyList[currentWaypointIndex][i];
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
        
        public override bool GetPath(Vector3 from, Vector3 to, out List<int> path)
        {
            path = null;

            if (IsDirectPathPossible(from, to))
            {
                return true;
            }

            var pathPair = GetPathPair(from, to);
            if (pathPair.Item1 == -1 || pathPair.Item2 == -1) return false;

            path = ConstructPath(pathPair.Item1, pathPair.Item2);
            return true;
        }

        private List<int> ConstructPath(int from, int to)
        {
            List<int> path = new List<int>();

            var bakedPath = bakedPaths[from];
            int currentIndex = to;
            while (currentIndex != from || currentIndex == -1)
            {
                path.Insert(0, currentIndex);

                int nextIndex = bakedPath.previousPathIndex[currentIndex];
                currentIndex = nextIndex;
            }

            path.Insert(0, from);
            return path;
        }

        private (int, int) GetPathPair(Vector3 from, Vector3 to)
        {
            List<int> fromAducentPoints = GetAducentPoints(from);
            List<int> toAducentPoints = GetAducentPoints(to);

            float minDistance = float.MaxValue;
            (int, int) pathPair = (-1, -1);

            foreach (var fromIndex in fromAducentPoints)
            {
                foreach (var toIndex in toAducentPoints)
                {
                    if (!IsPathPossible(fromIndex, toIndex, out var weight)) continue;

                    weight += Vector3.Distance(waypoints[fromIndex].position, from) + Vector3.Distance(waypoints[toIndex].position, to);

                    if (weight < minDistance)
                    {
                        minDistance = weight;
                        pathPair = (fromIndex, toIndex);
                    }
                }
            }

            return pathPair;
        }

        private bool IsPathPossible(int from, int to, out float weight)
        {
            if (from == to)
            {
                weight = 0;
                return true;
            }

            if (bakedPaths[from].previousPathIndex[to] != -1)
            {
                weight = bakedPaths[from].distances[to];
                return true;
            }

            weight = 0;
            return false;
        }

        private List<int> GetAducentPoints(Vector3 from)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (IsDirectPathPossible(from, waypoints[i].position))
                {
                    list.Add(i);
                }
            }

            return list;
        }
        
        public override Vector3 GetDesiredPosition(int index)
        {
            return waypoints[index].position;
        }
    }


    [System.Serializable]
    public struct BakedPath
    {
        public int[] previousPathIndex;
        public float[] distances;
    }
}