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

        private void Start()
        {
            CheckpointManager.Instance.onCheckpointLoad += ResetTrigger;
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= ResetTrigger;
        }

        private void ResetTrigger(int checkpoint)
        {
            if (checkpointThreshold == -1)
            {
                _isTriggered = false;
            }
            if(checkpoint <= checkpointThreshold) _isTriggered = false;
        }

        public void Trigger()
        {
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
