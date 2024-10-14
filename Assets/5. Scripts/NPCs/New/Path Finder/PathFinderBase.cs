using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NPCs.New
{
    public class PathFinderBase : MonoBehaviour
    {
        public Transform[] waypoints;
        public LayerMask layerMask;

        public virtual Vector3 GetDesiredPosition(int index)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool GetPath(Vector3 from, Vector3 to, out List<int> path)
        {
            throw new System.NotImplementedException();
        }

        protected virtual bool IsDirectPathPossible(Vector3 from, Vector3 to)
        {
            return !Physics.Linecast(from, to, layerMask);
        }
    }
}