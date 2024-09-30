using System;
using System.Collections;
using System.Collections.Generic;
using Thema_Type;
using UnityEngine;

namespace NPCs.New.V1
{
    public class V1NpcCanineTargetLostState : V1NpcBaseState
    {
        public Vector3 actionPosition;
        public bool rotateTowardsTarget;


        private List<int> _path;
        private bool _isReachable;
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private float _speedMultiplier;
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
            _pathCoroutine ??= StartCoroutine(GetPath(npc));
        }
        public override void Exit(V1Npc npc)
        {
            if (_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }
        }
        public override void UpdateState(V1Npc npc)
        {
            if (_path != null)
            {
                var desiredPos = npc.pathFinder.GetDesiredPosition(_path[0]);

                if (_path.Count > 1)
                {
                    if (ThemaVector.PlannerDistance(desiredPos, npc.transform.position) < npc.stopDistance)
                    {
                        desiredPos = npc.pathFinder.GetDesiredPosition(_path[1]);
                    }
                }
                
                npc.Rotate(desiredPos, npc.rotationSpeed * Time.deltaTime * _speedMultiplier);
            }
            else if(_isReachable)
            {
                npc.Rotate(actionPosition, npc.rotationSpeed * Time.deltaTime * _speedMultiplier);
            }
            else
            {
                npc.animator.SetBool(PathBlocked, true);
            }

            float distance = ThemaVector.PlannerDistance(npc.transform.position, actionPosition);
            bool reached = distance<npc.stopDistance;
            
            npc.animator.SetBool(PathBlocked, reached);
            npc.animator.SetFloat(Speed, _speedMultiplier);
            
            if (reached)
            {
                if (_speedMultiplier > 0)
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                    }
                    _speedCoroutine = StartCoroutine(ChangeMovementSpeed(npc, true));
                }
                if(rotateTowardsTarget) npc.Rotate(npc.target.position, npc.rotationSpeed * Time.deltaTime);
            }
            else
            {
                if (Mathf.Approximately(_speedMultiplier, 0))
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                    }
                    _speedCoroutine = StartCoroutine(ChangeMovementSpeed(npc));
                }
            }
        }


        private IEnumerator GetPath(V1Npc npc)
        {
            while (true)
            {
                if (!npc.gameObject.activeInHierarchy) continue;
                _isReachable = npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, actionPosition, out _path);
                yield return new WaitForSeconds(npc.pathFindingInterval);
            }
            // ReSharper disable once IteratorNeverReturns
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