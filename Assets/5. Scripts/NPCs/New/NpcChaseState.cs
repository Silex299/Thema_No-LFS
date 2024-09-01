using System.Collections;
using System.Collections.Generic;
using Mechanics.Types;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcChaseState : NpcStateBase
    {
        private bool _isReachable;
        private bool _isStopped;
        private float _speedMultiplier = 1;

        private List<int> _path;

        private bool _isAttacking;
        private int _currentPathIndex;
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");

        public override void Enter(NPCs.New.Npc parentNpc)
        {
            //if target is null change to serveillance
            npc = parentNpc;
            SetInitialAnimatorState();
            _pathCoroutine ??= npc.StartCoroutine(GetPath());
        }

        public override void Update()
        {
            Move();
            ProcessTarget();
        }

        public override void Exit()
        {
            if (_pathCoroutine != null)
            {
                npc.StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }

            if (_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }


        private void SetInitialAnimatorState()
        {
            npc.animator.SetInteger(StateIndex, 1);
            var animatorSpeed = npc.animator.GetFloat(Speed);
            _isStopped = !Mathf.Approximately(animatorSpeed, 1);
        }

        private IEnumerator GetPath()
        {
            while (true)
            {
                _isReachable = npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, out _path);

                if (_path != null)
                {
                    #region Calculate closest point in path


#if UNITY_EDITOR
                    npc.TestPath(_path);
#endif
                    
                    _currentPathIndex = 0;
                    float minDistance = float.MaxValue;
                    for (int i = 0; i < _path.Count; i++)
                    {
                        float distance = GameVector.PlanarDistance(npc.transform.position, npc.pathFinder.GetDesiredPosition(_path[i]));
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            _currentPathIndex = i;
                        }
                    }

                    #endregion
                }

                yield return new WaitForSeconds(npc.pathFindingInterval);
            }

            // ReSharper disable once IteratorNeverReturns
        }


        private void Move()
        {
            npc.animator.SetFloat(Speed, _speedMultiplier);

            Vector3 desiredPos = (_path != null) ? npc.pathFinder.GetDesiredPosition(_path[_currentPathIndex]) : npc.pathFinder.target.position;

            //debug line from npc to desired position //REMOVE
            Debug.DrawLine(npc.transform.position + npc.transform.up * npc.npcEyeHeight, desiredPos, Color.cyan);
            Debug.DrawRay(npc.transform.position + Vector3.up * 2f, npc.transform.forward, Color.blue);
            
            Rotate(npc.transform, desiredPos, npc.rotationSpeed * Time.deltaTime);
        }

        private void ProcessTarget()
        {
            float targetPlannerDistance = GameVector.PlanarDistance(npc.transform.position, npc.pathFinder.target.position);

            //TODO: If not reachable -> move to last list position and scream

            #region If rechable-> process path distance, target under attack distance -> attack

            if (_isReachable)
            {
                ProcessPathProximity();

                if (_path != null)
                {
                    float plannerPathDistance = GameVector.PlanarDistance(npc.transform.position, npc.pathFinder.GetDesiredPosition(_path[_currentPathIndex]));
                    if (plannerPathDistance < npc.stopDistance)
                    {
                        _currentPathIndex = (_currentPathIndex + 1) % _path.Count;
                    }
                }

                Attack(targetPlannerDistance < npc.attackDistance);
            }

            #endregion

            #region If not reachable -> Stop if distance is less than stop distance and vice vers

            else
            {
                ProcessPathProximity();
            }

            #endregion
        }

        private void Attack(bool attack)
        {
            if (attack) npc.onAttack?.Invoke();

            if (_isAttacking == attack) return;
            _isAttacking = attack;
            npc.animator.SetBool(Attack1, attack);
        }

        private void StopMoving()
        {
            //if speed coroutine is not null stop and start new coroutine
            if (_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
            }

            _speedCoroutine = npc.StartCoroutine(ChangeSpeed(0));
        }

        private void StartMoving()
        {
            //if speed coroutine is not null stop and start new coroutine
            if (_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
            }

            _speedCoroutine = npc.StartCoroutine(ChangeSpeed(1));
        }

        private IEnumerator ChangeSpeed(float targetSpeed)
        {
            float currentSpeed = _speedMultiplier;
            _isStopped = targetSpeed == 0;

            float timeElapsed = 0;
            while (timeElapsed <= npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(currentSpeed, targetSpeed, timeElapsed / npc.accelerationTime);
                yield return null;
            }

            _speedCoroutine = null;
        }

        /// <summary>
        /// Moves if path is not blocked
        /// </summary>
        private void ProcessPathProximity()
        {
            if ((npc.proximityDetection.proximityFlag & ProximityDetection.ProximityFlags.Front) == ProximityDetection.ProximityFlags.Front) //HITTING FRONT
            {
                if (!_isStopped) StopMoving();
                npc.animator.SetBool(PathBlocked, true);
            }
            else
            {
                if (_isStopped) StartMoving();
                npc.animator.SetBool(PathBlocked, false);
            }
        }
    }
}