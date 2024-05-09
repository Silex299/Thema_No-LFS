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

        private bool _isSwinging;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Action = Animator.StringToHash("Action");

        public override void ExitState(Player player)
        {
        }


        public override void EnterState(Player player)
        {
            player.AnimationController.CrossFade("Rope", 0.1f, 0);
        }

        public override void UpdateState(Player player)
        {
            if(_isSwinging) return;
            
            player.AnimationController.SetFloat(Speed, Input.GetAxis("Vertical"));
        }


        public override void FixedUpdateState(Player player)
        {

            var input = Input.GetAxis("Horizontal");
            player.AnimationController.SetFloat(Speed, input);

            if (Input.GetButton("Horizontal"))
            {
                if (Mathf.Abs(input) > 0.2f)
                {
                    connectedRope.SwingRope(Input.GetAxis("Horizontal"));
                    if (!_isSwinging)
                    {
                        player.AnimationController.SetBool(Action, true);
                        _isSwinging = true;
                    }
                }
            }
            else
            {
                
                if (_isSwinging)
                {
                    player.AnimationController.SetBool(Action, false);
                    _isSwinging = false;
                }
                
            }

        }

        public override void LateUpdateState(Player player)
        {

            var input = _isSwinging ? 0 : Input.GetAxis("Vertical");
            
            connectedRope.MovePlayer(input);
        }
    }
}