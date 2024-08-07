using System.Collections;
using UnityEngine;

namespace Mechanics.Player.Actions
{
    public class PlayerActionBase : MonoBehaviour
    {


        protected PlayerV1 player;
        private Coroutine _coroutineExit;
        protected Coroutine coroutineExecute;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            
            if (!player)
            {
                player = other.GetComponent<PlayerV1>();
                player.CanJump = false;
            }
                
            if (_coroutineExit != null)
            {
                StopCoroutine(_coroutineExit);
            }
            _coroutineExit = StartCoroutine(TriggerExit());
        }

        
        private IEnumerator TriggerExit()
        {
            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() => coroutineExecute == null);
            player.CanJump = true;
            player = null;
            _coroutineExit = null;

        }
        
    }
}
