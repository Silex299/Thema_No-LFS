using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    public class FractionTrigger : MonoBehaviour
    {
        [SerializeField] private float fractionThreshold;

        [SerializeField] private UnityEvent onFractionOverflow;
        [SerializeField] private UnityEvent onFractionUnderflow;

        private bool _overflowed;

        public void TriggerFraction(float fraction)
        {
            if (_overflowed)
            {
                if (!(fraction < fractionThreshold)) return;
                onFractionUnderflow?.Invoke();
                _overflowed = false;
            }
            else
            {
                if (!(fraction > fractionThreshold)) return;
                onFractionOverflow?.Invoke();
                _overflowed = true;

            }
        }

    }
}
