using System.Collections;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Misc
{
    //FIX: MOVE TO THEMA SCRIPT
    public class PlayerMover : MonoBehaviour
    {

        public Transform moveTo;
        public float transitionTime;
        public bool changeRotation;
        public bool restoreMovementAfterTransition;
        public bool restoreControllerAfterTransition;

        public void MovePlayer()
        {
            StartCoroutine(MoveCoroutine(moveTo, transitionTime, changeRotation, restoreMovementAfterTransition,
                restoreControllerAfterTransition));
        }
    
        public static IEnumerator MoveCoroutine(Transform moveTo, float transitionTime, bool changeRotation = true, bool restoreMovementAfterTransition = true, bool restoreControllerAfterTransition = true)
        {
        
            float timeElapsed = 0;
        
            PlayerMovementController.Instance.DisablePlayerMovement(true, false);
            PlayerMovementController.Instance.player.CController.enabled = false;
        
            var playerTransform = PlayerMovementController.Instance.transform;
            while (timeElapsed <= transitionTime)
            {
                timeElapsed += Time.deltaTime;

                // Move the player to the required position and rotate if needed
                playerTransform.position = Vector3.Lerp(playerTransform.position, moveTo.position, timeElapsed / transitionTime);
                
                if (changeRotation)
                {
                    playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, moveTo.rotation, timeElapsed / transitionTime);
                }
                
                yield return null;
            }

            playerTransform.position = moveTo.position;
            if (changeRotation)
            {
                playerTransform.rotation = moveTo.rotation;
            }


            if (restoreControllerAfterTransition)
            {
                PlayerMovementController.Instance.player.CController.enabled = true;
            }
            //if restore movement is true, enable the player movement
            if (restoreMovementAfterTransition)
            {
                PlayerMovementController.Instance.DisablePlayerMovement(false);
            }
        }


    }
}
