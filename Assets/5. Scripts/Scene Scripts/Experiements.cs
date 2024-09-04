using Managers;
using Managers.Checkpoints;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts
{
    public class Experiements : MonoBehaviour
    {


        [FoldoutGroup("Scene Objects")] public Rigidbody firstRope;
        [FoldoutGroup("Scene Objects")] public Player player;

        [FoldoutGroup("Other Params")] public Vector3 initialRopeForce;
        
        private void Start()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnNewCheckpointLoad;
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnNewCheckpointLoad;
        }

        private void OnNewCheckpointLoad(int checkpoint)
        {
            if (checkpoint == 0)
            {
                InitialSceneSetup();
            }
        }

        [Button]
        public void InitialSceneSetup()
        {
            firstRope.AddForce(initialRopeForce, ForceMode.Impulse);
            UIManager.Instance.FadeOut(0.5f);
            player.AnimationController.Play("Fall", 1);
        }
        
    }
}
