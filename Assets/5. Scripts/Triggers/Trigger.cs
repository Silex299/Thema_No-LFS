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
        [SerializeField] private UnityEvent exitActions;
        [SerializeField] private bool isEnabled = true;

        private bool _isTriggered;
        private float _lastTriggerTime;
        private bool _playerIsInTrigger;
        private Collider _interactCollider;
        private Coroutine _reset;

        protected virtual void OnTriggerStay(Collider other)
        {

            if (other.CompareTag(triggerTag))
            {
                if (_reset != null)
                {
                    StopCoroutine(_reset);
                }

                _reset = StartCoroutine(ResetTrigger());
                
                if (_isTriggered && oneTime) return;


                _playerIsInTrigger = true;
                _interactCollider = other;

            }
        }


        public IEnumerator ResetTrigger()
        {

            yield return new WaitForSeconds(0.2f);

            _playerIsInTrigger = false;
            //_isTriggered = false;
            _interactCollider = null;

            exitActions?.Invoke();
        }



        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if ((oneTime && _isTriggered) || !isEnabled) return;

            if (Time.time < secondActionDelay + _lastTriggerTime) return;

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