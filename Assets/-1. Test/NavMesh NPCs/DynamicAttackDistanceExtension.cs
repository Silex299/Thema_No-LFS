using Player_Scripts;
using UnityEngine;

namespace NavMesh_NPCs
{
    // ReSharper disable once InconsistentNaming
    public class DynamicAttackDistanceExtension : MonoBehaviour
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