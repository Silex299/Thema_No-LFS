using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Managers.Checkpoints
{

    public class GeneralTracker : Tracker
    {

        /// <summary>
        /// If tracker should be called during Start Call
        /// </summary>
        [SerializeField] private bool callToInitiate = true;
        /// <summary>
        /// If transforms should be tracked
        /// </summary>
        [SerializeField] private bool trackTransform = true;
        [SerializeField, TabGroup("States", "InitialState"), ShowIf(nameof(trackTransform))] private Vector3 initialPosition;
        [SerializeField, TabGroup("States", "InitialState"), ShowIf(nameof(trackTransform))] private Vector3 initialEualrRotation;
        [SerializeField, TabGroup("States", "FinalState"), ShowIf(nameof(trackTransform))] private Vector3 finalPosition;
        [SerializeField, TabGroup("States", "FinalState"), ShowIf(nameof(trackTransform))] private Vector3 finalEularRotation;



        [SerializeField, Space(10), InfoBox("Called when checkpoint is less than or equal to thresold checkpoint")] private UnityEvent initialStateAction;
        [SerializeField, InfoBox("Called when checkpoint is greater than thresold checkpoint")] private UnityEvent finalStateAction;


        public override void InitialSetup(CheckPoint checkPoint)
        {

            if (!callToInitiate) return;

            ResetItem(checkPoint);

        }

        public override void ResetItem(CheckPoint checkpoint)
        {

            if (CheckpointManager.Instance.CurrentCheckpoint > thresholdCheckpoint)
            {
                if (trackTransform)
                {
                    this.transform.localPosition = finalPosition;
                    this.transform.localRotation = Quaternion.Euler(finalEularRotation);
                }
                finalStateAction?.Invoke();
            }
            else
            {
                if (trackTransform)
                {
                    this.transform.localPosition = initialPosition;
                    this.transform.localRotation = Quaternion.Euler(initialEualrRotation);
                }
                initialStateAction?.Invoke();
            }
        }
    }

}