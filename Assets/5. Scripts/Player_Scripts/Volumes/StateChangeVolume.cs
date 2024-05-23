using Path_Scripts;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


namespace Player_Scripts.Volumes
{
    [RequireComponent(typeof(Collider))]
    public class StateChangeVolume : MonoBehaviour
    {


        [SerializeField, BoxGroup("ENTRY STATE")] private PlayerMovementState entryState;
        [SerializeField, BoxGroup("ENTRY STATE")] private float reTriggerDelay;
        [SerializeField, BoxGroup("ENTRY STATE")] private int entryStateIndex;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool enableDirection;
        [SerializeField, BoxGroup("ENTRY STATE")] private bool oneWayRotation;

        [SerializeField, BoxGroup("EXIT STATE")] private PlayerMovementState exitState;
        [SerializeField, BoxGroup("EXIT STATE")] private int exitStateIndex;


        private bool _playerIsInTrigger;
        private Coroutine triggerReset;

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;

            if (other.CompareTag("Player_Main"))
            {
                if (!_playerIsInTrigger)
                {
                    _playerIsInTrigger = true;
                    var playerController = PlayerMovementController.Instance;
                    playerController.ChangeState(entryState, entryStateIndex);

                    playerController.player.enabledDirectionInput = enableDirection;
                    playerController.player.oneWayRotation = oneWayRotation;
                }
                
                if (triggerReset != null)
                {
                    StopCoroutine(triggerReset);
                }

                triggerReset = StartCoroutine(ResetTrigger());
            }

            
        }


        private void OnTriggerExit(Collider other)
        {
            if (!_playerIsInTrigger) return;
            
            if (other.CompareTag("Player_Main"))
            {
                triggerReset = StartCoroutine(ResetTrigger());
            }

        }

        private IEnumerator ResetTrigger()
        {
            print("Calling");
            yield return new WaitForSeconds(0.2f);
            print("Called");
            var playerController = PlayerMovementController.Instance;
            playerController.ChangeState(exitState, exitStateIndex);
            playerController.player.enabledDirectionInput = false;
            playerController.player.oneWayRotation = false;

            yield return new WaitForSeconds(reTriggerDelay);
            _playerIsInTrigger = false;
        }


    }

}