using System.Collections;
using UnityEngine;
using Player_Scripts;

namespace Managers.Checkpoints
{

    public class PlayerCheckpointTracker : MonoBehaviour
    {

        [SerializeField] private Player player;
        public float playerResetDelay = 0.5f;
        
        public void ResetItem(CheckPoint checkPoint)
        {
            if(checkPoint.ignoreThisCheckpoint) return;
            StartCoroutine(WaitForAllSceneToBeLoaded(checkPoint));

        }


        private IEnumerator WaitForAllSceneToBeLoaded(CheckPoint checkPoint)
        {
            yield return new WaitForEndOfFrame();
            
            player.CController.enabled = false;
            player.DisabledPlayerMovement = true;
            
            Transform checkpointTransform = checkPoint.transform;
            player.transform.position = checkpointTransform.position;
            player.transform.rotation = checkpointTransform.rotation;
            
            yield return new WaitUntil(() => SceneManager.Instance.AllLoaded && LocalSceneManager.Instance.AllSceneLoaded);

            yield return new WaitForSeconds(playerResetDelay);
            
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