using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class ProceduralSpider : MonoBehaviour
    {

        [SerializeField, SceneObjectsOnly] private List<ProceduaralSpiderLeg> legs;
        [SerializeField] private float raycastHeight = 0.5f;
        [SerializeField] private float minGroundClearance = 0.2f;
        [SerializeField] private float maxGroundClearance = 0.3f;
        [SerializeField] private LayerMask raycastMask;
        [SerializeField] private float speed;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + transform.up * raycastHeight, 0.02f);
        }


        void FixedUpdate()
        {
            if(Physics.Raycast(transform.position + transform.up * raycastHeight, -transform.up, out RaycastHit hit, Mathf.Infinity, raycastMask))
            {

                if(transform.up != hit.normal)
                {
                    transform.up = Vector3.MoveTowards(transform.up, hit.normal, Time.fixedDeltaTime * speed);
                }

                Debug.DrawLine(transform.position + transform.up * raycastHeight, hit.point, Color.green);

                if(hit.distance < minGroundClearance || hit.distance>maxGroundClearance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, hit.point + transform.up * ((minGroundClearance + maxGroundClearance) / 2), Time.fixedDeltaTime * speed);
                }

            }
        }

    }
}