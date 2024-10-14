using System;
using Player_Scripts;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class WaterCreatureAfterPlayerDeath : V1NpcBaseState
    {


        public float speed = 1;
        public float surroundThreshold = 3;
        public float yOffset;
        
        private Transform _targetBone;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");


        public override void Enter(V1Npc npc)
        {
            _targetBone = PlayerMovementController.Instance.player.AnimationController.GetBoneTransform(HumanBodyBones.Hips);
            npc.animator.SetInteger(StateIndex, -1);
        }


        public override void UpdateState(V1Npc npc)
        {
            
            Vector3 desiredPos = _targetBone.position + Vector3.up * yOffset;
            npc.UnRestrictedRotate(desiredPos, Time.deltaTime * npc.rotationSpeed, Vector3.up);
            npc.transform.position += npc.transform.forward * (speed * Time.deltaTime);
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetBone.position + Vector3.up * yOffset, 0.2f);
        }
    }
}
