using System.Collections;
using UnityEngine;

namespace Triggers
{
    public class BetterTriggerBase : MonoBehaviour
    {

        public string triggerTag;
        public bool continuousCheck;

        private Coroutine _resetTriggerCoroutine;
        protected bool playerInTrigger;
        protected bool triggered;
        
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterBool(other);
        }  
        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitBool(other);
        }
        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayBool(other);
        }

        
        
        protected virtual bool OnTriggerEnterBool(Collider other)
        {
            if(!enabled) return false;
            if (!other.CompareTag(triggerTag)) return false;
            
            playerInTrigger = true;
            return true;
        }
    
        protected virtual bool OnTriggerExitBool(Collider other)
        {
            if (!other.CompareTag(triggerTag)) return false;
            
            playerInTrigger = false;
            return true;
        }

        protected virtual bool OnTriggerStayBool(Collider other)
        {
            if(!enabled) return false;
            if(!continuousCheck) return false;
            if (!other.CompareTag(triggerTag)) return false;
            
            playerInTrigger = true;
            if(_resetTriggerCoroutine != null)
            {
                StopCoroutine(_resetTriggerCoroutine);
            }
            _resetTriggerCoroutine = StartCoroutine(ResetTriggerCoroutine());
            return true;
        }
        
        

        private IEnumerator ResetTriggerCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        
    }
}
