using System.Collections;
using Player_Scripts;
using UnityEngine;

namespace Scene_Scripts
{
    public class SwingingMaze_FallCut : MonoBehaviour
    {

        public float exitDelay;
        public string exitAnimationStateName;
        

        public void PlayFall()
        {
            var playerMovement = PlayerMovementController.Instance;
            playerMovement.player.CanJump = false;
        }

        public void ExitFall()
        {
            StartCoroutine(PlayerMovementExit());
        }

        private IEnumerator PlayerMovementExit()
        {
            yield return new WaitForSeconds(exitDelay);
            
            var playerMovement = PlayerMovementController.Instance;
            playerMovement.player.enabledDirectionInput = false;
            playerMovement.player.oneWayRotation = false;
            playerMovement.PlayAnimation(exitAnimationStateName,0.2f, 0);
        }
        
        
    }
}
