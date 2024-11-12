using UnityEngine;

namespace NPCs.New.V1.States
{
    public class WaterCreatureChase : V1NpcBaseState
    {
        public float chaseSpeed = 5;
        public float attackDistance = 2;
        
        public int targetLostState = -1;
        private float _lastAttackTime;
        private Coroutine _accelerateCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        


        public override void Enter(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);
        }
        
        public override void UpdateState(V1Npc npc)
        {
            print("Running Chase State");
            
            Vector3 npcPoint = npc.transform.position + npc.transform.up * npc.npcEyeHeight;
            Vector3 targetPoint = npc.Target.position + npc.Target.up * npc.targetOffset;
            Vector3 desiredPos = targetPoint;

            if (npc.pathFinder.GetPath(npcPoint, targetPoint, out var path))
            {
                if (path != null)
                {
                    desiredPos = npc.pathFinder.GetDesiredPosition(path[0]);
                }
            }
            else
            {
                npc.animator.SetBool(PathBlocked, true);
                if (targetLostState != -1) npc.ChangeState(targetLostState);
                return;
            }
            
            npc.UnRestrictedRotate(desiredPos, Time.deltaTime * npc.rotationSpeed);
            npc.transform.position += npc.transform.forward * (chaseSpeed * Time.deltaTime);
            
            Attack(npc);
            
        }

        private void Attack(V1Npc npc)
        {
            var distance = Vector3.Distance(npc.transform.position, npc.Target.position);
            
            if (distance < attackDistance && npc.CanAttack)
            {
                npc.animator.SetBool(Attack1, true);
            }
            else
            {
                npc.animator.SetBool(Attack1, false);
            }
            
        }
        
        public override void Exit(V1Npc npc)
        {
            npc.animator.SetBool(Attack1, false);
        }


    }
}
