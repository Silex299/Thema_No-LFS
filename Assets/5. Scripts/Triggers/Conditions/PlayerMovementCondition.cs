using UnityEngine;
using Player_Scripts;

namespace Triggers
{


    public class PlayerMovementCondition : TriggerCondition
    {
        public override bool Condition(Collider other)
        {
            return !PlayerMovementController.Instance.player.DisablePlayerMovement;
        }
    }
}
