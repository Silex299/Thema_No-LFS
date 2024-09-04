using System;
using Sirenix.OdinInspector;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers.Checkpoints
{

    public class CheckpointManager : MonoBehaviour
    {


        [SerializeField] private int currentCheckpoint;
        [SerializeField] private int currentLevelIndex;


        [SerializeField, BoxGroup("Checkpoints")] private CheckPoint[] checkpoints;

        [SerializeField, BoxGroup("Trackers")] private PlayerCheckpointTracker playerTracker;
        [SerializeField, BoxGroup("Trackers")] private Tracker[] trackers;


        #region Editor
        
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

        [BoxGroup("Trackers"), Button("Get Trackers", ButtonSizes.Large), GUIColor(0.1f, 1f, 1f)]
        public void GetTrackers()
        {
            playerTracker = FindObjectOfType<PlayerCheckpointTracker>();
            trackers = FindObjectsOfType<Tracker>();
        }

#endif
        
        #endregion


        public int CurrentCheckpoint => currentCheckpoint;
        
        public static CheckpointManager Instance { get; private set; }
        public Action<int> onCheckpointLoad;


        private void Awake()
        {
            if (CheckpointManager.Instance == null)
            {
                CheckpointManager.Instance = this;
            }
            else if (CheckpointManager.Instance != this)
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            //LOAD CHECKPOINT FIRST
            currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            LoadCheckpointData();
            LoadCheckpoint(true);
        }


        public void LoadCheckpoint(bool initialSetup = false)
        {
            playerTracker.ResetItem(checkpoints[currentCheckpoint]);
            checkpoints[currentCheckpoint].LoadThisCheckpoint();
            SetTrackers(initialSetup);
            onCheckpointLoad?.Invoke(currentCheckpoint);
        }


        private void SetTrackers(bool initialSetup)
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

                SaveCheckpointData();
            }
        }

        private static string SavePath => Path.Combine(Application.persistentDataPath, "Checkpoint.data");
        private void SaveCheckpointData()
        {
            CheckpointInfo data = new CheckpointInfo()
            {
                level = currentLevelIndex,
                checkpoint = currentCheckpoint
            };
            BinaryFormatter formatter = new BinaryFormatter();

            using FileStream stream = new FileStream(SavePath, FileMode.Create);
            formatter.Serialize(stream, data);
        }

        private void LoadCheckpointData()
        {
            if (File.Exists(SavePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(SavePath, FileMode.Open);
                CheckpointInfo data = (CheckpointInfo)formatter.Deserialize(stream);

                currentCheckpoint = currentLevelIndex!= data.level ? 0 : data.checkpoint;
            }
            else
            {
                Debug.LogError("No saved file" + SavePath);
                currentCheckpoint = 0;
            }
        }


        [System.Serializable]
        public struct CheckpointInfo
        {
            public int level;
            public int checkpoint;
        }

    }

}