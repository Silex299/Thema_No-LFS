using System.Collections;
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
            StartCoroutine(WaitForAllSceneToBeLoaded());

        }


        private IEnumerator WaitForAllSceneToBeLoaded()
        {
            yield return new WaitUntil(() => SceneManager.Instance.AllLoaded && LocalSceneManager.Instance.AllSceneLoaded);
            player.Health.ResetHealth();
            player.Health.ResetPlayer();
            player.MovementController.Reset();
        }
        

        public void InitialSetup(CheckPoint checkPoint)
        {
            if(checkPoint.ignoreThisCheckpoint) return;
            
            player.CController.enabled = false;

            Transform checkpointTransform = checkPoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;
            player.MovementController.Reset();
        }

    }


}