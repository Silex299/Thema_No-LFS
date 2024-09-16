using UnityEngine;

namespace Player_Scripts
{
    public class PlayerStateInstances : MonoBehaviour
    {
        public int stateIndex;
        public bool overrideFlags;
        public bool oneWayRotation;
        public bool directionalInput;
        public bool forceBoost;
        public bool canBoost;
        public bool canRotate;
        public bool canPlayAlternateMovement;
        public bool canJump;

        public void ChangeState()
        {
            var playerMovement = PlayerMovementController.Instance;
            var player = playerMovement.player;

            if(player.OverrideFlags) return;
            
            playerMovement.ChangeState(stateIndex);
            player.oneWayRotation = oneWayRotation;
            player.enabledDirectionInput = directionalInput;
            player.ForceBoost = forceBoost;
            player.CanBoost = canBoost;
            player.CanRotate = canRotate;
            player.CanPlayAlternateMovement = canPlayAlternateMovement;
            player.CanJump = canJump;
            player.OverrideFlags = overrideFlags;

        }
        
    }
}
