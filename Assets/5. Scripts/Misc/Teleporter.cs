using System.Collections;
using UnityEngine;

namespace Misc
{

    public class Teleporter : MonoBehaviour
    {


        [SerializeField] private Transform destination;
        [SerializeField] private float delay = 1;
        [SerializeField] private float secondActionDelay = 2;


        private bool _isTriggered;
        private bool _trigger;

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Teleport(other.transform));
            }
        }


        private void LateUpdate()
        {
            if (_trigger)
            {
                Player_Scripts.PlayerMovementController controller = Player_Scripts.PlayerMovementController.Instance;

                controller.player.DisablePlayerMovement = true;
                controller.player.CController.enabled = false;

                controller.transform.position = destination.position;


                controller.player.CController.enabled = true;
                controller.player.DisablePlayerMovement = false;
                _trigger = false;
            }
        }


        private IEnumerator Teleport(Transform target)
        {

            Managers.UIManager ui = Managers.UIManager.Instance;

            ui.FadeIn();
            _isTriggered = true;

            yield return new WaitForSeconds(delay);

            _trigger = true;
            ui.FadeOut();

            yield return new WaitForSeconds(secondActionDelay);

            _isTriggered = false;
        }


    }

}