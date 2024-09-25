using Mechanics.Npc;
using UnityEngine;

namespace NPCs.New
{
    public class NpcIdleState : NpcStateBase
    {
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int Speed = Animator.StringToHash("Speed");

        // Update is called once per frame
        public override void Enter(Npc parentNpc)
        {
            npc = parentNpc;
            
            npc.animator.SetInteger(StateIndex, -1);
            npc.animator.SetBool(Attack, false);
            npc.animator.SetBool(PathBlocked, false);
            npc.animator.SetFloat(Speed, 0);
            npc.animator.CrossFade(npc.entryAnim, 0.25f);
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
        }
    }
}
