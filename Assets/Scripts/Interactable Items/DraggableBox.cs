
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactable_Items
{
    [RequireComponent(typeof(BoxCollider))]
    public class DraggableBox : MonoBehaviour
    {

        #region Variables

        #region Exposed Variables


        #region Raycast Variables

        [SerializeField, BoxGroup("Raycast")] private float xExtent;
        [SerializeField, BoxGroup("Raycast")] private float zExtent;
        [SerializeField, BoxGroup("Raycast")] private float raycastDistance;
        [SerializeField, BoxGroup("Raycast")] private float raycastYOffset;
        [SerializeField, BoxGroup("Raycast")] private LayerMask raycastMask;
        
        
        [SerializeField, BoxGroup("Movement")] private bool moveStraight = true;
        [SerializeField, BoxGroup("Movement")] private bool moveSideways = true;
        [SerializeField, BoxGroup("Movement")] private bool restrictYMovement = true;


        /// <summary>
        /// forward, backward, left, right proximity
        /// </summary>
        private bool[] proximity = new bool[4];

        #endregion

        #endregion

        private Vector3 _followOffset;
        protected bool _drag;
        protected bool _playerIsInTrigger;


        #endregion

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main") && !_playerIsInTrigger)
            {
                _playerIsInTrigger = true;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            _playerIsInTrigger = false;
            _drag = false;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main") && !_playerIsInTrigger)
            {
                _playerIsInTrigger = true;
            }
        }


        //TODO text in FixedUpdate
        protected virtual void Update()
        {
            if (!_playerIsInTrigger) return;


            if (Input.GetButton("e"))
            {
                
            }

            if (!_drag) return;



            ProximityRaycast();
            
            var newPos = PlayerMovementController.Instance.transform.position + _followOffset;
            var position = transform.position;

            if (restrictYMovement)
            {
                newPos.y = this.transform.position.y;
            }

            if (moveSideways)
            {

                //If moving forward and forward is blocked dont move
                if ((newPos.x - position.x) > 0 && proximity[3])
                {
                    newPos.x = position.x;
                }

                //If moving backward and back side is blocked then dont move
                else if ((newPos.x - position.x) < 0 && proximity[0])
                {
                    newPos.x = position.x;
                }

            }
            else
            {
                newPos.x = position.x;
            }

            if (moveStraight)
            {

                //If moving forward and forward is blocked dont move
                if ((newPos.z - position.z) > 0 && proximity[0])
                {
                    newPos.z = position.z;
                }

                //If moving backward and back side is blocked then dont move
                else if ((newPos.z - position.z) < 0 && proximity[1])
                {
                    newPos.z = position.z;
                }

            }
            else
            {
                newPos.z = position.z;
            }


            this.transform.position = newPos;
        }

        public void OnDrawGizmos()
        {
            ProximityRaycast();

            var position = transform.position;
            var forward = transform.forward;
            var right = transform.right;
            var up = transform.up;


            Gizmos.color = Color.red;
            if (!proximity[0]) Gizmos.DrawLine(position + forward * zExtent + up * raycastYOffset, position + forward * zExtent + up * raycastYOffset + transform.forward * raycastDistance);
            if (!proximity[1]) Gizmos.DrawLine(position - forward * zExtent + up * raycastYOffset, position - forward * zExtent + up * raycastYOffset - transform.forward * raycastDistance);

            Gizmos.color = Color.blue;      
            if (!proximity[2]) Gizmos.DrawLine(position + right * xExtent + up * raycastYOffset, position + right * xExtent + up * raycastYOffset + transform.right * raycastDistance);
            if (!proximity[3]) Gizmos.DrawLine(position - right * xExtent + up * raycastYOffset, position - right * xExtent + up * raycastYOffset - transform.right * raycastDistance);
        }


        private void ProximityRaycast()
        {

            //TODO: REMOVE DEBUGS
            var position = transform.position;
            var forward = transform.forward;
            var right = transform.right;
            var up = transform.up;

            //Forward
            if (moveStraight)
            {

                proximity[0] = Physics.Raycast(position + forward * zExtent + up * raycastYOffset, transform.forward, out RaycastHit hit1, raycastDistance, raycastMask);
                proximity[1] = Physics.Raycast(position - forward * zExtent + up * raycastYOffset, -transform.forward, out RaycastHit hit2, raycastDistance, raycastMask);

                //TODO REMOVE
                if (proximity[0])
                {
                    Debug.DrawLine(position + forward * zExtent + up * raycastYOffset, hit1.point, Color.green);
                }
                else if (proximity[1])
                {
                    Debug.DrawLine(position - forward * zExtent + up * raycastYOffset, hit2.point, Color.green);
                }

            }

            //Sideways
            if (moveSideways)
            {
                proximity[2] = Physics.Raycast(position + right * xExtent + up * raycastYOffset, transform.right, out RaycastHit hit1, raycastDistance, raycastMask);
                proximity[3] = Physics.Raycast(position - right * xExtent + up * raycastYOffset, -transform.right, out RaycastHit hit2, raycastDistance, raycastMask);

                //TODO REMOVE
                //TODO REMOVE
                if (proximity[2])
                {
                    Debug.DrawLine(position + right * xExtent + up * raycastYOffset, hit1.point, Color.green);
                }
                else if (proximity[3])
                {
                    Debug.DrawLine(position - right * xExtent + up * raycastYOffset, hit2.point, Color.green);
                }
            }


        }


    }
}
