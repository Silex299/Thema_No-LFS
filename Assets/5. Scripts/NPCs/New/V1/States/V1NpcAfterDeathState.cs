using System.Collections;
using Thema;
using UnityEngine;

namespace NPCs.New.V1
{
    public class V1NpcAfterDeathState : V1NpcBaseState
    {
        #region Variables

        private float _speedMultiplier;
        private bool _isStopped;

        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region Overriden  Methods

        public override void Enter(V1Npc npc)
        {
            SetInitialAnimatorState(npc);
        }
        
        public override void UpdateState(V1Npc npc)
        {
            Move(npc);
            ProcessTarget(npc);
        }
        
        public override void Exit(V1Npc npc)
        {
            if (_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }
            if(_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }

        #endregion
        
        #region Custom Methods
        
        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, -1);
            _speedMultiplier = 1;
            _isStopped = false;
        }
        
        private void ProcessTarget(V1Npc npc)
        {
            float plannerDistance = ThemaVector.PlannerDistance(npc.transform.position, npc.Target.position);
            
            if (plannerDistance < npc.stopDistance)
            {
                if(!_isStopped) StopMoving(npc);
            }
            else
            {
                if(_isStopped) StartMoving(npc);
            }
        }
        private void Move(V1Npc npc)
        {
            npc.animator.SetFloat(Speed, _speedMultiplier);
            npc.Rotate(npc.Target.position, _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
        }
        
        private void StopMoving(V1Npc npc)
        {
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
            _speedCoroutine = npc.StartCoroutine(ChangeSpeed(npc,0));
        }
        private void StartMoving(V1Npc npc)
        {
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
            _speedCoroutine = npc.StartCoroutine(ChangeSpeed(npc, 1));
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

        #endregion
        
    }
}