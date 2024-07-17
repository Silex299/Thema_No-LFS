using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Interactables
{

    public class DraggableFloater : Interactable
    {


        [SerializeField, BoxGroup("Floater Params")] private bool restricX;
        [SerializeField, BoxGroup("Floater Params")] private bool restricZ;


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

        public override PlayerInteractionType Interact()
        {
            if (!GetKey() || !_playerIsInTrigger)
            {
                _isInteracting = false;
                return PlayerInteractionType.NONE;
            }
            else
            {
                Transform playerTransform = PlayerMovementController.Instance.transform;

                Vector3 playerPos = playerTransform.position;

                Vector3 position = transform.position;
                Vector3 localPosition = transform.localPosition;
                Vector3 right = transform.right;
                Vector3 forward = transform.forward;
                Vector3 up = transform.up;

                if (!_isInteracting)
                {
                    _isInteracting = true;
                    _dragOffset = playerPos - position;
                }

                Vector3 desiredPosition = playerPos - _dragOffset;
                Vector3 desiredLocalPosition = transform.parent.InverseTransformPoint(desiredPosition);

                #region PROXIMITY

                if (restricX)
                {
                    desiredLocalPosition.x = 0;
                }
                else if (desiredLocalPosition.x - localPosition.x > 0)
                {
                    //MOVING IN +VE X

                    Ray xRay = new Ray(position + right * bounds.x + up * bounds.y, right);

                    if (Physics.Raycast(xRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, info.point, Color.green, 1f);
                        desiredLocalPosition.x = localPosition.x;
                    }
                    else
                    {
                        Debug.DrawLine(xRay.origin, xRay.origin + xRay.direction, Color.red, 1f);

                    }
                }
                else
                {
                    //MOVING IN -VE X
                    Ray xRay = new Ray(position - right * bounds.x + up * bounds.y, -right);

                    if (Physics.Raycast(xRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(xRay.origin, info.point, Color.green, 1f);
                        desiredLocalPosition.x = localPosition.x;
                    }
                    else
                    {
                        Debug.DrawLine(xRay.origin, xRay.origin + xRay.direction, Color.red, 1f);
                    }
                }




                if (restricZ)
                {
                    desiredLocalPosition.z = 0;
                }
                else if (desiredLocalPosition.z - localPosition.z > 0)
                {
                    //Moving in +ve z
                    Ray zRay = new Ray(position + forward * bounds.z + up * bounds.y, forward);

                    if (Physics.Raycast(zRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(zRay.origin, info.point, Color.green);
                        desiredLocalPosition.z = localPosition.z;
                    }
                    else
                    {
                        Debug.DrawLine(zRay.origin, zRay.origin + zRay.direction, Color.red, 1f);
                    }
                }
                else
                {
                    //moving in -ve Z
                    Ray zRay = new Ray(position - forward * bounds.z + up * bounds.y, -forward);

                    if (Physics.Raycast(zRay, out RaycastHit info, proximity, proximityMask))
                    {
                        //REMOVE
                        Debug.DrawLine(zRay.origin, info.point, Color.green);
                        desiredLocalPosition.z = localPosition.z;
                    }
                    else
                    {
                        Debug.DrawLine(zRay.origin, zRay.origin + zRay.direction, Color.red, 1f);
                    }
                }


                #endregion

                desiredLocalPosition.y = 0;
                transform.localPosition = desiredLocalPosition;


                return interactionType;
                //Follow The player
            }
        }



    }

}