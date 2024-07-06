using UnityEngine;
using Player_Scripts;

namespace Managers.Checkpoints
{

    public class PlayerCheckpointTracker : MonoBehaviour
    {

        [SerializeField] private Player player;
        
        public void ResetItem(CheckPoint checkPoint)
        {
            if(checkPoint.ignoreThisCheckpoint) return;

            player.CController.enabled = false;
            
            Transform checkpointTransform = checkPoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;

            player.Health.Reset();
            player.Health.ResetHealth();
            player.MovementController.Reset();
            player.CController.enabled = true;

        }

        public void InitialSetup(CheckPoint checkPoint)
        {
            if(checkPoint.ignoreThisCheckpoint) return;
            
            player.CController.enabled = false;

            Transform checkpointTransform = checkPoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;
            player.CController.enabled = true;
        }

    }


}