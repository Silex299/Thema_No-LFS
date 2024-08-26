using UnityEngine;

namespace Mechanics.Actions.Conditions
{
    public abstract class TriggerCondition : MonoBehaviour
    {
        public abstract bool Condition(Object other);

    }
}