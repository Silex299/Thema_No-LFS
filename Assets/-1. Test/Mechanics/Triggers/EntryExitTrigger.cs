using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Triggers
{
    public class EntryExitTrigger : MonoBehaviour
    {

        public string triggerTag;
        public bool continuousCheck;

        [Space(10)] public List<TriggerConditionBase> conditions;
        
        public UnityEvent entryTrigger;
        public UnityEvent exitTrigger;
        
        private bool _entryTriggered;
        private Coroutine _triggerExitCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                if(_entryTriggered) return;
                
                if (conditions.All(condition => condition.Condition(other)))
                {
                    entryTrigger.Invoke();
                    _entryTriggered = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                if(!_entryTriggered) return;
                
                if (conditions.All(condition => condition.Condition(other)))
                {
                    exitTrigger.Invoke();
                    _entryTriggered = false;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(!continuousCheck) return;

            if (other.CompareTag(triggerTag))
            {

                if (_triggerExitCoroutine != null)
                {
                    StopCoroutine(_triggerExitCoroutine);
                }

                _triggerExitCoroutine = StartCoroutine(TriggerExit(other));
                
                if(_entryTriggered) return;
                if (conditions.All(condition => condition.Condition(other)))
                {
                    entryTrigger.Invoke();
                    _entryTriggered = true;
                }
                
            }
        }

        private IEnumerator TriggerExit(Collider other)
        {
            yield return new WaitForSeconds(0.2f);
            
            if(!_entryTriggered) yield break;
                
            if (conditions.All(condition => condition.Condition(other)))
            {
                exitTrigger.Invoke();
                _entryTriggered = false;
            }

            _triggerExitCoroutine = null;
        }
    }
}
