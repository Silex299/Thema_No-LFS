using NavMesh_NPCs;
using Player_Scripts;
using Triggers;
using UnityEngine;

namespace Custom_Object_Scripts
{
    public class ExperimentGuardEncounter : BetterTriggerBase
    {

        public NavMeshNpcController guard;
        
        private static readonly int State = Animator.StringToHash("StateIndex");

        protected override bool OnTriggerExitBool(Collider other)
        {
            if (base.OnTriggerExitBool(other))
            {
                triggered = false;
                PlayerMovementController.Instance.player.AnimationController.SetInteger(State, 0);
                PlayerMovementController.Instance.player.CanJump = true;
            }

            return true;
        }

        private void Update()
        {
            if (!playerInTrigger) return;

            if (!triggered)
            {
                triggered = true;
                PlayerMovementController.Instance.player.AnimationController.SetInteger(State, -6);
                PlayerMovementController.Instance.player.CanJump = false;
            }

            if (!Input.GetButton("Sprint")) return;
            if (!triggered) return;
            
            triggered = false;
            guard.Target = PlayerMovementController.Instance.transform;
            PlayerMovementController.Instance.player.AnimationController.SetInteger(State, 0);
            PlayerMovementController.Instance.player.CanJump = true;
        }
    }
}