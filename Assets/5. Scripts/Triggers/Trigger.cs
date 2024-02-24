using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class Trigger : MonoBehaviour
    {

        [SerializeField, BoxGroup("Properties"), Space(5)] protected string triggerTag;
        [SerializeField, BoxGroup("Properties")] private bool oneTime;
        [SerializeField, BoxGroup("Properties")] private float secondActionDelay = 1;
        [SerializeField, BoxGroup("Properties"), Space(5)] private TriggerCondition[] conditionComponent;


        [SerializeField, Space(10)] private UnityEvent actions;
        [SerializeField] private bool isEnabled = true;

        private bool _isTriggered;
        private float _lastTriggerTime;
        private bool _playerIsInTrigger;
        private Collider _interactCollider;
        private Coroutine reset;

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                _playerIsInTrigger = true;
                _interactCollider = other;

                if(reset!= null)
                {
                    StopCoroutine(reset);
                }

                reset = StartCoroutine(ResetTrigger());
            }
        }


        public IEnumerator ResetTrigger()
        {

            yield return new WaitForSeconds(0.2f);

            _playerIsInTrigger = false;
            _interactCollider = null;
        }



        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if ((oneTime && _isTriggered) || !isEnabled) return;


            if (conditionComponent.Length > 0)
            {
                bool result_ = true;

                foreach (var component in conditionComponent)
                {
                    result_ = result_ && component.Condition(_interactCollider);

                    if (!result_)
                    {
                        return;
                    }
                }

                if (result_)
                {
                    PerformAction();
                }

            }
            else
            {
                PerformAction();
            }
        }



        private void PerformAction()
        {
            if (_isTriggered && Time.time < secondActionDelay + _lastTriggerTime) return;

            actions?.Invoke();

            _isTriggered = true;
            _lastTriggerTime = Time.time;
            
        }

        public void EnableTrigger(bool status)
        {
            isEnabled = status;
        }

    }

}