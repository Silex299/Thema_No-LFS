using System.Collections;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class V1NpcAnimationIntermediateState : V1NpcBaseState
    {
        
        public string animationName;
        public int animationLayer;
        
        public int exitStateIndex;
        public float exitDelay;
        
        public override void Enter(V1Npc npc)
        {
            npc.animator.CrossFade(animationName, 0.2f, animationLayer);
            npc.StartCoroutine(ExitState(npc));
        }
        
        private IEnumerator ExitState(V1Npc npc)
        {
            yield return new WaitForSeconds(exitDelay);
            npc.ChangeState(exitStateIndex);
        }
        
    }
}
