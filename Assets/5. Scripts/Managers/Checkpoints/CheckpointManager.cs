using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers.Checkpoints
{

    public class CheckpointManager : MonoBehaviour
    {

        [SerializeField] private int currentCheckpoint;


        [SerializeField, BoxGroup("Checkpoints")] private CheckPoint[] checkpoints;


#if UNITY_EDITOR
        [BoxGroup("Checkpoints"), Button("Get Checkpoints", ButtonSizes.Large), GUIColor(0.1f, 1f, 0.2f)]
        public void GetCheckpoints()
        {
            checkpoints = GetComponentsInChildren<CheckPoint>();

            int i = 0;
            foreach (var cp in checkpoints)
            {
                cp.name = "checkpoint_" + i;
                cp.checkpointIndex = i;
                i++;
            }
        }

#endif


        [SerializeField, BoxGroup("Trackers")] private PlayerCheckpointTracker playerTracker;
        [SerializeField, BoxGroup("Trackers")] private Tracker[] trackers;

#if UNITY_EDITOR

        [BoxGroup("Trackers"), Button("Get Trackers", ButtonSizes.Large), GUIColor(0.1f, 1f, 1f)]
        public void GetTrackers()
        {
            playerTracker = FindObjectOfType<PlayerCheckpointTracker>();
            trackers = FindObjectsOfType<Tracker>();
        }

#endif


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
   
        public void SaveCheckpoint(int index)
        {
            if (currentCheckpoint < index)
            {
                currentCheckpoint = index;
                print("Save Checkpoint");
            }
        }
    
    }

}