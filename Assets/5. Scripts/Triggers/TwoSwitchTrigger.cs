using Sirenix.OdinInspector;
using UnityEngine;

namespace Triggers
{
    public class TwoSwitchTrigger : MonoBehaviour
    {

        [Required] public SimpleSwitch simpleSwitch1;
        [Required] public SimpleSwitch simpleSwitch2;


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
            
        }
        
        
        #region Switch 1

        private void SetSwitch1()
        {
            
        }
        private void ResetSwitch1()
        {
            
        }

        #endregion
        
        #region Switch 2
        
        private void SetSwitch2()
        {
            
        }
        private void ResetSwitch2()
        {
            
        }

        #endregion
    }
}
