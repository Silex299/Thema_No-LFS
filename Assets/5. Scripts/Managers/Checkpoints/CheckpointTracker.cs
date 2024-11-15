using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Managers.Checkpoints
{
    public class CheckpointTracker : MonoBehaviour
    {
        [InfoBox("If threshold is less or equal as the loaded checkpoint, it will call the reset event, or final State Event")]
        [InfoBox("If set to exclusive, Reset function will call only for current Checkpoint and Final for other")]
        [BoxGroup("Misc")] public int checkpointThreshold;
        [BoxGroup("Misc")] public bool isExclusive;
        [BoxGroup("Misc")] public Transform targetTransform;

        [TabGroup("State", "Reset")] public bool loadResetTransform;
        [TabGroup("State", "Reset"), ShowIf(nameof(loadResetTransform))]
        public Vector3 resetPosition;
        [TabGroup("State", "Reset"), ShowIf(nameof(loadResetTransform))]
        public Vector3 resetRotation;

        [TabGroup("State", "Reset")] public UnityEvent resetEvent;
        [TabGroup("State", "Final")] public bool loadFinalStateTransform;
        [TabGroup("State", "Final"), ShowIf(nameof(loadFinalStateTransform))]
        public Vector3 finalStatePosition;
        [TabGroup("State", "Final"), ShowIf(nameof(loadFinalStateTransform))]
        public Vector3 finalStateRotation;
        [TabGroup("State", "Final")] public UnityEvent finalStateEvent;


        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoaded;
        }

        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoaded;
        }

        private void OnCheckpointLoaded(int checkpointIndex)
        {
            if (isExclusive)
            {
                StartCoroutine(checkpointIndex == checkpointThreshold ? ResetState() : FinalState());
            }
            else
            {
                StartCoroutine((checkpointIndex <= checkpointThreshold || checkpointIndex == -1) ? ResetState() : FinalState());
            }
        }

        private IEnumerator ResetState()
        {
            yield return new WaitForEndOfFrame();
            resetEvent.Invoke();
            if (!loadResetTransform) yield break;
            if (targetTransform)
            {
                targetTransform.localPosition = resetPosition;
                targetTransform.localRotation = Quaternion.Euler(resetRotation);
            }
            else
            {
                transform.localPosition = resetPosition;
                transform.localRotation = Quaternion.Euler(resetRotation);
            }
        }

        private IEnumerator FinalState()
        {
            yield return new WaitForEndOfFrame();

            finalStateEvent.Invoke();
            if (!loadFinalStateTransform)  yield break;
            
            if (targetTransform)
            {
                targetTransform.localPosition = finalStatePosition;
                targetTransform.localRotation = Quaternion.Euler(finalStateRotation);
            }
            else
            {
                transform.localPosition = finalStatePosition;
                transform.localRotation = Quaternion.Euler(finalStateRotation);
            }
        }
    }
}