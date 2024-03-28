using Sirenix.OdinInspector;
using UnityEngine;
using Thema_Camera;
using Player_Scripts;
using Path_Scripts;

namespace Managers.Checkpoints
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField, BoxGroup("Player Info")] private Player_Scripts.PlayerMovementState playerState;
        [SerializeField, BoxGroup("Player Info")] private int playerStateIndex;
        [SerializeField, BoxGroup("Player Info")] private int nextPathPointIndex;
        [SerializeField, BoxGroup("Player Info")] private int prevPathPointIndex;

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
            cameraOffsetInfo.ChangeCameraOffsetInstantaneous();
            PlayerMovementController.Instance.ChangeState(playerState, playerStateIndex);
            PlayerMovementController.Instance.player.Health.ResetHealth();

            CameraManager.Instance.Reset();

            PlayerPathController path = PlayerPathController.Instance;
            path.nextDestination = nextPathPointIndex;
            path.previousDestination = prevPathPointIndex;
        }

    }

}
