using Sirenix.OdinInspector;
using UnityEngine;
using Thema_Camera;
using Player_Scripts;
using Path_Scripts;

namespace Managers.Checkpoints
{
    public class CheckPoint : MonoBehaviour
    {
        
        public bool ignoreThisCheckpoint;
        
        [BoxGroup("Player Info")] public int playerStateIndex;
        [BoxGroup("Player Info")] public int nextPathPointIndex;
        [BoxGroup("Player Info")] public int prevPathPointIndex;

        [BoxGroup("Player Info")] public bool overrideAnimation;
        [BoxGroup("Player Info"), ShowIf(nameof(overrideAnimation))] public string overrideAnimationName;
        [BoxGroup("Player Movement")] public bool canRotate = true;
        [BoxGroup("Player Movement")] public bool canJump;
        [BoxGroup("Player Movement")] public bool canBoost;
        [BoxGroup("Player Movement")] public bool canPlayAlternateMovement;
        

        public int checkpointIndex;


        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") || other.CompareTag("Player_Main"))
            {
                CheckpointManager.Instance.SaveCheckpoint(checkpointIndex);
            }
        }

        public void LoadThisCheckpoint()
        {
            if(ignoreThisCheckpoint) return;

            //Player
            Player player = PlayerMovementController.Instance.player;
            player.CanBoost = canBoost;
            player.CanJump = canJump;
            player.CanRotate = canRotate;
            player.CanPlayAlternateMovement = canPlayAlternateMovement;
            
            PlayerMovementController.Instance.ChangeState(playerStateIndex);
            if(overrideAnimation) PlayerMovementController.Instance.player.AnimationController.Play(overrideAnimationName, 0);
            
            
            PlayerPathController path = PlayerPathController.Instance;
            path.nextDestination = nextPathPointIndex;
            path.previousDestination = prevPathPointIndex;
        }

    }

}
