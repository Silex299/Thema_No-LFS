using UnityEngine;
using Player_Scripts;

namespace Triggers
{


    public class PlayerMovementCondition : TriggerCondition
    {
        [SerializeField] private bool checkForPlayerMovement = true;
        [SerializeField] private bool checkForGrounded;

        public override bool Condition(Collider other)
        {
            Player player = PlayerMovementController.Instance.player;

            if (checkForGrounded && checkForPlayerMovement)
            {
                return !player.DisabledPlayerMovement && player.IsGrounded;
            }
            else if(checkForPlayerMovement)
            {
                return !player.DisabledPlayerMovement;
            }
            else if (checkForGrounded)
            {
                return player.IsGrounded;
            }

            return false;
        }
    }
}
