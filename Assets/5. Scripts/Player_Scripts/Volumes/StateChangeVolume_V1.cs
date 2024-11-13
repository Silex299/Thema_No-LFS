using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExternPropertyAttributes;
using Misc;
using Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Volumes
{
    public class StateChangeVolumeV1 : MonoBehaviour
    {
        
        
        [BoxGroup("Conditions")] public List<TriggerCondition> conditions;
        
        [SerializeField, BoxGroup("ENTRY STATE")] private PlayerStateInstances entryState;
        [SerializeField, BoxGroup("ENTRY STATE")] private UnityEvent entryAction;

        [SerializeField, BoxGroup("EXIT STATE")] private bool overrideConditionCheck;
        [SerializeField, BoxGroup("EXIT STATE")] private PlayerStateInstances exitState;
        [SerializeField, BoxGroup("EXIT STATE")] private UnityEvent exitAction;

        
        [BoxGroup("Misc")] public bool continuousCheck = true;
        [BoxGroup("Misc")] public float recheckTime = 0.2f;
        
        
        private bool _triggered;
        private Coroutine _resetCoroutine;
        

        private void OnTriggerStay(Collider other)
        {
            if (!continuousCheck) return;
            if (!other.CompareTag("Player_Main")) return;
            
            EntryAction(other);
            if(_resetCoroutine!=null) StopCoroutine(_resetCoroutine);
            _resetCoroutine = StartCoroutine(ResetTrigger(other));
        }
        private void OnTriggerEnter(Collider other)
        {
            if(continuousCheck) return;
            if (other.CompareTag("Player_Main"))
            {
                EntryAction(other);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(continuousCheck) return;
            if (other.CompareTag("Player_Main"))
            {
               ExitAction(other);
            }
        }

        private IEnumerator ResetTrigger(Collider other)
        {
            yield return new WaitForSeconds(recheckTime);
            
            ExitAction(other);
            _resetCoroutine = null;
        }

        
        private void EntryAction(Collider other)
        {
            if(_triggered) return;
            if (conditions.Any(condition => !condition.Condition(other))) return;
                        
            print(gameObject.name);
            entryState?.ChangeState();
            _triggered = true;
            
            entryAction?.Invoke();
        }

        private void ExitAction(Collider other)
        {
            if(!_triggered) return;
            if (!overrideConditionCheck)
            {
                if (conditions.Any(condition => !condition.Condition(other))) return;
            }
            
            print("DAFAK");
            exitState?.ChangeState();
            _triggered = false;
            exitAction?.Invoke();
        }
        
        
    }
}
