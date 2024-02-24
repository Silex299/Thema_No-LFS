using Sirenix.OdinInspector;
using UnityEngine;


namespace Managers.Checkpoints
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField, BoxGroup("Player Info")] private Player_Scripts.PlayerMovementState playerState;
        [SerializeField, BoxGroup("Player Info")] private int playerStateIndex;

        [SerializeField, BoxGroup("Trackers")] private Tracker[] trackers;


        public void LoadCheckpoint()
        {
            foreach(Tracker tracker in trackers)
            {
                tracker.ResetItem(this);
            }
        }

        public void InitialCheckpointLoad()
        {
            foreach(Tracker tracker in trackers)
            {
                tracker.InitialSetup(this);
            }
        }

    }

}
