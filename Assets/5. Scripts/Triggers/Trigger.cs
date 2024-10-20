using System;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class Trigger : MonoBehaviour
    {
        [SerializeField, BoxGroup("Properties"), Space(5)]
        protected string triggerTag;
        [SerializeField, BoxGroup("Properties")]
        private bool oneTime;
        [SerializeField, BoxGroup("Properties")]
        private bool entryTrigger;
        [SerializeField, BoxGroup("Properties")]
        private float secondActionDelay = 1;

        [SerializeField, BoxGroup("Properties"), Space(5)]
        private TriggerCondition[] conditionComponent;


        [SerializeField, Space(10)] private UnityEvent actions;
        [SerializeField] private UnityEvent exitActions;
        [SerializeField] private bool isEnabled = true;

        private bool _isTriggered;
        private float _lastTriggerTime;
        private bool _playerIsInTrigger;
        private Collider _interactCollider;
        private Coroutine _reset;

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }
        

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

            if (entryTrigger)
            {
                _isTriggered = false;
            }

            exitActions?.Invoke();
        }


        private void Update()
        {
            if (!_playerIsInTrigger) return;

            if ((oneTime && _isTriggered) || !isEnabled) return;

            if (Time.time < secondActionDelay + _lastTriggerTime) return;

            if (conditionComponent.Length > 0)
            {
                foreach (var component in conditionComponent)
                {
                    var result = component.Condition(_interactCollider);

                    if (!result)
                    {
                        return;
                    }
                }

                PerformAction();
            }
            else
            {
                PerformAction();
            }
        }


        private void PerformAction()
        {
            actions?.Invoke();

            print("Triggered");
            
            _isTriggered = true;
            _lastTriggerTime = Time.time;
        }

        public void EnableTrigger(bool status)
        {
            isEnabled = status;
        }
        
    }
}