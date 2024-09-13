using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExternPropertyAttributes;
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

        [SerializeField, BoxGroup("EXIT STATE")] private PlayerStateInstances exitState;
        [SerializeField, BoxGroup("EXIT STATE")] private UnityEvent exitAction;

        
        [BoxGroup("Misc")] public bool continuousCheck = true;
        [BoxGroup("Misc")] public float recheckTime = 0.2f;
        
        
        private bool _triggered;
        private Coroutine _resetCoroutine;
        

        private void OnTriggerStay(Collider other)
        {
            if (continuousCheck)
            {
                if (other.CompareTag("Player_Main"))
                {
                    if (!_triggered)
                    {
                        EntryAction(other);
                    }
                    else
                    {
                        if (_resetCoroutine == null)
                        {
                            _resetCoroutine = StartCoroutine(ResetTrigger());
                        }
                        else
                        {
                            StopCoroutine(_resetCoroutine);
                            _resetCoroutine = null;
                        }
                    }
                }
                
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.CompareTag("Player_Main"))
            {
                EntryAction(other);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
               ExitAction(other);
            }
        }

        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(recheckTime);
            
            exitState.ChangeState();
            _triggered = false;
            _resetCoroutine = null;
        }

        
        private void EntryAction(Collider other)
        {
            if (conditions.Any(condition => !condition.Condition(other))) return;
                        
            print(gameObject.name);
            entryState.ChangeState();
            _triggered = true;
            
            entryAction?.Invoke();
        }

        private void ExitAction(Collider other)
        {
            if (conditions.Any(condition => !condition.Condition(other))) return;
            
            exitState.ChangeState();
            _triggered = false;
            exitAction?.Invoke();
        }
        
        
    }
}
