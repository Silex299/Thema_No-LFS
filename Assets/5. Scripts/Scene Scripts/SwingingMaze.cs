using System.Collections;
using Misc;
using Misc.Items;
using NavMesh_NPCs;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts
{
    public class SwingingMaze : MonoBehaviour
    {

        
        [BoxGroup("Cut_2")] public NavMeshNpcController npc;
        [BoxGroup("Cut_2")] public Rope rope;
        [BoxGroup("Cut_2")] public float sceneAnimationDuration;
        [BoxGroup("Cut_2")] public float sceneAnimDelay;
        
        public void EngageCut2Trigger()
        {
            
            var playerMovement = PlayerMovementController.Instance;
            playerMovement.ResetMovement();
            playerMovement.player.CanRotate = false;
        }

        public void Cut_2_Fall()
        {
            StartCoroutine(Cut_2());
        }

        private IEnumerator Cut_2()
        {

            float timeElapsed = Time.time;
            //break the rope.
            rope.BreakRope(false);
            //play scene animation
            npc.agent.enabled = false;
            

            //after player falls disable player movement
            //if player is grounded (after 0.4f) enable player movement
            yield return new WaitUntil(() => PlayerMovementController.Instance.player.IsGrounded);
            
            
            yield return new WaitForSeconds(sceneAnimDelay);
            
            PlayerSceneAnimatonManager.Instance.PlayPlayerSceneAnimation(1);
            timeElapsed = Time.time - timeElapsed;
            PlayerMovementController.Instance.player.CanJump = false;

            yield return new WaitForSeconds(sceneAnimationDuration - timeElapsed);
            
            //if scene animation is complete-> set agent target to player
            npc.Target = PlayerMovementController.Instance.transform;
            npc.agent.enabled = true;


        }
        
    }
}
