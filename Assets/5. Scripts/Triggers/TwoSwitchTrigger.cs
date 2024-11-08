using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class TwoSwitchTrigger : MonoBehaviour
    {

        [Required, FoldoutGroup("References")] public SimpleSwitch simpleSwitch1;
        [Required, FoldoutGroup("References")] public SimpleSwitch simpleSwitch2;


        [FoldoutGroup("Events"), InfoBox("true if first switch is triggered, false if second one")] public UnityEvent<bool> oneTriggeredEvents;
        [FoldoutGroup("Events")] public UnityEvent bothTriggeredEvents;
        [FoldoutGroup("Events")] public UnityEvent noneTriggeredEvents;
        
        

        #region Built-in Method
        
        private void OnEnable()
        {
            
            simpleSwitch1.triggeredEvents.AddListener(UpdateSwitches);
            simpleSwitch1.unTriggeredEvents.AddListener(UpdateSwitches);
            
            simpleSwitch2.triggeredEvents.AddListener(UpdateSwitches);
            simpleSwitch2.unTriggeredEvents.AddListener(UpdateSwitches);
            
        }
        public void OnDisable()
        {
            simpleSwitch1.triggeredEvents.RemoveListener(UpdateSwitches);
            simpleSwitch1.unTriggeredEvents.RemoveListener(UpdateSwitches);
            
            simpleSwitch2.triggeredEvents.RemoveListener(UpdateSwitches);
            simpleSwitch2.unTriggeredEvents.RemoveListener(UpdateSwitches);
        }

        #endregion
        

        private void UpdateSwitches()
        {
            bool isSwitch1Triggered = simpleSwitch1.Triggered;
            bool isSwitch2Triggered = simpleSwitch2.Triggered;

            if (isSwitch1Triggered && isSwitch2Triggered)
            {
                bothTriggeredEvents.Invoke();
            }
            else if (isSwitch1Triggered || isSwitch2Triggered)
            {
                oneTriggeredEvents.Invoke(isSwitch1Triggered);
            }
            else
            {
                noneTriggeredEvents.Invoke();
            }
            
        }
        
    }
}
