using UnityEngine;

namespace Triggers
{
    public abstract class TriggerCondition : MonoBehaviour
    {
        public abstract bool Condition(Collider other);

    }
}