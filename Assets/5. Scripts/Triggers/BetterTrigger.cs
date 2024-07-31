using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class BetterTrigger : BetterTriggerBase
    {
        
        public float resetAfterActionDelay;
        public TriggerCondition[] conditions;
    

        public UnityEvent action;
        public UnityEvent entryAction;
        public UnityEvent exitAction;
    
        private Coroutine _playerInTriggerCoroutine;
        private Coroutine _resetTriggerCoroutine;
        private float _lastActionTime;
        
        
        protected override bool OnTriggerEnterBool(Collider other)
        {
            if (!base.OnTriggerEnterBool(other)) return true;
            
            
            _playerInTriggerCoroutine ??= StartCoroutine(PlayerInTrigger(other));
            return true;
        }

        protected override bool OnTriggerExitBool(Collider other)
        {
            if (!base.OnTriggerExitBool(other)) return true;
            
            exitAction.Invoke();
            if (_playerInTriggerCoroutine != null)
            {
                StopCoroutine(_playerInTriggerCoroutine);
                _playerInTriggerCoroutine = null;
            }
            return true;
        }


        protected override bool OnTriggerStayBool(Collider other)
        {
            if (!base.OnTriggerStayBool(other)) return true;
            
            _playerInTriggerCoroutine ??= StartCoroutine(PlayerInTrigger(other));
            
            return true;
        }


        private IEnumerator PlayerInTrigger(Collider other)
        {
            if (!enabled)
            {
                _playerInTriggerCoroutine = null;
                yield break;
            }

            entryAction.Invoke();
            
            while (true)
            {
                if(conditions.All(condition => condition.Condition(other)))
                {
                    action.Invoke();
                    break;
                }
                
                yield return null;
            }
            
            yield return new WaitForSeconds(resetAfterActionDelay);
        }

    }
}
