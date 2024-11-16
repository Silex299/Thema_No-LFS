using System;
using Sirenix.OdinInspector;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Managers.Checkpoints
{

    public class CheckpointManager : MonoBehaviour
    {


        [SerializeField] private CheckpointInfo checkpointInfo;

        [SerializeField, BoxGroup("Checkpoints")] private CheckPoint[] checkpoints;

        [SerializeField, BoxGroup("Trackers")] private PlayerCheckpointTracker playerTracker;


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
        }

#endif
        
        #endregion


        public int CurrentCheckpoint => checkpointInfo.checkpoint;
        
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
                Destroy(CheckpointManager.Instance);
                CheckpointManager.Instance = this;
            }
        }

        private void OnEnable()
        {
            checkpointInfo = SceneManager.Instance.savedCheckpointInfo;
            checkpointInfo.level = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            LoadCheckpoint();
        }


        public void LoadCheckpoint()
        {
            playerTracker.ResetItem(checkpoints[checkpointInfo.checkpoint]);
            checkpoints[checkpointInfo.checkpoint].LoadThisCheckpoint();
            onCheckpointLoad?.Invoke(checkpointInfo.checkpoint);
        }
        
        public void SaveCheckpoint(int index)
        {
            checkpointInfo.checkpoint = index;

            if (SceneManager.Instance.CanSaveCheckpoint(checkpointInfo))
            {
                SceneManager.Instance.SaveCheckpointData(checkpointInfo);
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