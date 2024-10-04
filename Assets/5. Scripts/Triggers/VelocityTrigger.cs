using System.Collections;
using Managers.Checkpoints;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class VelocityTrigger : MonoBehaviour
    {
        

        [BoxGroup("Checkpoint")]
        public int checkpointThreshold = -1;
        [BoxGroup("Checkpoint")] public Vector3 resetPosition;
        [BoxGroup("Checkpoint")] public Vector3 resetRotation;
        [BoxGroup("Checkpoint"), Space(10)] public Vector3 finalPosition;
        [BoxGroup("Checkpoint")] public Vector3 finalRotation;
        
        
        public float velocityThreshold = 1;
        public float checkInterval = 0.5f;
        [field: SerializeField] public bool IsEnabled { get; set; } = true;

        public UnityEvent trigger;
        private Vector3 _lastPosition;


        private void Start()
        {
            _lastPosition = transform.position;
        }

        #region Checkpoint Load

        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoad;
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoad;
        }
        private void OnCheckpointLoad(int checkpoint)
        {
            if (checkpointThreshold == -1 || checkpoint <= checkpointThreshold)
            {
                StartCoroutine(ResetPosition());
            }
            else
            {
                StartCoroutine(ResetPosition(true));
            }
        }
        private IEnumerator ResetPosition(bool finalState = false)
        {
            
            yield return new WaitForEndOfFrame();
            
            if (finalState)
            {
                transform.localPosition = finalPosition;
                transform.localRotation = Quaternion.Euler(finalRotation);
            }
            else
            {
                transform.localPosition = resetPosition;
                transform.localRotation = Quaternion.Euler(resetRotation);
            }
            
            _lastPosition = transform.position;
            
        }
        

        #endregion

        private void FixedUpdate()
        {
            if (!IsEnabled) return;
            
            if(_lastPosition == transform.position) return;
            
            var velocity = (transform.position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;
            
            if(Mathf.Abs(velocity.magnitude)> velocityThreshold)
            {
                trigger?.Invoke();
            }

        }
    }
}
