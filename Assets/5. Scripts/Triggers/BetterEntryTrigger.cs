using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class BetterEntryTrigger : MonoBehaviour
    {
        public string triggerTag;

        [BoxGroup("Entry")] public List<TriggerCondition> entryConditions;
        [BoxGroup("Entry")] public UnityEvent entryEvent;


        [BoxGroup("Exit")] public List<TriggerCondition> exitConditions;
        [BoxGroup("Exit")] public UnityEvent exitEvent;


        [BoxGroup("Misc")] public bool continuousCheck;
        [BoxGroup("Misc")] public float secondsToReset = 1f;


        private float _secondActionTime;
        private bool _triggered;
        private Coroutine _resetCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(triggerTag)) return;
            if (Time.time < _secondActionTime) return;

            if (entryConditions.Any(condition => !condition.Condition(other))) return;

            _secondActionTime = Time.time + secondsToReset;
            entryEvent.Invoke();
            _triggered = true;
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(triggerTag)) return;
            if (_secondActionTime > Time.time) return;

            if (exitConditions.Any(condition => !condition.Condition(other))) return;

            _secondActionTime = Time.time + secondsToReset;
            exitEvent.Invoke();
            _triggered = false;
        }


        private void OnTriggerStay(Collider other)
        {
            if (!continuousCheck) return;
            if (!other.CompareTag(triggerTag)) return;
            
            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
            }
            _resetCoroutine = StartCoroutine(ResetTrigger());
            
            if (_triggered) return;
            if (entryConditions.Any(condition => !condition.Condition(other))) return;
            _secondActionTime = Time.time + secondsToReset;
            entryEvent.Invoke();
            _triggered = true;
        }


        IEnumerator ResetTrigger()
        {
            if (!_triggered) yield break;
            
            yield return new WaitForSeconds(0.2f);

            _secondActionTime = Time.time + secondsToReset;
            exitEvent.Invoke();
            _triggered = false;
        }
    }
}