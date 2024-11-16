using System.Collections;
using Managers.Checkpoints;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class OneTimeTrigger : MonoBehaviour
    {

        [InfoBox("-1 if you want to reset the trigger everytime a checkpoint loads")]
        [InfoBox("Resets if checkpoint is less than or equal to threshold")]
        public int checkpointThreshold = -1;
        public float initialDelay;
        public UnityEvent trigger;
        private bool _isTriggered;

        private bool _loaded;

        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += ResetTrigger;
            
            if (_loaded) return;
            int currentIndex = CheckpointManager.Instance.CurrentCheckpoint;
            ResetTrigger(currentIndex);
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= ResetTrigger;
        }

        private void ResetTrigger(int checkpoint)
        {
            _loaded = true;
            if (checkpointThreshold == -1)
            {
                _isTriggered = false;
            }
            else
            {
                _isTriggered = checkpoint > checkpointThreshold;
            }
        }

        public void Trigger()
        {
            if(!enabled) return;
            StartCoroutine(TriggerEnumerator());
        }
        
        IEnumerator TriggerEnumerator()
        {
            if (_isTriggered) yield break;
            
            yield return new WaitForSeconds(initialDelay);
            _isTriggered = true;
            trigger.Invoke();
        }
        
    }
}
