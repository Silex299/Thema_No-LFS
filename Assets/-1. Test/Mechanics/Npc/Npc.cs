using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Mechanics.Npc
{
    public class Npc : MonoBehaviour
    {

        #region Variables

        #region Common Properties
        [FoldoutGroup("Npc Properties")] public Animator animator;
        [FoldoutGroup("Npc Properties")] public float stopDistance = 1f;
        [FoldoutGroup("Npc Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Properties")] public float accelerationTime = 1;
        #endregion
        
        #region Seveillance
        [FoldoutGroup("Serveillance")] public List<Vector3> serveillancePoints;
        [FoldoutGroup("Serveillance")] public float serveillanceWaitTime;
        #endregion
        #region Chase
        [FoldoutGroup("Chase")] public CustomPathFinderBase pathFinder;
        [FoldoutGroup("Chase")] public float pathFindingInterval = 0.5f;
        [FoldoutGroup("Chase")] public float attackDistance;
        internal Action onAttack;
        #endregion
        #region After Death
        [FoldoutGroup("After Death")] public bool serveillanceAfterDeath;
        #endregion
        
        #endregion
        
        #region Editor
#if UNITY_EDITOR

        [Button]
        public void GetWaypoints(List<Transform> points)
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
