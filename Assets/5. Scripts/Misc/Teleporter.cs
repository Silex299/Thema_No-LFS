using System.Collections;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{

    public class Teleporter : MonoBehaviour
    {


        [SerializeField] private Transform destination;
        [SerializeField] private float delay = 1;
        [SerializeField] private float secondActionDelay = 2;


        [SerializeField, Space(10)] private UnityEvent teleportAction; 

        private bool _isTriggered;
        private bool _trigger;

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.CompareTag("Player_Main"))
            {
                StartCoroutine(Teleport(other.transform));
            }
        }


        /**
        private void LateUpdate()
        {
            if (_trigger)
            {
                Player_Scripts.PlayerMovementController controller = Player_Scripts.PlayerMovementController.Instance;

                controller.player.DisabledPlayerMovement = true;
                controller.player.CController.enabled = false;

                controller.transform.position = destination.position;
                teleportAction?.Invoke();

                controller.player.CController.enabled = true;
                controller.player.DisabledPlayerMovement = false;
                _trigger = false;
            }
        }
**/

        private IEnumerator Teleport(Transform target)
        {

            Managers.UIManager ui = Managers.UIManager.Instance;
            var player = PlayerMovementController.Instance.player;

            ui.FadeIn();
            player.CController.enabled = false;
            player.DisabledPlayerMovement = true;
            
            _isTriggered = true;
            
            yield return new WaitForSeconds(delay);
            yield return new WaitForEndOfFrame();
            
            player.transform.position = destination.position;
            teleportAction?.Invoke();
            
            yield return new WaitForSeconds(secondActionDelay);
            
            player.CController.enabled = true;
            player.DisabledPlayerMovement = false;
            ui.FadeOut();
            _isTriggered = false;
        }


    }

}