using UnityEngine;

namespace Player_Scripts
{
    public class PlayerStateInstances : MonoBehaviour
    {
        public int stateIndex;
        public bool oneWayRotation;
        public bool directionalInput;
        public bool canBoost;
        public bool canRotate;
        public bool canPlayAlternateMovement;
        public bool canJump;

        public void ChangeState()
        {
            print("fuck");
            var playerMovement = PlayerMovementController.Instance;
            var player = playerMovement.player;

            playerMovement.ChangeState(stateIndex);
            player.oneWayRotation = oneWayRotation;
            player.enabledDirectionInput = directionalInput;
            
            player.CanBoost = canBoost;
            player.CanRotate = canRotate;
            player.CanPlayAlternateMovement = canPlayAlternateMovement;
            player.CanJump = canJump;

        }
        
    }
}
