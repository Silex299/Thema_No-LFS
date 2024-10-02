using System;
using Managers.Checkpoints;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.Canine
{
    public class CanineRagdoll : MonoBehaviour
    {
        
        public Rigidbody[] allRagdollRigidBodies;
        public Animator animator;


        private void OnEnable()
        {
            Reset();
            CheckpointManager.Instance.onCheckpointLoad += Reset;
        }

        [Button]
        public void PlayRagdoll()
        {
            //disable kinematic on all rigidbodies
            foreach (var rigidBody in allRagdollRigidBodies)
            {
                rigidBody.isKinematic = false;
            }
            animator.enabled = false;
        }


        private void Reset(int index = 0)
        {
            animator.enabled = true;
            //enable kinematic on all rigidbodies
            foreach (var rigidBody in allRagdollRigidBodies)
            {
                rigidBody.isKinematic = true;
            }
        }
    }
}
