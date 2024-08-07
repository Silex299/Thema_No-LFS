using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Triggers
{
    public class StayTrigger : MonoBehaviour
    {


        public string triggerTag;
        public List<TriggerConditionBase> conditions;
        [Tooltip("Make it a large value if you want to reset on exit")]public float secondActionDelay;
        [Tooltip("Resets the trigger on exit")]public bool exitReset = true;

        public UnityEvent triggerAction;
        
        private bool _triggered;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                if(_triggered) return;
                
                if (conditions.All(condition => condition.Condition(other)))
                {
                    _triggered = true;
                    triggerAction.Invoke();
                    StartCoroutine(ResetTrigger());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (exitReset)
            {
                _triggered = false;
            }
        }
        
        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(secondActionDelay);
            _triggered = false;
        }
    }
}
