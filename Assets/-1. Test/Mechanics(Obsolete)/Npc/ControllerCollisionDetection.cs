using System;
using UnityEngine;

namespace Mechanics.Npc
{
    public class ControllerCollisionDetection : MonoBehaviour
    {
        public float proximityThreshold;
        public float height;
        public float castRadius;
        public LayerMask layerMask;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + transform.up * (height / 2), new Vector3(castRadius, height, castRadius));

            FrontProximity();
            LeftProximity();
        }

        private void FrontProximity()
        {
            if (Physics.CapsuleCast(transform.position, transform.position + transform.up * height, castRadius, transform.forward, out RaycastHit hit, proximityThreshold, layerMask))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }

        private void RightProximity()
        {
            if (Physics.CapsuleCast(transform.position, transform.position + transform.up * height, castRadius, transform.right, out RaycastHit hit, proximityThreshold, layerMask))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }
        
        private void LeftProximity()
        {
            if (Physics.CapsuleCast(transform.position, transform.position + transform.up * height, castRadius, -transform.right, out RaycastHit hit, proximityThreshold, layerMask))
            {
                Debug.DrawLine(transform.position, hit.point, Color.cyan);
            }
        }
    }
}