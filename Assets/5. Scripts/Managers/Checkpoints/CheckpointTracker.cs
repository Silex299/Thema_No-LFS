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
        [InfoBox("If threshold is same as the loaded checkpoint, it will call the reset event, or final State Event")]
        public int checkpointThreshold;

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
            StartCoroutine(checkpointIndex <= checkpointThreshold ? ResetState() : FinalState());
        }

        private IEnumerator ResetState()
        {
            yield return new WaitForEndOfFrame();
            resetEvent.Invoke();
            if (!loadResetTransform) yield break;
            transform.localPosition = resetPosition;
            transform.localRotation = Quaternion.Euler(resetRotation);
        }

        private IEnumerator FinalState()
        {
            yield return new WaitForEndOfFrame();

            finalStateEvent.Invoke();
            if (!loadFinalStateTransform)  yield break;
            transform.localPosition = finalStatePosition;
            transform.localRotation = Quaternion.Euler(finalStateRotation);
        }
    }
}