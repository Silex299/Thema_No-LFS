using Sirenix.OdinInspector;
using UnityEngine;
using Player_Scripts;

namespace Triggers
{
    public class StateCondition : TriggerCondition
    {

        [SerializeField, BoxGroup, EnumToggleButtons] private PlayerMovementState conditionState;
        public override bool Condition(Collider other)
        {
            return PlayerMovementController.Instance.VerifyState(conditionState);
        }
    }
}
