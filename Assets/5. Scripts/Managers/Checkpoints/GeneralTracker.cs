using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers.Checkpoints
{

    public class GeneralTracker : Tracker
    {

        [SerializeField, TabGroup("States", "InitialState")] private Vector3 initialPosition;
        [SerializeField, TabGroup("States", "InitialState")] private Vector3 initialEualrRotation;
        [SerializeField, TabGroup("States", "FinalState")] private Vector3 finalPosition;
        [SerializeField, TabGroup("States", "FinalState")] private Vector3 finalEularRotation;


        [SerializeField, Space(10)] private UnityEvent initialStateAction;
        [SerializeField] private UnityEvent finalStateAction;


        public override void InitialSetup(CheckPoint checkPoint)
        {

            ResetItem(checkPoint);

        }

        public override void ResetItem(CheckPoint checkpoint)
        {

            if (CheckpointManager.Instance.CurrentCheckpoint > thresholdCheckpoint)
            {
                this.transform.localPosition = finalPosition;
                this.transform.localRotation = Quaternion.Euler(finalEularRotation);
                finalStateAction?.Invoke();
            }
            else
            {
                this.transform.localPosition = initialPosition;
                this.transform.localRotation = Quaternion.Euler(initialEualrRotation);
                initialStateAction?.Invoke();
            }
        }
    }

}