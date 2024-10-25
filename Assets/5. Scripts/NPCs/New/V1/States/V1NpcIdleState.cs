using UnityEngine;

namespace NPCs.New.V1
{
    
    public sealed class V1NpcIdleState : V1NpcBaseState
    {
        
        public string entryAnim;
        public float transitionDuration = 0.25f;
        public int animatorLayer = 0;
        
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void Enter(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, -1);
            npc.animator.SetBool(Attack, false);
            npc.animator.SetBool(PathBlocked, false);
            npc.animator.SetFloat(Speed, 0);
            npc.animator.CrossFade(entryAnim, transitionDuration, animatorLayer);
        }
    }
}
