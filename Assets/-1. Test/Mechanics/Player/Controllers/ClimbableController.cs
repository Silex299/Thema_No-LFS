using System.Collections;
using Mechanics.Player.Interactable;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Controllers
{
    [RequireComponent(typeof(ClimbableBase))]
    public class ClimbableController : Controller
    {


        public string entryAnim;
        
        
        public bool canJumpOff;
        public ClimbableBase climbable;


        public UnityEvent exitEvent;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private Coroutine _exitClimbable;
        private static readonly int Jump = Animator.StringToHash("Jump");
        public override void ControllerEnter(PlayerV1 player)
        {
            climbable.EngageClimbable(player);
            player.ResetAnimator();
            player.animator.CrossFade(entryAnim, 0.2f, 0);
        }
        
        public override void ControllerExit(PlayerV1 player)
        {
            player.ResetMovement();
            if (_exitClimbable != null)
            {
                player.StopCoroutine(_exitClimbable);
                _exitClimbable = null;
            }
            climbable.ExitClimbable();
        }
        
        public override void ControllerLateUpdate(PlayerV1 player)
        {
            if(!climbable.Engaged) return; 
            
            var input = Input.GetAxis("Vertical");
            climbable.MovePlayer(input, player.transform);
            player.animator.SetFloat(Speed, input);
            if(!canJumpOff) return;
            
            if (Input.GetButtonDown("Jump"))
            {
                _exitClimbable ??= player.StartCoroutine(ExitCoroutine(player));
            }
            
        }

        private IEnumerator ExitCoroutine(PlayerV1 player)
        {
            climbable.ExitClimbable();
            
            player.animator.SetTrigger(Jump);
            yield return new WaitForSeconds(0.1f);

            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }
            
            _exitClimbable = null;
            exitEvent.Invoke();

        }
        
    }
}
