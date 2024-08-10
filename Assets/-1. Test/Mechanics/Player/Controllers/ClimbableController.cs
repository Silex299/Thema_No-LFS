using System.Collections;
using Mechanics.Player.Interactable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Controllers
{
    [RequireComponent(typeof(ClimbableBase))]
    public class ClimbableController : Controller
    {

        #region Variables


        #region Exposed Variables

        [BoxGroup("Controller Properties")] public string entryAnim;
        [BoxGroup("Controller Properties")] public Vector3 exitForce;
        [BoxGroup("Controller Properties")] public float transitionTime;
        [BoxGroup("Controller Properties")] public float movementSpeed;
        [BoxGroup("Controller Properties")] public bool canJumpOff;
        
        [BoxGroup("Reference")] public ClimbableBase climbable;
        
        [BoxGroup("Events")] public UnityEvent exitEvent;
        
        [FoldoutGroup("Misc")] public bool engaged;
        

        #endregion


        private Coroutine _exitClimbable;
        private Coroutine _engageCoroutine;
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        #endregion
        
        
        #region Engage
        
        public override void ControllerEnter(PlayerV1 player)
        {
            base.ControllerEnter(player);
            _engageCoroutine ??= StartCoroutine(EngagePlayer(player));
        }
        private IEnumerator EngagePlayer(PlayerV1 player)
        {
            Vector3 targetPos = climbable.GetInitialConnectPoint(player.transform);
            Vector3 initPlayerPos = player.transform.position;
            
            float timeElapsed = 0;
            
            while (timeElapsed < transitionTime)
            {
                player.transform.position = Vector3.Lerp(initPlayerPos, targetPos, timeElapsed);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            engaged = true;
            _engageCoroutine = null;

        }
        
        #endregion
        
        #region Action
        
        public override void ControllerUpdate(PlayerV1 player)
        {
            if(!engaged) return; 
            
            var input = Input.GetAxis("Vertical");
            
            MovePlayer(player, input);
            //ROTATE PLAYER
        }

        private void MovePlayer(PlayerV1 player, float input)
        {
            //Set movement speed 
            player.animator.SetFloat(Speed, input);

            //Get movement vector and move the player
            Vector3 movementVector = climbable.GetMovementVector(player.transform, movementSpeed * Time.deltaTime);
            player.characterController.Move(movementVector);
            
            //Jump off the climbable
            if (Input.GetButtonDown("Jump") && canJumpOff)
            {
                _exitClimbable ??= StartCoroutine(JumpOff(player));
            }
            
        }
      
        #endregion


        #region Exit
        
        private IEnumerator JumpOff(PlayerV1 player)
        {
            engaged = false;
            
            //Jump to trigger and speed
            player.animator.SetTrigger(Jump);
            player.AddForce(exitForce);
            
            //apply gravity until grounded
            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }
            
            //invoke exit event on player reaching the ground
            exitEvent.Invoke();
            _exitClimbable = null;

        }
        
        public override void ControllerExit(PlayerV1 player)
        {
            engaged = false;
        }

        #endregion
    }
}
