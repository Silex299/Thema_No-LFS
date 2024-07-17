using System;
using System.Collections;
using Misc;
using Player_Scripts;
using UnityEngine;

namespace Scene_Scripts
{
    public class SwingingMaze_FallCut : MonoBehaviour
    {

        private bool _triggered;
        public string fallAnimationName;
        public float fallAnimationDelay;


        public string movementStateName;

        public float exitDelay;
        public string exitAnimationStateName;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main") && !_triggered)
            {
                PlayFall();
                _triggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                
            }
        }


        private void PlayFall()
        {
            StartCoroutine(PlayerMovement());
        }


        private IEnumerator PlayerMovement()
        {
            var playerMovement = PlayerMovementController.Instance;
            
            //CHECK if player still jumps disable canJump here
            //Same for other movement states
            playerMovement.player.CanRotate = false;
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
            playerMovement.PlayAnimation(exitAnimationStateName, 0.2f, 0);
        }
        
        

        public void Reset()
        {
            _triggered = false;
        }

    }
}
