using Misc.Items;
using UnityEngine;

namespace Player_Scripts.States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseStates
    {
        public Rope connectedRope;
        public Transform handSocket;
        public Vector3 offset;

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
            player.AnimationController.SetFloat(Speed, Input.GetAxis("Vertical"));

        }


        public override void FixedUpdateState(Player player)
        {

            if (Input.GetButton("Horizontal"))
            {
                connectedRope.SwingRope(Input.GetAxis("Horizontal"));
            }

        }


        public override void LateUpdateState(Player player)
        {
            connectedRope.MovePlayer(Input.GetAxis("Vertical"));
        }
    }
}