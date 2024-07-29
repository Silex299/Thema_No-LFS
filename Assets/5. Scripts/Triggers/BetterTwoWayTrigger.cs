using System.Linq;
using Player_Scripts;
using UnityEngine.Events;

namespace Triggers
{
    public class BetterTwoWayTrigger : BetterTriggerBase
    {

        public TriggerCondition[] conditions;
        
        public UnityEvent triggerEvent;
        public UnityEvent unTriggerEvent;
        
        
        private void Update()
        {
            if(!playerInTrigger) return;

            if (conditions.Any(condition => !condition.Condition(PlayerMovementController.Instance.player.CController)))
            {
                if (!triggered) return;
                
                unTriggerEvent.Invoke();
                triggered = false;
            }
            else
            {
                if(triggered) return;
                triggerEvent.Invoke();
                triggered = true;
            }
            
        }
    }
}
