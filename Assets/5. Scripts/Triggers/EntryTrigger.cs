using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{

    public class EntryTrigger : MonoBehaviour
    {


        #region  Variables

        
        
        [SerializeField, BoxGroup("Params")] private string triggerTag;
        [SerializeField, BoxGroup("Params")] private bool continuousCheck;
        [SerializeField, BoxGroup("Condition")] private TriggerCondition[] conditions;

        [SerializeField, BoxGroup("Actions")] private UnityEvent entryAction;
        [SerializeField, BoxGroup("Actions")] private UnityEvent exitAction;


        #endregion


        #region Private Variables

        private bool _playerIsInTrigger;
        private bool _executed;
        private Coroutine _triggerReset;
        private Collider _interactCollider;

        #endregion


        #region Built in methods

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

            if (!_playerIsInTrigger)
            {
                _playerIsInTrigger = true;
                _interactCollider = other;
            }
            
            if (_triggerReset != null)
            {
                StopCoroutine(_triggerReset);
            }
            _triggerReset = StartCoroutine(ResetTriggerCoroutine());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                ResetTrigger();
            }
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
        
        #endregion

        #region Custom Methods

        private IEnumerator ResetTriggerCoroutine()
        {

            yield return new WaitForSeconds(0.2f);

            ResetTrigger();
        }
        private void Execute()
        {
            entryAction?.Invoke();
            _executed = true;
        }
        private void ResetTrigger()
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
        
        #endregion
    }

}