using System.Collections;
using Mechanics.Types;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcAfterDeathState : NpcStateBase
    {

        private bool _isReachable;
        private float _speedMultiplier;
        private bool _isStopped;
        
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override void Enter(NPCs.New.Npc parentNpc)
        {
            //TODO: if serveillance after death is true change to serveillance
            
            npc = parentNpc;

            if (npc.serveillanceAfterDeath)
            {
                npc.ChangeState(1);
                return;
            }
            
            SetInitialAnimatorState();
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
            npc.animator.SetInteger(StateIndex, -1);
            _speedMultiplier = 1;
            _isStopped = false;
        }
        
        private void Move()
        {
            npc.animator.SetFloat(Speed, _speedMultiplier);
            Rotate(npc.transform, npc.pathFinder.target.position,
                _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
        }
        private void ProcessTarget()
        {
            float plannerDistance = GameVector.PlanarDistance(npc.transform.position, npc.pathFinder.target.position);
            
            #region if target is reachable -> move to target or vice versa
            if (_isReachable)
            {
                if (plannerDistance < npc.stopDistance)
                {
                    if(!_isStopped) StopMoving();
                }
                else
                {
                    if(_isStopped) StartMoving();
                }
            }
            #endregion
            #region if not reachable -> stop moving
            else
            { 
                if(!_isStopped) StopMoving();
            }
            #endregion
        }
        private void StopMoving()
        {
            if (_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
            _speedCoroutine = npc.StartCoroutine(ChangeSpeed(0));
        }
        private void StartMoving()
        {
            if (_speedCoroutine != null)
            {
                npc.StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
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
