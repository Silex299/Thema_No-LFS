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
        [FoldoutGroup("Npc Properties")] public CharacterController characterController;
        #endregion
        
        #region Seveillance
        [FoldoutGroup("Serveillance")] public List<Vector3> serveillancePoints;
        [FoldoutGroup("Serveillance")] public float serveillanceWaitTime;
        #endregion
        #region Chase
        [FoldoutGroup("Chase")] public CustomPathFinderBase pathFinder;
        [FoldoutGroup("Chase")] public float npcEyeHeight = 1.5f;
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

        [Button]
        public void TestPath(Color color)
        {
            var path = pathFinder.GetPath(transform.position, out List<int> pathList);
            print(path);
            
            if (path)
            {
                //if path list has more than 1 element draw lines from each element to the next
                if (pathList?.Count > 0)
                {
                    //draw line from position to first index
                    Debug.DrawLine(transform.position + transform.up * npcEyeHeight, pathFinder.GetDesiredPosition(pathList[0]), color, 15);
                    
                    for (int i = 0; i < pathList.Count - 1; i++)
                    {
                        Debug.DrawLine(pathFinder.GetDesiredPosition(pathList[i]), pathFinder.GetDesiredPosition(pathList[i + 1]), color, 15);
                    }
                    
                    //draw line from last index to target
                    Debug.DrawLine(pathFinder.GetDesiredPosition(pathList[^1]), pathFinder.target.position + pathFinder.target.up * pathFinder.targetOffset, color, 15);
                }
            }
            
        }
        
#endif
        #endregion
        
    }
}
