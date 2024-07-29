using System.Collections;
using UnityEngine;

namespace Triggers
{
    public class BetterTriggerBase : MonoBehaviour
    {

        public string triggerTag;
        public bool continuousCheck;

        private Coroutine _resetTriggerCoroutine;
        private bool _playerInTrigger;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!enabled) return;
            if(other.CompareTag(triggerTag))
            {
                _playerInTrigger = true;
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag(triggerTag))
            {
                _playerInTrigger = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(!enabled) return;
        
            if(!continuousCheck) return;

            if (other.CompareTag(triggerTag))
            {

                _playerInTrigger = true;
                
                if(_resetTriggerCoroutine != null)
                {
                    StopCoroutine(_resetTriggerCoroutine);
                }
                _resetTriggerCoroutine = StartCoroutine(ResetTriggerCoroutine());

            }
        }
        
        private IEnumerator ResetTriggerCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        
    }
}
