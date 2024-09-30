using System.Collections;
using System.Collections.Generic;
using Thema_Type;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class V1NpcCanineTargetLostState : V1NpcBaseState
    {
        public Vector3 actionPosition;
        public bool rotateTowardsTarget;


        private float _speedMultiplier;
        private Coroutine _speedCoroutine;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");


        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Application.isPlaying ? actionPosition : transform.TransformPoint(actionPosition), 0.2f);
        }


        private void Start()
        {
            actionPosition = transform.TransformPoint(actionPosition);
        }

        public override void Enter(V1Npc npc)
        {
            _speedMultiplier = 1;
            npc.animator.SetBool(Attack, false);
            npc.animator.SetBool(PathBlocked, false);
            npc.animator.SetInteger(StateIndex, 3);
        }
        
        public override void UpdateState(V1Npc npc)
        {
            
            var navAgent = npc.navigationAgent;
            
            navAgent.SetDestination(actionPosition);

            var plannerDistance = ThemaVector.PlannerDistance(actionPosition, npc.transform.position);
            
            bool reachedDestination = plannerDistance < npc.stopDistance;
            
            if (reachedDestination)
            {
                if (_speedMultiplier > 0)
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                    }
                    _speedCoroutine = StartCoroutine(ChangeMovementSpeed(npc, true));
                }
                
                npc.Rotate(npc.target.position, npc.rotationSpeed * Time.deltaTime);
                
            }
            else
            {
                if (_speedMultiplier < 0.8f)
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                    }
                    _speedCoroutine = StartCoroutine(ChangeMovementSpeed(npc, false));
                }
                
                npc.Rotate(navAgent.desiredVelocity, _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
            }
            
            
            npc.animator.SetFloat(Speed, _speedMultiplier);

        }
        
        private IEnumerator ChangeMovementSpeed(V1Npc npc, bool stop = false)
        {
            float timeElapsed = 0;
            float startSpeed = _speedMultiplier;
            float endSpeed = stop ? 0 : 1;

            while (timeElapsed < npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(startSpeed, endSpeed, timeElapsed / npc.accelerationTime);
                yield return null;
            }
            _speedMultiplier = endSpeed;
            _speedCoroutine = null;

        }
    }
}