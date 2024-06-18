using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Triggers.Conditions
{
    public class AnimatorCondition : TriggerCondition
    {
        public string animationPropertyName;
        public bool inverted;

        public override bool Condition(Collider other)
        {
            return inverted
                ? !PlayerMovementController.Instance.player.AnimationController.GetBool(animationPropertyName)
                : PlayerMovementController.Instance.player.AnimationController.GetBool(animationPropertyName);
        }
    }
}