using System.Linq;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class V1NpcTargetMatchingSubState : V1NpcBaseState
    {
        [InfoBox("Matches the socket position to targetPlayer bone if certain animations are playing (based on its normalized time)")]
        public Transform socket;
        public HumanBodyBones targetPlayerBone;
        [Tooltip("Maximum distance at which position is matched")]public float distanceThreshold = 4;
        public Vector3 matchingOffset;
        public float matchSpeed;
        [Space(10)] public int animationLayer = 1;
        public string[] animationName;
        [MinMaxSlider(0, 1, true)] public Vector2 normalizedTime;

        private Player Player => PlayerMovementController.Instance.player;

        private float _lastNormalisedValue;
        
        public override void LateUpdateState(V1Npc npc)
        {
            Vector3 npcPos = npc.transform.position;
            Vector3 targetPos = Player.AnimationController.GetBoneTransform(targetPlayerBone).position;

            if(ThemaVector.PlannerDistance(npcPos, targetPos) > distanceThreshold) return;
            
            AnimatorStateInfo stateInfo = npc.animator.GetCurrentAnimatorStateInfo(animationLayer);

            if (animationName.Any(anim => stateInfo.IsName(anim)))
            {
                Vector3 offset = npcPos - socket.position;

                Vector3 desiredPos = targetPos + offset + npc.transform.TransformVector(matchingOffset);
                desiredPos.y = npcPos.y;
                
                float normalisedFraction = stateInfo.normalizedTime % 1;
                
                if (normalisedFraction > normalizedTime.x && normalisedFraction < normalizedTime.y && normalisedFraction> _lastNormalisedValue)
                {
                    float effectFraction = (normalisedFraction - normalizedTime.x) / (normalizedTime.y - normalizedTime.x);
                    npc.transform.position = Vector3.Lerp(npc.transform.position, desiredPos, Time.deltaTime * effectFraction * matchSpeed);
                }
                
                _lastNormalisedValue = normalisedFraction;
            }
        }
    }
}