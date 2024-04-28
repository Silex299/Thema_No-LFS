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



        private bool _triggered;
        private Coroutine triggerReset;

        private void OnTriggerStay(Collider other)
        {
            if(!enabled) return;
            
            if (!_triggered)
            {
                if (other.CompareTag("Player_Main"))
                {
                    _triggered = true;
                    var playerController = PlayerMovementController.Instance;
                    playerController.ChangeState(entryState, entryStateIndex);

                    playerController.player.enabledDirectionInput = enableDirection;
                    playerController.player.oneWayRotation = oneWayRotation;
                }
            }

            if (_triggered)
            {
                if(triggerReset!= null)
                {
                    StopCoroutine(triggerReset);
                }

                triggerReset = StartCoroutine(ResetTrigger());

            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!_triggered) return;
            if (other.CompareTag("Player_Main"))
            {
                triggerReset = StartCoroutine(ResetTrigger());
            }
        }

        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            var playerController = PlayerMovementController.Instance;
            playerController.ChangeState(exitState, exitStateIndex);
            playerController.player.enabledDirectionInput = false;
            playerController.player.oneWayRotation = false;

            yield return new WaitForSeconds(reTriggerDelay);
            _triggered = false;
        }


    }

}