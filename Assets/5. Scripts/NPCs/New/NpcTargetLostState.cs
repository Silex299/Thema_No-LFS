using Mechanics.Npc;
using UnityEngine;

namespace NPCs.New
{
    public class NpcTargetLostState : NpcStateBase
    {
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        // Update is called once per frame
        public override void Enter(Npc parentNpc)
        {
            npc = parentNpc;
        }

        public override void Update()
        {
        }


        public override void Exit()
        {
        }

        private void InitialAnimationSetup()
        {
            npc.animator.SetInteger(StateIndex, 3);
        }
    }
}
