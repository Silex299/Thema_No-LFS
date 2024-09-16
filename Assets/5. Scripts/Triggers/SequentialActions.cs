using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class SequentialActions : MonoBehaviour
    {
        [SerializeField] private TimedAction[] timedActions;


        public void Execute()
        {
            StartCoroutine(ExecuteActions());
        }

        private IEnumerator ExecuteActions()
        {
            foreach (TimedAction action in timedActions)
            {
                action.action?.Invoke();
                yield return new WaitForSeconds(action.time);
            }
        }

        [System.Serializable]
        public struct TimedAction
        {
            public UnityEvent action;
            public float time;
        }

    }

}