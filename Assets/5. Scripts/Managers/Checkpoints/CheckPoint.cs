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


        public void LoadThisCheckpoint()
        {
            cameraOffsetInfo.ChangeCameraOffsetInstantaneous();
            PlayerMovementController.Instance.ChangeState(playerState, playerStateIndex);

            CameraManager.Instance.Reset();

            PlayerPathController path = PlayerPathController.Instance;
            path.nextDestination = nextPathPointIndex;
            path.previousDestination = prevPathPointIndex;
        }

    }

}
