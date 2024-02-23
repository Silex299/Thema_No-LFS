using Player_Scripts;
using UnityEngine;

namespace Interactable_Items
{
    public class RopeSegment : MonoBehaviour
    {
        public bool isAttached;

        public float minY;
        public float maxY;
        
        public Rigidbody rb;

        private void OnTriggerEnter(Collider other)
        {
            
            if(isAttached) return;

            if (!other.CompareTag("Player_Main") && !other.CompareTag("Player")) return;

            if (!PlayerController.Instance.ropeMovement.SetConnectedRigidBody(this)) return;
            
            //Change player state
            PlayerController.Instance.ChangeState(PlayerController.PlayerStates.RopeMovement, 0, 0.1f);
            
            //Attach player tp the rope
            PlayerController.Instance.transform.parent = this.transform;
            isAttached = true;
        }


        private void LateUpdate()
        {
            if(!isAttached) return;


            var playerTransform = PlayerController.Instance.transform;
            
            var playerPos = playerTransform.localPosition;

            
            //Keep player in rope range, so that player doesn't go outside the bounds of the rope
            if (playerPos.y < minY)
            {
                playerPos.y = minY;
            }
            else if (playerPos.y > maxY)
            {
                playerPos.y = maxY;
            }


            //Keep player centred by setting local x and z position to zero
            playerTransform.localPosition =
                Vector3.MoveTowards(playerPos, new Vector3(0, playerPos.y, 0), Time.deltaTime * 7);

            
            
        }
    }
}
