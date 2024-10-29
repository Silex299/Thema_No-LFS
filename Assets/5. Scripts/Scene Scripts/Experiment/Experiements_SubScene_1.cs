using System;
using Managers.Checkpoints;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts
{
    // ReSharper disable once InconsistentNaming
    public class Experiements_SubScene_1 : MonoBehaviour
    {


        [FoldoutGroup("Scene Objects")] public Rigidbody firstRope;

        [FoldoutGroup("Other Params")] public Vector3 initialRopeForce;

        public Action checkpointLoad;
        
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
            CutsceneManager.Instance.PlayClip(0);
        }
        
    }
}
