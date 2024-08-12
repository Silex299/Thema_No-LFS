using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Npc
{
    public class Npc : MonoBehaviour
    {
        public Animator animator;
        public float stopDistance = 1f;
        public float rotationSpeed = 10;
        public float accelerationTime = 1;
        
        [FoldoutGroup("Serveillance")] public List<Vector3> serveillancePoints;
        [FoldoutGroup("Serveillance")] public float serveillanceWaitTime;
        
        #region Editor
#if UNITY_EDITOR

        [Button]
        public void GetWaypoints(Transform[] points)
        {
            serveillancePoints = new List<Vector3>();
            foreach (var point in points)
            {
                serveillancePoints.Add(point.position);
            }
        }
        
#endif
        #endregion
        
    }
}
