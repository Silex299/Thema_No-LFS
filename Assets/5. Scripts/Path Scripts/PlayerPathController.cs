using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Path_Scripts
{
    public class PlayerPathController : MonoBehaviour
    {

        #region Variables

        public static PlayerPathController Instance;

        
        [BoxGroup("Path Direction")] public int nextDestination;
        [BoxGroup("Path Direction")] public int previousDestination;
        [BoxGroup("Path Direction")] public OverridePath overridePath;
        
        [SerializeField,BoxGroup("Misc")] private List<Transform> pathPoints;
        public List<Transform> PathPoints => pathPoints;

        [Button("Set Path Points", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void SetPathPoints()
        {
            pathPoints = new List<Transform>();

            //Get points 
            PathPoint[] points = GetComponentsInChildren<PathPoint>();
            int currentIndex = 0;

            foreach (PathPoint point in points)
            {
                pathPoints.Add(point.transform);
                point.name = "PathPoint_" + currentIndex;

                point.SetPoint(Mathf.Clamp(currentIndex + 1, 0, points.Length - 1),
                    Mathf.Clamp(currentIndex - 1, 0, points.Length - 1), currentIndex);

                currentIndex++;
            }

        }


        #endregion

        private void OnEnable()
        {
            if (PlayerPathController.Instance == null)
            {
                PlayerPathController.Instance = this;
            }
            else if (PlayerPathController.Instance != this)
            {
                Destroy(PlayerPathController.Instance);
            }
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying || pathPoints.Count < 2) return;

            Gizmos.color = Color.red;

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[pathPoints[i].GetComponent<PathPoint>().nextPointIndex].position);
            }
        }


        public void ChangeNextPathPoint(int index)
        {
            nextDestination = index;
        }

        public Vector3 GetNextPosition(float input, float otherInput)
        {
            if (overridePath)
            {
                return overridePath.GetNextPosition(input, otherInput);
            }
            else
            {
                return input > 0 ? pathPoints[nextDestination].position : pathPoints[previousDestination].position;
            }
        }

    }
}
