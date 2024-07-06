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

        [SerializeField, BoxGroup("Camera Info")] private ChangeOffset cameraOffsetInfo;

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
            //camera
            cameraOffsetInfo.ChangeCameraOffsetInstantaneous();
            CameraManager.Instance.Reset();

            //Player
            PlayerMovementController.Instance.ChangeState(playerStateIndex);
            PlayerPathController path = PlayerPathController.Instance;
            path.nextDestination = nextPathPointIndex;
            path.previousDestination = prevPathPointIndex;
        }

    }

}
