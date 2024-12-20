﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;
using Weapons.NPC_Weapon;

namespace NPCs.New.V1.States
{
    public class V1NpcChaseState : V1NpcBaseState
    {
        #region Variables

        public float attackDistance;
        public int stateIndexOnTargetLost = -1;
        public WeaponBase weapon;

        [HideIf("stateIndexOnTargetLost", -1)] public float returnInterval;


        private bool _isStopped;
        private float _speedMultiplier = 1;
        private float _pathBlockTime;
        private bool _pathBlocked;
        private bool _isReachable;
        public bool UpdatePath { get; set; } = true;

        private List<int> _path;

        private bool _isAttacking;
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private Coroutine _targetLostCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");

        #endregion

        public override void Enter(V1Npc npc)
        {
            SetInitialAnimatorState(npc);
            _pathCoroutine ??= StartCoroutine(GetPath(npc));
        }

        public override void UpdateState(V1Npc npc)
        {
            Vector3 targetPosition = npc.Target.position + npc.Target.up * npc.targetOffset;
            Vector3 npcPosition = npc.transform.position + npc.transform.up * npc.npcEyeHeight;

            Vector3 desiredPos = targetPosition;

            if (_isReachable)
            {
                if (_path is { Count: > 0 })
                {
                    desiredPos = npc.pathFinder.GetDesiredPosition(_path[0]);

                    if (ThemaVector.PlannerDistance(npcPosition, desiredPos) < npc.stopDistance)
                    {
                        _path.RemoveAt(0);
                    }
                }

                switch (_isStopped)
                {
                    case true when !ShouldStop(npc):
                    {
                        if (_speedCoroutine != null)
                        {
                            StopCoroutine(_speedCoroutine);
                            _speedCoroutine = null;
                        }

                        _speedCoroutine ??= StartCoroutine(ChangeSpeed(npc, 1));
                        break;
                    }
                    case false when ShouldStop(npc) && npc.CanAttack:
                    {
                        if (_speedCoroutine != null)
                        {
                            StopCoroutine(_speedCoroutine);
                            _speedCoroutine = null;
                        }

                        _speedCoroutine ??= StartCoroutine(ChangeSpeed(npc, 0));
                        break;
                    }
                }
            }
            else
            {
                if (!_isStopped)
                {
                    if (_speedCoroutine != null)
                    {
                        StopCoroutine(_speedCoroutine);
                        _speedCoroutine = null;
                    }

                    if (stateIndexOnTargetLost != -1)
                    {
                        _targetLostCoroutine ??= StartCoroutine(TargetLostStateChange(npc));
                    }
                    else
                    {
                        _speedCoroutine ??= StartCoroutine(ChangeSpeed(npc, 0));
                    }
                }
            }

            npc.animator.SetFloat(Speed, _speedMultiplier);
            npc.Rotate(desiredPos, npc.rotationSpeed * Time.deltaTime);
            
            
        }

        public override void LateUpdateState(V1Npc npc)
        {
            #region Attack

            
            if (ShouldAttack(npc) && npc.CanAttack)
            {
                npc.animator.SetBool(Attack1, true);
                //TODO: Check On debug
                Debug.DrawLine(npc.transform.position, npc.Target.position, Color.green);
                Attack(npc);
            }
            else
            {
                
                Debug.DrawLine(npc.transform.position, npc.Target.position, Color.red);
                npc.animator.SetBool(Attack1, false);
            }

            #endregion
        }


        private bool ShouldAttack(V1Npc npc)
        {
            var distance = ThemaVector.PlannerDistance(npc.transform.position, npc.Target.position);
            return distance <= attackDistance;
        }

        private void Attack(V1Npc npc)
        {
            weapon?.Fire();
            npc.aimRigController?.Aim(npc.Target);
        }

        private bool ShouldStop(V1Npc npc)
        {
            var distance = ThemaVector.PlannerDistance(npc.Target.position, npc.transform.position);
            return distance < npc.stopDistance;
        }

        public override void Exit(V1Npc npc)
        {
            if (_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }

            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }

        private IEnumerator ChangeSpeed(V1Npc npc, float targetSpeed)
        {
            
            float currentSpeed = _speedMultiplier;
            _isStopped = (targetSpeed == 0);

            float timeElapsed = 0;
            while (timeElapsed <= npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(currentSpeed, targetSpeed, timeElapsed / npc.accelerationTime);
                yield return null;
            }

            _speedMultiplier = targetSpeed;
            _speedCoroutine = null;
        }

        private IEnumerator TargetLostStateChange(V1Npc npc)
        {
            yield return new WaitForSeconds(returnInterval);
            npc.ChangeState(stateIndexOnTargetLost);
            _targetLostCoroutine = null;
        }

        private IEnumerator GetPath(V1Npc npc)
        {
            while (true)
            {
                if (UpdatePath)
                {
                    _isReachable = npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, npc.Target.position + npc.Target.up * npc.targetOffset, out _path);
                    PreviewPath(npc);
                }

                yield return new WaitForSeconds(npc.pathFindingInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);

            _isStopped = false;
            _speedMultiplier = 1;
            npc.animator.SetFloat(Speed, _speedMultiplier);
            npc.animator.CrossFade("Default", 0.1f, 1);
        }

        private void PreviewPath(V1Npc npc)
        {


            if (_path == null)
            {
                if (_isReachable)
                {
                    Debug.DrawLine(npc.Target.position + npc.Target.up * npc.targetOffset, npc.transform.position + npc.transform.up * npc.npcEyeHeight, Color.green, npc.pathFindingInterval);
                }
                return;
            }
            

            Debug.DrawLine(npc.transform.position + npc.transform.up * npc.npcEyeHeight, npc.pathFinder.GetDesiredPosition(_path[0]), Color.white, npc.pathFindingInterval);

            for (int i = 0; i < _path.Count - 1; i++)
            {
                Debug.DrawLine(npc.pathFinder.GetDesiredPosition(_path[i]), npc.pathFinder.GetDesiredPosition(_path[i + 1]), Color.yellow, npc.pathFindingInterval);
            }

            Debug.DrawLine(npc.Target.position + npc.Target.up * npc.targetOffset, npc.pathFinder.GetDesiredPosition(_path[^1]), Color.white, npc.pathFindingInterval);
        }
    }
}