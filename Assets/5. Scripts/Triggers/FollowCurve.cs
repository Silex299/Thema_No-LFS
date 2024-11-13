using System;
using Player_Scripts;
using UnityEngine;

namespace Triggers
{
    public class FollowCurve : MonoBehaviour
    {
        public Vector3[] points; // Points defining the curve
        public int segmentsPerCurve = 20; // Number of segments between each point for smoothness

        public bool restrictX = true;
        public bool restrictY = false;
        public bool restrictZ = true;

        [Header("Smoothness")] public float smoothness;

       
        [ContextMenu("Update Player Position")] 
        private void TestPlayerPos()
        {
            var player = FindObjectOfType(typeof(Player)) as Player;
            var pos = transform.InverseTransformPoint(player.transform.position);
            
            Debug.DrawLine(transform.TransformPoint(pos), transform.TransformPoint(FindClosestPointOnCurve(pos)),  Color.red, 4f);
            
            print(transform.TransformPoint(pos));
            print(transform.TransformPoint(FindClosestPointOnCurve(pos)));
        }

        public void UpdatePosition(Transform target)
        {
            if (points == null || points.Length < 2)
                return;


            var pos = transform.InverseTransformPoint(target.position);
            
            // Find the closest point on the curve to the player's current position
            Vector3 closestPointOnCurve = FindClosestPointOnCurve(pos);

            if (restrictX) pos.x = closestPointOnCurve.x;
            if (restrictY) pos.y = closestPointOnCurve.y;
            if (restrictZ) pos.z = closestPointOnCurve.z;

            target.position = Vector3.Lerp(target.position, transform.TransformPoint(pos), Time.deltaTime * smoothness);
        }

        private Vector3 FindClosestPointOnCurve(Vector3 playerPosition)
        {
            Vector3 closestPoint = Vector3.zero;
            float closestDistance = float.MaxValue;

            // Loop through each segment of the curve
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 p0 = i == 0 ? points[i] : points[i - 1];
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                Vector3 p3 = i == points.Length - 2 ? points[i + 1] : points[i + 2];

                // Check segments between p1 and p2 for smooth interpolation
                for (int j = 0; j <= segmentsPerCurve; j++)
                {
                    float t = j / (float)segmentsPerCurve;
                    Vector3 positionOnCurve = CatmullRom(p0, p1, p2, p3, t);
                    float distance = Vector3.Distance(playerPosition, positionOnCurve);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = positionOnCurve;
                    }
                }
            }

            return closestPoint;
        }

        // Catmull-Rom Spline interpolation function
        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector3 result = 0.5f * ((2f * p1) +
                        (-p0 + p2) * t +
                        (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                        (-p0 + 3f * p1 - 3f * p2 + p3) * t3);

            return result;
        }
        
        // Draw Gizmos to visualize the points
        private void OnDrawGizmos()
        {
            if (points == null)
                return;

            Gizmos.color = Color.yellow;

            // Draw a small sphere for each point to make it visible in the editor
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(points[i]), 0.1f);
            }

            // Draw the curve
            Gizmos.color = Color.green;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 p0 = i == 0 ? points[i] : points[i - 1];
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                Vector3 p3 = i == points.Length - 2 ? points[i + 1] : points[i + 2];

                for (int j = 0; j < segmentsPerCurve; j++)
                {
                    float t1 = j / (float)segmentsPerCurve;
                    float t2 = (j + 1) / (float)segmentsPerCurve;

                    Vector3 start = CatmullRom(p0, p1, p2, p3, t1);
                    Vector3 end = CatmullRom(p0, p1, p2, p3, t2);

                    Gizmos.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
                }
            }
        }
    }
}
