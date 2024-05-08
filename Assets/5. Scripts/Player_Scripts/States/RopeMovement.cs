using System;
using Managers;
using UnityEngine;

namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        [SerializeField] private Transform handSocket;

        private Rigidbody _lastRigidBody;
        private bool _constrainY;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void ExitState(Player player)
        {
        }

        public override void LateUpdateState(Player player)
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

            _constrainY = (Mathf.Abs(input) < 0.1f);

            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                if (_lastRigidBody)
                {
                    _lastRigidBody.AddForce(0, 0, horizontalInput * Time.deltaTime * 200);
                    player.transform.parent = _lastRigidBody.transform;
                }
            }
            else
            {
                player.transform.parent = MasterManager.Instance.transform;
            }
        }


        public override void FixedUpdateState(Player player)
        {
            //Create a Layer mask only for the rope
            LayerMask ropeLayer = LayerMask.GetMask("Ignore Player");


            // sphere cast from the socket in forward direction
            if (Physics.SphereCast(handSocket.position, 0.1f, handSocket.forward, out RaycastHit hit, 1, ropeLayer))
            {
                //debugDrawLine
                Debug.DrawLine(handSocket.position, hit.point, Color.red);
                _lastRigidBody = hit.collider.attachedRigidbody;
            }


            if (_lastRigidBody)
            {
                Vector3 playerPos = player.transform.position;
                Vector3 rbPosition = _lastRigidBody.transform.position;

                Vector3 newPos = new Vector3(rbPosition.x, playerPos.y, rbPosition.z);


                player.transform.position = Vector3.Lerp(playerPos, newPos, Time.fixedDeltaTime * 10);
                player.transform.rotation = Quaternion.identity;

                _lastRigidBody.AddForce(0, 0, 200 * Mathf.Sin(Time.fixedDeltaTime));
            }
        }
    }
}