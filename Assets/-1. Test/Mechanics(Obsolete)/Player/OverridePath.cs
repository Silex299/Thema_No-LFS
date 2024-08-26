using UnityEngine;

namespace Mechanics.Player
{
    public class OverridePath : MonoBehaviour
    {
        public Transform[] overridePoints = new Transform[2];
        public float distanceThreshold = 0.1f;


        public Vector3 GetDestination(bool moveNext)
        {

            return overridePoints[moveNext ? 1 : 0].position;

        }
    }
}