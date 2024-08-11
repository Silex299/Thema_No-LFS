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
            Quaternion targetRot = climbable.GetInitialRotation(player.transform);
            Vector3 initPlayerPos = player.transform.position;
            Quaternion initPlayerRot = player.transform.rotation;

            float timeElapsed = 0;

            while (timeElapsed < transitionTime)
            {
                player.transform.position = Vector3.Lerp(initPlayerPos, targetPos, timeElapsed / transitionTime);
                player.transform.rotation = Quaternion.Slerp(initPlayerRot, targetRot, timeElapsed / transitionTime);
                timeElapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            engaged = true;
            _engageCoroutine = null;
        }

        #endregion

        #region Action

        public override void ControllerUpdate(PlayerV1 player)
        {
            if (!engaged) return;
            MovePlayer(player, Input.GetAxis("Vertical"));
            climbable.UpdateClimbable(player);
        }

        private void MovePlayer(PlayerV1 player, float input)
        {
            #region Update Animator

            player.animator.SetFloat(Speed, input);

            #endregion

            #region Update player position and rotation

            Vector3 movementVector = climbable.GetMovementVector(player.transform, movementSpeed * Time.deltaTime);
            player.characterController.Move(movementVector);
            player.transform.rotation = climbable.GetMovementRotation(player.transform);

            #endregion

            #region player jump

            //Jump off the climbable
            if (Input.GetButtonDown("Jump") && canJumpOff)
            {
                _exitClimbable ??= StartCoroutine(JumpOff(player));
            }

            #endregion
        }

        #endregion

        #region Exit

        private IEnumerator JumpOff(PlayerV1 player)
        {
            
            #region Player Contstrains

            player.GroundCheck();
            player.AddForce(exitForce);
            
            //Reset Rotation
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);

            #endregion
            
            #region Trigger Jump off

            engaged = false;
            player.animator.SetTrigger(Jump);

            #endregion
            
            #region apply gravity untill grounded

            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }

            #endregion

            #region exit climbable action

            exitEvent.Invoke();
            _exitClimbable = null;

            #endregion
        }

        public override void ControllerExit(PlayerV1 player)
        {
            engaged = false;
            player.ResetMovement();
        }

        #endregion
    }
}