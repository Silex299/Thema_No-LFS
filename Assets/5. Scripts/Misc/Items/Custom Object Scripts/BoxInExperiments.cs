using System;
using System.Collections;
using Managers.Checkpoints;
using Misc;
using Misc.Items;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Custom_Object_Scripts
{
    public class BoxInExperiments : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Properties")]
        private Animator animator;

        [SerializeField, FoldoutGroup("Properties")]
        private float initialDelay;

        [SerializeField, FoldoutGroup("Properties")]
        private Rigidbody rb;

        [SerializeField, FoldoutGroup("Properties")]
        private Rope[] connectedRopes;

        [SerializeField, FoldoutGroup("Properties")]
        private PlayerSceneAnimatonManager sceneAnim;

        [SerializeField, FoldoutGroup("Properties")]
        private UnityEvent onBoxFall;

        [FoldoutGroup("Checkpoint")] public Vector3 finalPos;
        [FoldoutGroup("Checkpoint")] public Vector3 finalRot;

        private bool _triggered;


        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += OnCheckpointLoad;
        }
        
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= OnCheckpointLoad;
        }


        public void FallBox()
        {
            StartCoroutine(StartAction());
        }

        private IEnumerator StartAction()
        {
            if (!PlayerMovementController.Instance.VerifyState(PlayerMovementState.BasicMovement))
            {
                yield break;
            }

            _triggered = true;
            yield return new WaitForSeconds(initialDelay);

            connectedRopes[0].BreakRope();
            yield return new WaitForSeconds(1.5f);
            connectedRopes[1].BreakRope();
            rb.isKinematic = true;
            animator.enabled = true;
            sceneAnim.PlayPlayerSceneAnimation(0);
        }

        public void EnableRopeTrigger()
        {
            onBoxFall.Invoke();
        }


        private void OnCheckpointLoad(int checkpoint)
        {
            if (checkpoint <= 2) return;
            FallBox();
        }
    }
}