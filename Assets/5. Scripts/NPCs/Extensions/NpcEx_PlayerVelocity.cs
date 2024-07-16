using Player_Scripts;
using UnityEngine;

namespace NPCs.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class NpcEx_PlayerVelocity : MonoBehaviour
    {
        public GuardNpc npc;

        public float attackAnimTime = 1.233f;
        public float attackAnimDistanceTravelled = 3.47f;
        public float attackThreshold = 2;
        
        private static readonly int TargetVelocity = Animator.StringToHash("TargetVelocity");

        private void FixedUpdate()
        {
            npc.chaseState.attackDistance = attackThreshold + attackAnimDistanceTravelled -
                                            PlayerVelocityCalculator.Instance.velocity.magnitude * attackAnimTime;
        }
    }
}
