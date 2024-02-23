using System.Collections.Generic;
using Interactable_Items.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class RotatingPuzzleTrigger : MonoBehaviour
    {


        [SerializeField] private float unlockOffset;
        [SerializeField] private List<UnlockTriggers> triggers;

        [SerializeField] private UnityEvent onTriggered;

        private bool isTriggered;

        private void Start()
        {

            foreach (var trigger in triggers)
            {
                trigger.trigger.OnTriggerValueChangedInternal += (float fraction) => { trigger.UpdateTrigger(fraction, unlockOffset); };
            }
        }

        private void OnDisable()
        {
            foreach (var trigger in triggers)
            {
                trigger.trigger.OnTriggerValueChangedInternal -= (float fraction) => { trigger.UpdateTrigger(fraction, unlockOffset); };
            }
        }

        private void Update()
        {

            if (isTriggered) return;

            bool allTriggered = true;

            foreach (var trigger in triggers)
            {
                allTriggered = allTriggered && trigger.isTriggerd;
            }

            if (allTriggered)
            {
                onTriggered?.Invoke();
                isTriggered = true;
            }

        }


        #region Custom type
        [System.Serializable]
        public class UnlockTriggers
        {
            public RotatingTrigger trigger;
            public float triggerThresold;
            public bool isTriggerd;

            public void UpdateTrigger(float fraction, float offset)
            {
                if (Mathf.Abs(fraction - triggerThresold) <= offset)
                {
                    isTriggerd = true;
                }
                else
                {
                    isTriggerd = false;
                }
            }
        }
        #endregion

    }
}

