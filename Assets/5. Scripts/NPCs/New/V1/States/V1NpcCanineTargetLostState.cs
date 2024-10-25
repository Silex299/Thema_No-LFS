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
        public int animatorStateIndex;


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
            npc.animator.Play("Default", 1);
            npc.animator.SetInteger(StateIndex, animatorStateIndex);
            _pathCoroutine ??= StartCoroutine(GetPath(npc));
        }

        public override void Exit(V1Npc npc)
        {
            if (_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }
            
            npc.animator.Play("Default", 1);
            npc.animator.SetBool(PathBlocked, false);
        }

        public override void UpdateState(V1Npc npc)
        {
            if (_isReachable)
            {
                var desiredPos = actionPosition;

                if (_path is { Count: > 0 })
                {
                    desiredPos = npc.pathFinder.GetDesiredPosition(_path[0]);

                    if (ThemaVector.PlannerDistance(npc.transform.position, desiredPos) < npc.stopDistance)
                    {
                        _path.RemoveAt(0);
                    }
                }

                npc.Rotate(desiredPos, npc.rotationSpeed * Time.deltaTime * _speedMultiplier);
            }
            else if (_isReachable)
            {
                npc.Rotate(actionPosition, npc.rotationSpeed * Time.deltaTime * _speedMultiplier);
            }


            float distance = ThemaVector.PlannerDistance(npc.transform.position, actionPosition);
            bool reached = distance < npc.stopDistance;

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

                npc.animator.SetBool(PathBlocked, true);
                if (rotateTowardsTarget) npc.Rotate(npc.Target.position, npc.rotationSpeed * Time.deltaTime);
            }
            else
            {
                if (Mathf.Approximately(_speedMultiplier, 0))
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                    }

                    npc.animator.SetBool(PathBlocked, false);
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