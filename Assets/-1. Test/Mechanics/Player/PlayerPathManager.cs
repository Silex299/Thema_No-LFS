using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Mechanics.Player
{
    public class PlayerPathManager : MonoBehaviour
    {
        public float pointSpacing = 0.1f;
        public float distanceThreshold = 0.1f;

        
        
        public List<Vector3> curvePoints;
        public static PlayerPathManager Instance;
        public int currentIndex;

        
        public OverridePath OverridenPath { set; get; }
        

        #region UNITY EDITOR

#if UNITY_EDITOR

        
        [Button]
        public void GenerateCurvePoints()
        {
            var controlPoints = transform.Cast<Transform>().Select(t => t.position).ToList();

            curvePoints.Clear();

            if (controlPoints == null || controlPoints.Count < 2)
            {
                Debug.LogWarning("You need at least 2 control points to generate a spline.");
                return;
            }

            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                Vector3 p0 = i == 0 ? controlPoints[i] : controlPoints[i - 1];
                Vector3 p1 = controlPoints[i];
                Vector3 p2 = controlPoints[i + 1];
                Vector3 p3 = i + 2 < controlPoints.Count ? controlPoints[i + 2] : controlPoints[i + 1];

                AddCurvePoints(p0, p1, p2, p3);
            }
        }

        private void OnDrawGizmos()
        {
            if (curvePoints == null || curvePoints.Count < 2)
            {
                Debug.LogWarning("You need at least 2 control points to draw a spline.");
                return;
            }

            Gizmos.color = Color.red;

            for (int i = 0; i < curvePoints.Count - 1; i++)
            {
                Gizmos.DrawLine(curvePoints[i], curvePoints[i + 1]);
                Gizmos.DrawSphere(curvePoints[i], 0.1f);
            }

            if (curvePoints.Count > 0)
            {
                Gizmos.DrawWireSphere(curvePoints[^1], 0.05f);
            }
        }

        private void AddCurvePoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float t = 0;
            Vector3 previousPoint = CatmullRom(p0, p1, p2, p3, t);
            curvePoints.Add(previousPoint);

            while (t < 1)
            {
                float targetDistance = pointSpacing;
                float distance = 0;
                Vector3 point = previousPoint;

                while (distance < targetDistance && t < 1)
                {
                    t += 0.01f; // Increment by a small value
                    Vector3 currentPoint = CatmullRom(p0, p1, p2, p3, t);
                    distance += Vector3.Distance(previousPoint, currentPoint);
                    previousPoint = currentPoint;
                    point = currentPoint;
                }

                if (t >= 1) break;

                curvePoints.Add(point);
            }
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * ((2f * p1) +
                           (-p0 + p2) * t +
                           (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                           (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }

#endif

        #endregion


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }


        public Vector3 GetDestination(Vector3 playerPos, bool moveNext)
        {
            var index = moveNext ? currentIndex + 1 : currentIndex - 1;
            index = Mathf.Clamp(index, 0, curvePoints.Count-1);
            
            float plannerDistance = PlannerDistance(playerPos, curvePoints[index]);

            if (plannerDistance < distanceThreshold)
            {
                currentIndex = index;
            }
            
            return OverridenPath ? OverridenPath.GetDestination(moveNext) : curvePoints[index];
        }


        public static float PlannerDistance(Vector3 a, Vector3 b)
        {
            a.y = b.y = 0;
            Debug.DrawLine(a, b, Color.blue, 1f);
            return Vector3.Distance(a, b);
        }
    }
}