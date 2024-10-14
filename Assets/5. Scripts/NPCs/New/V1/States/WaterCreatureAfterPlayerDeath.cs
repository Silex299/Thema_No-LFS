using System.Collections;
using Player_Scripts;
using UnityEngine;
namespace NPCs.New.V1.States
{
    public class WaterCreatureAfterPlayerDeath : V1NpcBaseState
    {


        public float speed = 1;
        
        private Transform _targetBone;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Attack = Animator.StringToHash("Attack");


        public override void Enter(V1Npc npc)
        {
            _targetBone = PlayerMovementController.Instance.player.AnimationController.GetBoneTransform(HumanBodyBones.Hips);
            StartCoroutine(ResetAttack(npc));
        }


        public override void UpdateState(V1Npc npc)
        {
            var distance = Vector3.Distance(npc.transform.position, _targetBone.position);
            
            if (distance > npc.stopDistance)
            {
                npc.transform.position += npc.transform.forward * (speed * Time.deltaTime);
                npc.UnRestrictedRotate(_targetBone.position, Time.deltaTime * npc.rotationSpeed);
                npc.animator.SetInteger(StateIndex, 1);
            }
            else
            {
                npc.animator.SetInteger(StateIndex, -1);
            }
            
        }


        private IEnumerator ResetAttack(V1Npc npc)
        {
            yield return new WaitForSeconds(0.2f);
            npc.animator.SetBool(Attack, false);
        }
        
    }
}
