using UnityEngine;

namespace Mechanics.Player.Conditions
{
    public abstract class TriggerCondition : MonoBehaviour
    {
        public abstract bool Condition(Object other);

    }
}