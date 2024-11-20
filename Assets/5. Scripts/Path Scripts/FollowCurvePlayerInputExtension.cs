using System;
using ExternPropertyAttributes;
using Managers.Checkpoints;
using Player_Scripts;
using UnityEngine;

namespace Path_Scripts
{
    public class FollowCurvePlayerInputExtension : MonoBehaviour
    {

        [InfoBox("Automatically resets on checkpoint load")]
        public FollowCurvePathOverride parentPath;
        public float sprintMultiplier = 1.5f;
        [field: SerializeField]public bool GetPlayerInput { get; set; } = false;

        private float _initialSpeed;
        private Vector3 _initialRotationOffset;

        private Player Player => PlayerMovementController.Instance.player;

        private void Start()
        {
            _initialSpeed = parentPath.progressSpeed;
            _initialRotationOffset = parentPath.rotationOffset;
        }

        private void Update()
        {
            if(!GetPlayerInput) return;
            
            
            bool isSprinting = Mathf.Abs(Input.GetAxis("Sprint")) > 0.2f && Player.CanBoost;

            if (!Player.IsGrounded)
            {
                parentPath.progressSpeed = 0;
            }
            else if (isSprinting)
            {
                parentPath.progressSpeed = _initialSpeed * sprintMultiplier;
            }
            else
            {
                parentPath.progressSpeed = _initialSpeed;
            }
            

            if (Mathf.Abs(Player.UseHorizontal ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical")) > 0.1f)
            {
                bool isForward = (Player.UseHorizontal ? Input.GetAxis("Horizontal") >= 0 : Input.GetAxis("Vertical") >= 0);
                if (isForward)
                {
                    parentPath.rotationOffset = _initialRotationOffset;
                }
                else
                {
                    parentPath.rotationOffset = -_initialRotationOffset;
                }
            }
            
        }

        private void OnEnable()
        {
            CheckpointManager.Instance.onCheckpointLoad += ResetExtension;
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= ResetExtension;
            ResetExtension(0);
        }
        private void ResetExtension(int index)
        {
            GetPlayerInput = false;
        }
    }
}
