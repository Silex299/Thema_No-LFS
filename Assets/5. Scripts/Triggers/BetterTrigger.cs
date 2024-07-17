using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class BetterTrigger : MonoBehaviour
    {

        public string triggerTag;
        public bool continuousCheck;
        public float resetAfterActionDelay;
        public TriggerCondition[] conditions;
    

        public UnityEvent action;
        private UnityEvent entryAction;
        private UnityEvent exitAction;
    
        private Coroutine _playerInTriggerCoroutine;
        private Coroutine _resetTriggerCoroutine;

    
        private void OnTriggerEnter(Collider other)
        {
            if(!enabled) return;
            if(other.CompareTag(triggerTag))
            {
                _playerInTriggerCoroutine ??= StartCoroutine(PlayerInTrigger(other));
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag(triggerTag))
            {
                if (_playerInTriggerCoroutine != null)
                {
                    StopCoroutine(_playerInTriggerCoroutine);
                    _playerInTriggerCoroutine = null;
                    exitAction.Invoke();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(!enabled) return;
        
            if(!continuousCheck) return;

            if (other.CompareTag(triggerTag))
            {
                _playerInTriggerCoroutine ??= StartCoroutine(PlayerInTrigger(other));
            
                if(_resetTriggerCoroutine != null)
                {
                    StopCoroutine(_resetTriggerCoroutine);
                }
                _resetTriggerCoroutine = StartCoroutine(ResetTriggerCoroutine());

            }
        }


        private IEnumerator PlayerInTrigger(Collider other)
        {
            if(!enabled) yield break;

            entryAction.Invoke();
            
            while (true)
            {
                bool result = true;
            
                foreach (var condition in conditions)
                {
                    result = condition.Condition(other);

                    if (!result)
                    {
                        break;
                    }
                }

                if (result)
                {
                    action.Invoke();
                    break;
                }
            
                yield return null;
            }
            
            yield return new WaitForSeconds(resetAfterActionDelay);
        
            StopCoroutine(_playerInTriggerCoroutine);
            _playerInTriggerCoroutine = null;

        }

        private IEnumerator ResetTriggerCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
            if (_playerInTriggerCoroutine!=null)
            {
                StopCoroutine(_playerInTriggerCoroutine);
                _playerInTriggerCoroutine = null;
            }
            exitAction.Invoke();
        }


    }
}
