using System.Collections;
using UnityEngine;

namespace Path_Scripts
{
    public class OverridePath : MonoBehaviour
    {
        public Transform nextTransform;
        public Transform prevTransform;


        private Coroutine _triggerCoroutine;
        protected bool triggered;


        public virtual Vector3 NextPoint => nextTransform.position;
        public virtual Vector3 PreviousPoint => prevTransform.position;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (!triggered)
                {
                    triggered = true;
                    PlayerPathController.Instance.overridePath = this;
                } 
                
                if(_triggerCoroutine!=null) StopCoroutine(_triggerCoroutine);
                _triggerCoroutine = StartCoroutine(ResetTrigger());
            }
        }

        IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            
            triggered = false;
            if (PlayerPathController.Instance.overridePath == this)
            {
                PlayerPathController.Instance.overridePath = null;
            }
        }
    }
}
