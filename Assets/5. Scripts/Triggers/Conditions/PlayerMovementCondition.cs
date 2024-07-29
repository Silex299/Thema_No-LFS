using UnityEngine;
using Player_Scripts;
using Sirenix.OdinInspector;

namespace Triggers
{
    public class PlayerMovementCondition : TriggerCondition
    {
        [InfoBox("If you want to trigger if player movement is not disabled, uncheck this box. If you want to trigger if player movement is disabled, check this box.")] 
        [SerializeField] private bool invertedMovementCheck;
        [SerializeField] private bool checkForPlayerMovement = true;
        
        [Space(10), InfoBox("If you want to trigger if player is grounded, uncheck this box. If you want to trigger if player is not grounded, check this box.")] 
        [SerializeField] private bool invertedGroundCheck;
        [SerializeField] private bool checkForGrounded;

        public override bool Condition(Collider other)
        {
            Player player = PlayerMovementController.Instance.player;

            if (checkForGrounded && checkForPlayerMovement)
            {
                return !invertedMovementCheck ? !player.DisabledPlayerMovement :
                    player.DisabledPlayerMovement && !invertedGroundCheck ? player.IsGrounded : !player.IsGrounded;
            }
            else if (checkForPlayerMovement)
            {
                return !invertedMovementCheck ? !player.DisabledPlayerMovement : player.DisabledPlayerMovement;
            }
            else if (checkForGrounded)
            {
                return !invertedGroundCheck ? player.IsGrounded : !player.IsGrounded;
            }

            return false;
        }
    }
}