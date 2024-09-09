using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.VFX;

namespace Player_Scripts.Interactables
{

    public class DragBox : Interactable
    {
        [BoxGroup] public Transform targetBox;

        [SerializeField, BoxGroup("Proximity")] private bool restrictX;
        [SerializeField, BoxGroup("Proximity")] private bool restrictY = true;
        [SerializeField, BoxGroup("Proximity")] private bool restrictZ;

        [SerializeField, BoxGroup("Proximity")]
        private float defaultY;
        
        [SerializeField, BoxGroup("Proximity")] private Vector3 bounds;
        [SerializeField, BoxGroup("Proximity")] private float proximity;
        [SerializeField, BoxGroup("Proximity")] private LayerMask proximityMask;
        
        
        
        private Vector3 _dragOffset;

        private void OnDrawGizmos()
        {

            if (Application.isPlaying) return;


            Gizmos.color = Color.red;
            Ray forwardRay = new Ray(transform.position + transform.forward * bounds.z + bounds.y * transform.up, transform.forward);
            Gizmos.DrawRay(forwardRay);


            Ray backwardRay = new Ray(transform.position - transform.forward * bounds.z + bounds.y * transform.up, -transform.forward);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(backwardRay);


            Ray leftRay = new Ray(transform.position - transform.right * bounds.x + transform.up * bounds.y, -transform.right);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(leftRay);
            Ray rightRay = new Ray(transform.position + transform.right * bounds.x + transform.up * bounds.y, transform.right);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rightRay);

        }


        private void Start()
        {
            if (!targetBox)
            {
                targetBox = transform;
            }
        }

        public override PlayerInteractionType Interact()
        {

            if (!enabled) return PlayerInteractionType.NONE;
            
            if (!GetKey())
            {
                isInteracting = false;

                return PlayerInteractionType.NONE;

            }
            else
            {

                var player = PlayerMovementController.Instance.transform;
                var playerPos = player.position;

                var boxLocalPosition = targetBox.localPosition;
                var boxPos =targetBox.position;
                var boxForward = targetBox.forward;
                var boxRight = targetBox.right;
                var boxUp = targetBox.up;


                if (!isInteracting)
                {
                    isInteracting = true;
                    _dragOffset = playerPos - boxPos;
                }


                var desiredPosition = playerPos - _dragOffset;

                var desiredLocalPosition = targetBox.parent.InverseTransformPoint(desiredPosition);

                // INFO ::: Forward is Z and Right is X 
                
                if (restrictX)
                {
                    desiredLocalPosition.x = 0;
                }
                else if (desiredLocalPosition.x - boxLocalPosition.x > 0)
                {
                    //print("Moving +ve x");
                    Ray xRay = new Ray(boxPos + boxRight * bounds.x + boxUp * bounds.y, boxRight);

                    if (Physics.Raycast(xRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, info.point, Color.green, 1f);
                        desiredLocalPosition.x = boxLocalPosition.x;
                    }
                    else
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, xRay.origin + xRay.direction * bounds.x, Color.red, 1f);
                    }
                }
                else
                {
                    //print("Moving -ve x");
                    Ray xRay = new Ray(boxPos - boxRight * bounds.x + boxUp * bounds.y, -boxRight);

                    if (Physics.Raycast(xRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, info.point, Color.green, 1f);
                        desiredLocalPosition.x = boxLocalPosition.x;
                    }
                    else
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, xRay.origin + xRay.direction * bounds.x, Color.red, 1f);
                    }
                }

                if (restrictZ)
                {
                    desiredLocalPosition.z = 0;
                }
                else if (desiredLocalPosition.z - boxLocalPosition.z > 0)
                {
                    //print("Moving +ve z");
                    Ray zRay = new Ray(boxPos + boxForward * bounds.z + boxUp * bounds.y, boxForward);

                    if (Physics.Raycast(zRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(zRay.origin, info.point, Color.green);
                        desiredLocalPosition.z = boxLocalPosition.z;
                    }
                }
                else
                {
                    //print("Moving -ve z");
                    Ray zRay = new Ray(boxPos - boxForward * bounds.z + boxUp * bounds.y, -boxForward);

                    if (Physics.Raycast(zRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(zRay.origin, info.point, Color.green);
                        desiredLocalPosition.z = boxLocalPosition.z;
                    }
                }

                if(restrictY) desiredLocalPosition.y = defaultY;
                targetBox.localPosition = desiredLocalPosition;

                return interactionType;
            }

        }
        

    }
}
