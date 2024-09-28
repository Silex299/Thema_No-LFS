using System.Collections;
using System.Collections.Generic;
using Mechanics.Npc;
using NPCs.New.V1;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace NPCs.New
{
    public class V1NpcChaseState : V1NpcBaseState
    {

        #region Variables

        public float attackDistance;
        public bool returnToServeillanceOnTargetLost;
        [ShowIf(nameof(returnToServeillanceOnTargetLost))]
        public float returnInterval;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool CanAttack { get; set; } = true;
        
        
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
            _pathCoroutine ??= StartCoroutine(GetPath(npc));
        }


        public override void UpdateState(V1Npc npc)
        {
            Move(npc);
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
        
        
        private void Move(V1Npc npc)
        {
            npc.animator.SetFloat(Speed, _speedMultiplier);
            
            Vector3 desiredPos = npc.target.position;
            
            if (_path != null)
            {
                desiredPos =  npc.pathFinder.GetDesiredPosition(_path[0]);
                
                if (_path.Count > 1)
                {
                    if (ThemaVector.PlannerDistance(desiredPos, npc.transform.position) < npc.stopDistance)
                    {
                        desiredPos = npc.pathFinder.GetDesiredPosition(_path[1]);
                    }
                }
            }
            
            Debug.DrawLine(npc.transform.position + npc.transform.up * npc.npcEyeHeight, desiredPos, Color.cyan);
            
            ProcessDistanceAndProximity(npc, desiredPos, _path != null);
            npc.Rotate(desiredPos, npc.rotationSpeed * Time.deltaTime);

        }
        
        private void ProcessDistanceAndProximity(V1Npc npc, Vector3 desiredPos, bool hasPath)
        {
         
            
            bool stopMovement = false;
            
            if (!hasPath)
            {
                float distance = ThemaVector.PlannerDistance(npc.transform.position, desiredPos);
                stopMovement = distance < npc.stopDistance;
                
                if (CanAttack)
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
            
            if(!npc.proximityDetection) return;
            
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
                    npc.ChangeState(1);
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
            if (attack) npc.onAttack.Invoke();
            
            if (_isAttacking == attack) return;
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
        

        private IEnumerator GetPath(V1Npc npc)
        {
            while (true)
            {
                if(!npc.gameObject.activeInHierarchy) continue;
                npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, npc.target.position + npc.target.up * npc.targetOffset, out _path);
                yield return new WaitForSeconds(npc.pathFindingInterval);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);
            var animatorSpeed = npc.animator.GetFloat(Speed);
            _isStopped = !Mathf.Approximately(animatorSpeed, 1);
        }
        
        
    }
}