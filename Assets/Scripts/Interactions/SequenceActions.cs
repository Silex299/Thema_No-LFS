using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class SequenceActions : MonoBehaviour
    {

        public List<SequenceAction> actions;


        public void ExecuteTriggers()
        {
            StartCoroutine(ExecuteSequenceTriggers());
        }

        private IEnumerator ExecuteSequenceTriggers()
        {
            foreach (SequenceAction action in actions)
            {
                action.triggerEvent.Invoke();
                yield return new WaitForSeconds(action.interval);
            }

        }

        [System.Serializable]
        public struct SequenceAction
        {
            public UnityEvent triggerEvent;
            public float interval;
        }
    }
}
