using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers.Checkpoints
{

    public class CheckpointManager : MonoBehaviour
    {

        [SerializeField] private int currentCheckpoint;


        [SerializeField, BoxGroup("Checkpoints")] private CheckPoint[] checkpoints;


#if UNITY_EDITOR
        [BoxGroup("Checkpoints"), Button("Get Checkpoints", ButtonSizes.Large)]
        public void GetCheckpoints()
        {
            checkpoints = GetComponentsInChildren<CheckPoint>();

            int i = 0;
            foreach (var cp in checkpoints)
            {
                cp.name = "checkpoint_" + i;
                i++;
            }
        }

#endif


        [SerializeField, BoxGroup("Trackers")] private PlayerCheckpointTracker playerTracker;
        [SerializeField, BoxGroup("Trackers")] private Tracker[] trackers;


        public int CurrentCheckpoint => currentCheckpoint;


        private static CheckpointManager instance;
        public static CheckpointManager Instance => instance;

        private void Awake()
        {
            if(CheckpointManager.Instance == null)
            {
                CheckpointManager.instance = this;
            }
            else if(CheckpointManager.Instance != this)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            playerTracker.InitialSetup(checkpoints[currentCheckpoint]);
            checkpoints[currentCheckpoint].LoadThisCheckpoint();

            SetTrackers(true);
        }


        public void RespawnPlayer()
        {
            playerTracker.ResetItem(checkpoints[currentCheckpoint]);
            checkpoints[currentCheckpoint].LoadThisCheckpoint();
            SetTrackers(false);
        }


        public void SetTrackers(bool initialSetup)
        {
            foreach (Tracker tracker in trackers)
            {
                if (initialSetup)
                {
                    tracker.InitialSetup(checkpoints[currentCheckpoint]);
                }
                else
                {
                    tracker.ResetItem(checkpoints[currentCheckpoint]);
                }

            }
        }
    }

}