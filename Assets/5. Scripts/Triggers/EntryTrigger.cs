using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{

    public class EntryTrigger : MonoBehaviour
    {


        [SerializeField, BoxGroup("Params")] private string triggerTag;
        [SerializeField, BoxGroup("Params")] private bool continuousCheck;
        [SerializeField, BoxGroup("Condition")] private TriggerCondition[] conditions;

        [SerializeField, BoxGroup("Actions")] private UnityEvent entryAction;
        [SerializeField, BoxGroup("Actions")] private UnityEvent exitAction;


        private bool _playerIsInTrigger;
        private bool _executed;
        private Coroutine _triggerReset;
        private Collider _interactCollider;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(triggerTag)) return;
            
            _playerIsInTrigger = true;
            _interactCollider = other;
        }


        private void OnTriggerStay(Collider other)
        {
            
            if(!continuousCheck) return;

            if (!other.CompareTag(triggerTag)) return;
            
            if (_triggerReset != null)
            {
                StopCoroutine(_triggerReset);
            }

            _triggerReset = StartCoroutine(ResetTrigger());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                Reset();
            }
        }

        private IEnumerator ResetTrigger()
        {

            yield return new WaitForSeconds(0.2f);

            Reset();
        }




        private void Update()
        {
            if (!_playerIsInTrigger || _executed) return;

            if (conditions.Length > 0)
            {
                foreach(TriggerCondition condition in conditions)
                {
                    var result = condition.Condition(_interactCollider);
                    if (!result)
                    {
                        return;
                    }
                }

                Execute();
            }
            else
            {
                Execute();
            }

        }

        private void Execute()
        {
            entryAction?.Invoke();
            _executed = true;
        }


        private void Reset()
        {

            _triggerReset = null;
            _interactCollider = null;
            _playerIsInTrigger = false;
            _executed = false;

            if (conditions.Length > 0)
            {
                bool result = true;

                foreach (TriggerCondition condition in conditions)
                {
                    result = condition.Condition(_interactCollider);
                    if (!result)
                    {
                        return;
                    }
                }

                exitAction?.Invoke();
            }
            else
            {
                exitAction?.Invoke();
            }


        }
    }

}