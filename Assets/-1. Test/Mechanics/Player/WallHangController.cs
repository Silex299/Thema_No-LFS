using System.Collections;
using Mechanics.Player.Custom;
using UnityEngine;

namespace Mechanics.Player
{
    public class WallHangController : Controller
    {


        public string enterAnimation;
        public float transitionTime;
        

        public string exitInput = "Jump";
        public float actionWidth = 0.2f;
        public AdvancedCurvedAnimation exitAnim;


        public Controller exitController;
        
        
        private bool _engaged;
        private Coroutine _engageCoroutine;
        private Coroutine _exitCoroutine;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + transform.right * actionWidth, transform.position - transform.right * actionWidth);
        }

        public override void ControllerEnter(PlayerV1 player)
        {
            if (_exitCoroutine == null && _engageCoroutine == null)
            {
                _engageCoroutine ??= StartCoroutine(EngageAnim(player));
            }
            
        }


        public override void ControllerUpdate(PlayerV1 player)
        {
            if(!_engaged) return;

            if (Input.GetButtonDown(exitInput))
            {
                _exitCoroutine ??= StartCoroutine(ExitController(player));
            }
        }
        
        private IEnumerator EngageAnim(PlayerV1 player)
        {
            player.animator.CrossFade(enterAnimation, 0.2f, 1);
            
            var playerTransform = player.transform;
            var initialPlayerPos = playerTransform.position;
            float timeElapsed = 0;
            
            
            while (timeElapsed <= transitionTime)
            {

                timeElapsed = Time.deltaTime;

                playerTransform.position = Vector3.Lerp(initialPlayerPos, player.Interactable.transform.position, timeElapsed / transitionTime);
                yield return new WaitForEndOfFrame();
            }

            _engaged = true;
            _engageCoroutine = null;
        }

        private IEnumerator ExitController(PlayerV1 player)
        {
            _engaged = false;
            
            yield return exitAnim.PlayAnim(transform, player, actionWidth);

            player.MovementController = exitController;
            _exitCoroutine = null;
        }
        
    }
}
