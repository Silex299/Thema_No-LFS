using System;
using Managers;
using UnityEngine;

namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        public Transform handSocket;
        public Vector3 offset;

        private Rigidbody _lastRigidBody;
        private bool _constrainY;
        private bool _swing;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void ExitState(Player player)
        {
        }


        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Rope", 0.1f, 0);
        }

        public override void UpdateState(Player player)
        {
            
            float input = Input.GetAxis("Vertical");
            player.AnimationController.SetFloat(Speed, input);

            return;
            _constrainY = (Mathf.Abs(input) < 0.1f);

            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                if (_lastRigidBody)
                {
                    _lastRigidBody.AddForce(0, 0, horizontalInput * Time.deltaTime * 200);
                    player.transform.parent = _lastRigidBody.transform;
                    _swing = true;
                }
            }
        }


        public override void FixedUpdateState(Player player)
        {
           
            
            return;

            /**
            if (!_swing)
            {
                // sphere cast from the socket in forward direction
                if (Physics.SphereCast(handSocket.position, 0.1f, handSocket.forward, out RaycastHit hit, 1, ropeLayer))
                {
                    //debugDrawLine
                    Debug.DrawLine(handSocket.position, hit.point, Color.red);
                    _lastRigidBody = hit.collider.attachedRigidbody;
                }
            }
            **/
        }


        public override void LateUpdateState(Player player)
        {
            return;
            //Create a Layer mask only for the rope
            LayerMask ropeLayer = LayerMask.GetMask("Ignore Player");


            if (Physics.Raycast(handSocket.position, Vector3.forward, out RaycastHit hit1, 10f, ropeLayer))
            {
                Debug.DrawLine(handSocket.position, hit1.point, Color.red);
                _lastRigidBody = hit1.collider.attachedRigidbody;

                Vector3 newPos = _lastRigidBody.transform.position + offset;
                player.transform.position = newPos;
                
                
                _lastRigidBody.AddForce(0, 0, 200 * Mathf.Sin(Time.deltaTime));

            }
            
        }
    }
}