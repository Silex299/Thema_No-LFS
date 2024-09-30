using System.Collections;
using System.Collections.Generic;
using NPCs.New.Other;
using Sirenix.OdinInspector;
using Thema_Type;
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
        
        [HideIf("stateIndexOnTargetLost", -1)]
        public float returnInterval;

        
        
        private bool _isStopped;
        private float _speedMultiplier = 1;
        private float _pathBlockTime;
        private bool _pathBlocked;

        private List<int> _path;

        private bool _isAttacking;
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");

        #endregion

        public override void Enter(V1Npc npc)
        {
            SetInitialAnimatorState(npc);
        }
        public override void UpdateState(V1Npc npc)
        {
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
        
        
      
        private void ProcessDistanceAndProximity(V1Npc npc, Vector3 desiredPos, bool hasPath)
        {
            
            bool stopMovement = false;
            
            if (!hasPath)
            {
                //TODO: check for in sight
                float distance = ThemaVector.PlannerDistance(npc.transform.position, npc.target.position);
                stopMovement = distance < npc.stopDistance;
                
                if (npc.CanAttack)
                {
                    Attack(npc, distance < attackDistance);
                }
                else if (_isAttacking)
                {
                    Attack(npc , false);
                }
                
            }
            else
            {
                if(_isAttacking) Attack(npc, false);
            }
            
            if(!npc.proximityDetection || stateIndexOnTargetLost == -1) return;
            
            if ((npc.proximityDetection.proximityFlag & ProximityDetection.ProximityFlags.Front) == ProximityDetection.ProximityFlags.Front) //HITTING FRONT
            {
                if (!_pathBlocked)
                {
                    _pathBlocked = true;
                    _pathBlockTime = Time.time;
                    stopMovement = true;
                    npc.animator.SetBool(PathBlocked, true);
                }
                else if(_pathBlockTime + returnInterval < Time.time)
                {
                    _pathBlocked = false;
                    npc.animator.SetBool(PathBlocked, false);
                    npc.ChangeState(stateIndexOnTargetLost);
                }
            }
            else
            {
                if (_pathBlocked)
                {
                    _pathBlocked = false;
                    npc.animator.SetBool(PathBlocked, false);
                }
            }
            
            
            if (stopMovement)
            {
                if(!_isStopped) StopMoving(npc);
            }
            else
            {
                if(_isStopped) StartMoving(npc);
            }

            
        }
        private void Attack(V1Npc npc, bool attack)
        {
            if (attack)
            {
                weapon?.Fire();
                npc.aimRigController?.Aim(npc.target);
            }
            
            _isAttacking = attack;
            npc.animator.SetBool(Attack1, _isAttacking);
        }


        private void StartMoving(V1Npc npc)
        {
            //if speed coroutine is not null stop and start new coroutine
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
            }

            _speedCoroutine = StartCoroutine(ChangeSpeed(npc, 1));
        }
        private void StopMoving(V1Npc npc)
        {
            //if speed coroutine is not null stop and start new coroutine
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
            }

            _speedCoroutine = StartCoroutine(ChangeSpeed(npc, 0));
        }
        private IEnumerator ChangeSpeed(V1Npc npc, float targetSpeed)
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
        

      
        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);
            var animatorSpeed = npc.animator.GetFloat(Speed);
            _isStopped = !Mathf.Approximately(animatorSpeed, 1);
        }
        
        
    }
}