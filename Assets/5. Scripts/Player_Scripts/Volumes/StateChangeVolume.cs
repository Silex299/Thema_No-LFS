using System;
using Path_Scripts;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Player_Scripts.Volumes
{
    [RequireComponent(typeof(Collider))]
    public class StateChangeVolume : MonoBehaviour
    {


        
        
        [SerializeField, BoxGroup("ENTRY STATE")] private float reTriggerDelay;
        [SerializeField, BoxGroup("ENTRY STATE")] private int entryStateIndex;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool enableEntryDirection;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool entryOneWayRotation;

        [SerializeField, BoxGroup("EXIT STATE")] private int exitStateIndex;
        [SerializeField, BoxGroup("EXIT STATE")] private bool enableExitDirection;
        [SerializeField, BoxGroup("EXIT STATE")] private bool exitOneWayRotation;
        
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
                        PlayerMovementController.Instance.ChangeState(entryStateIndex);
                        PlayerMovementController.Instance.player.enabledDirectionInput = enableEntryDirection;
                        PlayerMovementController.Instance.player.oneWayRotation = entryOneWayRotation;
                        _triggered = true;
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
            if(continuousCheck) return;
            
            if (other.CompareTag("Player_Main"))
            {
                PlayerMovementController.Instance.ChangeState(entryStateIndex);
                PlayerMovementController.Instance.player.enabledDirectionInput = enableEntryDirection;
                PlayerMovementController.Instance.player.oneWayRotation = entryOneWayRotation;
                _triggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("Player_Main"))
            {
                
                PlayerMovementController.Instance.ChangeState(exitStateIndex);
                PlayerMovementController.Instance.player.enabledDirectionInput = enableExitDirection;
                PlayerMovementController.Instance.player.oneWayRotation = exitOneWayRotation;
                _triggered = false;

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


    }

}