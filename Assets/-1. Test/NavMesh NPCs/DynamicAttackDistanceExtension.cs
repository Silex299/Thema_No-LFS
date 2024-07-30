using System;
using Misc;
using Player_Scripts;
using Sirenix.OdinInspector;
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
        [Space(10)] public bool is2D;
        [ShowIf(nameof(is2D))] public Axis axis;
        [ShowIf(nameof(is2D))] public bool invertedAxis;

        private static readonly int TargetVelocity = Animator.StringToHash("TargetVelocity");

        private void FixedUpdate()
        {
            float dynamicAttackDistance = attackThreshold + attackAnimDistanceTravelled;

            if (is2D)
            {
                dynamicAttackDistance -= invertedAxis
                    ? -1
                    : 1 * GetAxis(PlayerVelocityCalculator.Instance.velocity, axis) * attackAnimTime;
            }
            else
            {
                dynamicAttackDistance -= invertedAxis
                    ? -1
                    : 1 * PlayerVelocityCalculator.Instance.velocity.magnitude * attackAnimTime;
            }

            if (dynamicAttackDistance < 0)
            {
                npc.attackDistance = attackThreshold + Mathf.Abs(dynamicAttackDistance);
            }
            else
            {
                npc.attackDistance = dynamicAttackDistance;
            }
        }


        private float GetAxis(Vector3 vector, Axis accessAxis)
        {
            return accessAxis switch
            {
                Axis.x => vector.x,
                Axis.y => vector.y,
                Axis.z => vector.z,
                _ => 0
            };
        }
    }
}