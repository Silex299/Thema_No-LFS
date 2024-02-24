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


        private void Start()
        {

            checkpoints[currentCheckpoint].InitialCheckpointLoad();

        }


        public void LoadLastCheckpoint()
        {
            print("Hello");
            checkpoints[currentCheckpoint].LoadCheckpoint();
        }

    }

}