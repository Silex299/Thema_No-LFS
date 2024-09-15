using Managers.Checkpoints;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class OneTimeTrigger : MonoBehaviour
    {

        [InfoBox("-1 if you want to reset the trigger everytime a checkpoint loads")]public int checkpointThreshold = -1;
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
            if(checkpoint>=checkpointThreshold) _isTriggered = false;
        }

        public void Trigger()
        {
            if (_isTriggered) return;
            
            _isTriggered = true;
            trigger.Invoke();
        }
        
    }
}
