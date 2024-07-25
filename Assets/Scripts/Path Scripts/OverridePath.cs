using System.Collections;
using UnityEngine;

namespace Path_Scripts
{
    public class OverridePath : MonoBehaviour
    {
        public Transform nextTransform;
        public Transform prevTransform;


        private Coroutine _triggerCoroutine;
        private bool _triggered;
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if(_triggered) return;
                
                _triggered = true;
                PlayerPathController.Instance.overridePath = this;
                
                if(_triggerCoroutine!=null) StopCoroutine(_triggerCoroutine);
                _triggerCoroutine = StartCoroutine(ResetTrigger());
            }
        }

        IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _triggered = false;
            PlayerPathController.Instance.overridePath = null;
        }
    }
}
