using System;
using System.Collections;
using Misc;
using Player_Scripts;
using UnityEngine;

namespace Scene_Scripts
{
    public class SwingingMaze_FallCut : MonoBehaviour
    {

        public string fallAnimationName;
        public float fallAnimationDelay;


        public string movementStateName;

        public float exitDelay;
        public string exitAnimationStateName;
        

        public void PlayFall()
        {
            StartCoroutine(PlayerMovement());
        }


        
        //Some Rotation fix here
        
        private IEnumerator PlayerMovement()
        {
            var playerMovement = PlayerMovementController.Instance;
            
            //CHECK if player still jumps disable canJump here
            //Same for other movement states
            playerMovement.player.CanRotate = false;
            playerMovement.player.enabledDirectionInput = true;
            playerMovement.PlayAnimation(fallAnimationName, 0.2f, 1);
            
            yield return new WaitForSeconds(fallAnimationDelay);
            
            playerMovement.PlayAnimation(movementStateName, 0.2f, 0);
            
        }
        
        public void StopPlayerMovement()
        {

            StartCoroutine(StopPlayerMovementCoroutine());

        }

        private IEnumerator StopPlayerMovementCoroutine()
        {

            yield return new WaitForSeconds(exitDelay);
            
            var playerMovement = PlayerMovementController.Instance;
            
            playerMovement.player.CanRotate = true;
            playerMovement.player.enabledDirectionInput = true;
            playerMovement.PlayAnimation(exitAnimationStateName, 0.2f, 0);
        }
        

    }
}
