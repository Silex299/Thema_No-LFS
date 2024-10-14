using Player_Scripts;
using UnityEngine;
using VLB;

namespace NPCs.New.V1.States
{
    public class WaterCreatureChase : V1NpcBaseState
    {

        public Transform target;
        public float chaseSpeed = 5;
        public float attackDistance = 2;
        public float attackInterval = 0.2f;
        public float damageDistance = 1;
        
        public int targetLostState = -1;
        private float _lastAttackTime;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        


        public override void Enter(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);
        }
        
        public override void UpdateState(V1Npc npc)
        {
            
            
            Vector3 npcPoint = npc.transform.position + npc.transform.up * npc.npcEyeHeight;
            Vector3 targetPoint = target.position + target.up * npc.targetOffset;
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
            var distance = Vector3.Distance(npc.transform.position, target.position);
            
            if (distance < attackDistance && npc.CanAttack)
            {
                if(distance<damageDistance  && Time.time - _lastAttackTime > attackInterval)
                {
                    _lastAttackTime = Time.time;
                    PlayerMovementController.Instance.player.Health.TakeDamage(30);
                }
                
                npc.animator.SetBool(Attack1, true);
            }
            else
            {
                npc.animator.SetBool(Attack1, false);
            }
            
        }

    }
}
