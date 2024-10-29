using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Path_Scripts
{
    public class OverridePath : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Base"), InfoBox("Ignore For Two way override Path")] private Transform nextTransform;
        [SerializeField, FoldoutGroup("Base")] private Transform prevTransform;
        [FoldoutGroup("Base")] public bool useBothAxes;


        private Coroutine _triggerCoroutine;
        private bool _triggered;
        

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                print("Player In Trigger");
                if (!_triggered)
                {
                    _triggered = true;
                    PlayerPathController.Instance.overridePath = this;
                } 
                
                if(_triggerCoroutine!=null) StopCoroutine(_triggerCoroutine);
                _triggerCoroutine = StartCoroutine(ResetTrigger());
            }
        }

        IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            
            _triggered = false;
            if (PlayerPathController.Instance.overridePath == this)
            {
                PlayerPathController.Instance.overridePath = null;
            }
        }

        public virtual Vector3 GetNextPosition(float input, float otherInput)
        {
            return input > 0 ? nextTransform.position : prevTransform.position;
        }
    }
}
