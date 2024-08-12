using System.Collections;
using Mechanics.Types;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcChaseState : NpcStateBase
    {
        
        private Vector3 _desiredPosition;
        private bool _isReachable;
        private bool _isStopped;
        private float _speedMultiplier = 1;
        
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack1 = Animator.StringToHash("Attack");

        public override void Enter(Npc parentNpc)
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
            
            if(_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }
        
        
        private void SetInitialAnimatorState()
        {
            npc.animator.SetInteger(StateIndex, 1);
        }
        private IEnumerator GetPath()
        {
            while (true)
            {
                _isReachable = npc.pathFinder.GetDesiredPosition(out _desiredPosition);
                yield return new WaitForSeconds(npc.pathFindingInterval);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
        
        private void Move()
        {
            npc.animator.SetFloat(Speed, _speedMultiplier);
            Rotate(npc.transform, _desiredPosition,
                _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
        }
        private void ProcessTarget()
        {
            float plannerDistance = GameVector.PlanarDistance(npc.transform.position, _desiredPosition);

            #region If rechable and under attack distance -> attack
            if (_isReachable)
            {
                if (_isStopped) StartMoving();
                Attack(plannerDistance < npc.attackDistance);
            }
            #endregion
            #region If not reachable -> Stop if distance is less than stop distance and vice vers
            else
            {
                if (plannerDistance < npc.stopDistance)
                {
                    if(!_isStopped) StopMoving();
                }
                else
                {
                    if (_isStopped) StartMoving();
                }
            }
            #endregion
            
        }
        private void Attack(bool attack)
        {
            npc.animator.SetBool(Attack1, attack);
            if (attack) npc.onAttack.Invoke();
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
    }
}
