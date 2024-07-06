using UnityEngine;
using Player_Scripts;

namespace Managers.Checkpoints
{

    public class PlayerCheckpointTracker : MonoBehaviour
    {

        [SerializeField] private Player player;
        
        public void ResetItem(CheckPoint checkpoint)
        {

            player.CController.enabled = false;
            Transform checkpointTransform = checkpoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;

            player.Health.Reset();
            player.MovementController.Reset();

        }

        public void InitialSetup(CheckPoint checkPoint)
        {
            player.CController.enabled = false;

            Transform checkpointTransform = checkPoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;
            player.CController.enabled = true;
        }

    }


}