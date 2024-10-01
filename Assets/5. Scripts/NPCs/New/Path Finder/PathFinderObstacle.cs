
using System.Collections;
using UnityEngine;

namespace NPCs.New.Path_Finder
{
    public class PathFinderObstacle : MonoBehaviour
    {
        
        
        public DijkstraPathFinder dijkstraPathFinder;

        public float distanceThreshold;
        public float checkInterval;


        private IEnumerator Start()
        {
            Vector3 lastPosition = transform.position;
            while (true)
            {
                float distance = (transform.position - lastPosition).magnitude;
                lastPosition = transform.position;
                
                if (distance > distanceThreshold)
                {
                    dijkstraPathFinder.BakePath();
                    Debug.LogWarning("Baked Path");
                }
                
                yield return new WaitForSeconds(checkInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        
    }
}
