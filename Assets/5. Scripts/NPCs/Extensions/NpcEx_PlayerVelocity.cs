using NavMesh_NPCs;
using Player_Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class NpcEx_PlayerVelocity : MonoBehaviour
    {
        public NavMeshNpcController npc;

        public float attackAnimTime = 1.233f;
        public float attackAnimDistanceTravelled = 3.47f;
        public float attackThreshold = 2;

        private static readonly int TargetVelocity = Animator.StringToHash("TargetVelocity");

        private void FixedUpdate()
        {
            float dynamicAttackDistance = attackThreshold + attackAnimDistanceTravelled -
                                          PlayerVelocityCalculator.Instance.velocity.magnitude * attackAnimTime;

            if (dynamicAttackDistance < 0)
            {
                npc.attackDistance = attackThreshold + Mathf.Abs(dynamicAttackDistance);
            }
            else
            {
                npc.attackDistance = dynamicAttackDistance;
            }
        }
    }
}