using System.Collections.Generic;
using UnityEngine;

namespace NPCs.New
{
    public class PathFinderBase : MonoBehaviour
    {
        public Transform[] waypoints;

        public virtual Vector3 GetDesiredPosition(int index)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool GetPath(Vector3 from, Vector3 to, out List<int> path)
        {
            throw new System.NotImplementedException();
        }
    }
}