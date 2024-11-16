using System.Collections;
using Managers;
using Managers.Checkpoints;
using UnityEngine;

namespace Scene_Scripts
{
    public class Experiments : MonoBehaviour
    {
        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoad;
            StartCoroutine(UiUpdateOnCheckpointLoad());
        }

        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoad;
        }
        
        private void OnCheckpointLoad(int checkpoint)
        {
            if (checkpoint >= 0)
            {
                StartCoroutine(UiUpdateOnCheckpointLoad());
            }
        }

        private IEnumerator UiUpdateOnCheckpointLoad()
        {
            yield return new WaitForSeconds(2f);
            UIManager.Instance.FadeOut(1f);
        }
        
    }
}
