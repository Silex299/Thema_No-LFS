using System.Collections;
using Mechanics.Types;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcServeillanceState : NpcStateBase
    {

        private Npc _npc;
        private float _speedMultiplier = 1;
        private int _currentWaypointIndex;
        
        private Coroutine _changeWaypointCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");


        public override void Enter(Npc npc)
        {
            _npc = npc;
            SetInitialAnimatorState();
            GetInitialIndex();
        }
        
        public override void Update()
        {
            Move();
            if (CheckForWaypointThreshold())
            {
                _changeWaypointCoroutine ??= _npc.StartCoroutine(ChangeWaypoint());
            } //Change waypoint
        }
        
        public override void Exit()
        {
            if (_changeWaypointCoroutine == null) return;
            
            _npc.StopCoroutine(_changeWaypointCoroutine);
            _changeWaypointCoroutine = null;
        }
        
        
        private void Move()
        {
            _npc.animator.SetFloat(Speed, _speedMultiplier);
            Rotate(_npc.transform, _npc.serveillancePoints[_currentWaypointIndex], 
                _speedMultiplier * _npc.rotationSpeed * Time.deltaTime);
        }
        /// <summary>
        /// Checks if the npc has reached the waypoint
        /// </summary>
        /// <returns> if waypoint can be changed </returns>
        private bool CheckForWaypointThreshold()
        {
            if (GameVector.PlanarDistance(_npc.transform.position, _npc.serveillancePoints[_currentWaypointIndex]) < _npc.stopDistance)
            {
                return true;
            }
            return false;
        }
        private IEnumerator ChangeWaypoint()
        {
            //decelerate 
            float timeElapsed = 0;
            while (timeElapsed < _npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(1, 0, timeElapsed / _npc.accelerationTime);
                yield return null;
            }
            
            //wait 
            yield return new WaitForSeconds(_npc.serveillanceWaitTime);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _npc.serveillancePoints.Count;
            
            //accelerate
            timeElapsed = 0;
            while (timeElapsed < _npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(0, 1, timeElapsed / _npc.accelerationTime);
                yield return null;
            }
            _changeWaypointCoroutine = null;
        }

        private void SetInitialAnimatorState()
        {
            _npc.animator.SetInteger(StateIndex, 0);
        }
        private void GetInitialIndex()
        {
            float minDistance = Mathf.Infinity;
            
            for (int i = 0; i < _npc.serveillancePoints.Count; i++)
            {
                float distance = GameVector.PlanarDistance(_npc.transform.position, _npc.serveillancePoints[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _currentWaypointIndex = i;
                }
            }
        }
        
    }
}