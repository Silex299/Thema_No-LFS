using Managers.Checkpoints;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts
{
    public class Experiements : MonoBehaviour
    {


        [FoldoutGroup("Scene Objects")] public Rigidbody firstRope;
        [FoldoutGroup("Scene Objects")] public CutsceneManager cutsceneManager;

        [FoldoutGroup("Other Params")] public Vector3 initialRopeForce;
        
        private void OnEnable()
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
            cutsceneManager.PlayClip(3);
        }
        
    }
}
