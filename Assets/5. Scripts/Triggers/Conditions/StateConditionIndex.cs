using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Triggers.Conditions
{
    public class StateConditionIndex : TriggerCondition
    {

        [SerializeField, BoxGroup, EnumToggleButtons] private int index;
        public override bool Condition(Collider other)
        {
            return PlayerMovementController.Instance.VerifyState(index);
        }
    }
}
