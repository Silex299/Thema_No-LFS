using Sirenix.OdinInspector;
using UnityEngine;


namespace Managers.Checkpoints
{

    public class DraggableTracker : Tracker
    {


        [SerializeField] private Player_Scripts.Interactables.DraggableCart draggableCart;

        [SerializeField, TabGroup("States", "InitialState")] private Vector3 initialPosition;
        [SerializeField, TabGroup("States", "InitialState")] private Vector3 initialEualrRotation;
        [SerializeField, TabGroup("States", "FinalState")] private Vector3 finalPosition;
        [SerializeField, TabGroup("States", "FinalState")] private Vector3 finalEularRotation;



        public override void InitialSetup(CheckPoint checkPoint)
        {
            if (CheckpointManager.Instance.CurrentCheckpoint > thresholdCheckpoint)
            {
                //SETUP FOR SECOND 
            }
            else
            {
                this.transform.localPosition = initialPosition;
                this.transform.localRotation = Quaternion.Euler(initialEualrRotation);

            }
        }

        public override void ResetItem(CheckPoint checkpoint)
        {

            if (CheckpointManager.Instance.CurrentCheckpoint > thresholdCheckpoint)
            {
                //SETUP FOR SECOND 
            }
            else
            {
                this.transform.localPosition = initialPosition;
                this.transform.localRotation = Quaternion.Euler(initialEualrRotation);
                draggableCart.Reset();
            }
        }
    }

}
