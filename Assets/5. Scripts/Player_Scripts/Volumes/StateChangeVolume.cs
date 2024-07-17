
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Triggers;
using UnityEngine;
using UnityEngine.Events;


namespace Player_Scripts.Volumes
{
    [RequireComponent(typeof(Collider))]
    public class StateChangeVolume : MonoBehaviour
    {
        
        [BoxGroup("Conditions")] public List<TriggerCondition> conditions;
        
        [SerializeField, BoxGroup("ENTRY STATE")] private int entryStateIndex;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool enableEntryDirection;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool entryOneWayRotation;
        [SerializeField, BoxGroup("ENTRY STATE")] private UnityEvent entryAction;

        [SerializeField, BoxGroup("EXIT STATE")] private int exitStateIndex;
        [SerializeField, BoxGroup("EXIT STATE")] private bool enableExitDirection;
        [SerializeField, BoxGroup("EXIT STATE")] private bool exitOneWayRotation;

        [SerializeField, BoxGroup("EXIT STATE")]
        private UnityEvent exitAction;

        
        [BoxGroup("Misc")] public bool continuousCheck = true;
        [BoxGroup("Misc")] public float recheckTime = 0.2f;
        
        
        private bool _triggered;
        private Coroutine _resetCoroutine;
        

        private void OnTriggerStay(Collider other)
        {
            if (continuousCheck)
            {

                if (other.CompareTag("Player_Main"))
                {
                    if (!_triggered)
                    {
                        EntryAction(other);
                    }
                    else
                    {
                        if (_resetCoroutine == null)
                        {
                            _resetCoroutine = StartCoroutine(ResetTrigger());
                        }
                        else
                        {
                            StopCoroutine(_resetCoroutine);
                            _resetCoroutine = null;
                        }
                    }
                }
                
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.CompareTag("Player_Main"))
            {
                EntryAction(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("Player_Main"))
            {
                
               ExitAction(other);

            }
        }

        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(recheckTime);
            
            PlayerMovementController.Instance.ChangeState(exitStateIndex);
            PlayerMovementController.Instance.player.enabledDirectionInput = enableExitDirection;
            PlayerMovementController.Instance.player.oneWayRotation = exitOneWayRotation;
            _triggered = false;
            _resetCoroutine = null;
        }

        
        private void EntryAction(Collider other)
        {
            if (conditions.Any(condition => !condition.Condition(other))) return;
                        
            PlayerMovementController.Instance.ChangeState(entryStateIndex);
            PlayerMovementController.Instance.player.enabledDirectionInput = enableEntryDirection;
            PlayerMovementController.Instance.player.oneWayRotation = entryOneWayRotation;
            _triggered = true;
            
            entryAction?.Invoke();
        }

        private void ExitAction(Collider other)
        {
            if (conditions.Any(condition => !condition.Condition(other))) return;
                
            PlayerMovementController.Instance.ChangeState(exitStateIndex);
            PlayerMovementController.Instance.player.enabledDirectionInput = enableExitDirection;
            PlayerMovementController.Instance.player.oneWayRotation = exitOneWayRotation;
            _triggered = false;
            
            exitAction?.Invoke();
        }
        

    }

}