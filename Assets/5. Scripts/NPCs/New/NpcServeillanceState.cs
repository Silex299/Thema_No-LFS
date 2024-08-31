using System.Collections;
using Mechanics.Types;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcServeillanceState : NpcStateBase
    {

        private float _speedMultiplier = 1;
        private int _currentWaypointIndex;
        
        private Coroutine _changeWaypointCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");


        public override void Enter(Npc parentNpc)
        {
            npc = parentNpc;
            SetInitialAnimatorState();
            GetInitialIndex();
        }
        
        public override void Update()
        {
            Move();
            
            if(npc.serveillancePoints.Count == 0) return;
            if (CheckForWaypointThreshold())
            {
                _changeWaypointCoroutine ??= npc.StartCoroutine(ChangeWaypoint());
            } //Change waypoint
        }
        
        public override void Exit()
        {
            if (_changeWaypointCoroutine != null)
            {
                npc.StopCoroutine(_changeWaypointCoroutine);
                _changeWaypointCoroutine = null;
            }
        }
        
        
        private void Move()
        {
            //TODO: Make it more dynamic
            npc.animator.SetFloat(Speed, _speedMultiplier);
            
            if(npc.serveillancePoints.Count == 0) return;
            
            Rotate(npc.transform, npc.serveillancePoints[_currentWaypointIndex], 
                _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
        }
        /// <summary>
        /// Checks if the npc has reached the waypoint
        /// </summary>
        /// <returns> if waypoint can be changed </returns>
        private bool CheckForWaypointThreshold()
        {
            if (GameVector.PlanarDistance(npc.transform.position, npc.serveillancePoints[_currentWaypointIndex]) < npc.stopDistance)
            {
                return true;
            }
            return false;
        }
        
        private IEnumerator ChangeWaypoint()
        {
            //decelerate 
            float timeElapsed = 0;
            while (timeElapsed < npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(1, 0, timeElapsed / npc.accelerationTime);
                yield return null;
            }
            
            //wait 
            yield return new WaitForSeconds(npc.serveillanceWaitTime);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % npc.serveillancePoints.Count;
            
            //accelerate
            timeElapsed = 0;
            while (timeElapsed < npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(0, 1, timeElapsed / npc.accelerationTime);
                yield return null;
            }
            _changeWaypointCoroutine = null;
        }
        private void SetInitialAnimatorState()
        {
            npc.animator.SetInteger(StateIndex, 0);
            _speedMultiplier = 1;
            if (npc.serveillancePoints.Count != 0) return;
            _speedMultiplier = 0;
            npc.animator.CrossFade(npc.entryAnim, 0.25f);
        }
        private void GetInitialIndex()
        {
            float minDistance = Mathf.Infinity;
            
            for (int i = 0; i < npc.serveillancePoints.Count; i++)
            {
                float distance = GameVector.PlanarDistance(npc.transform.position, npc.serveillancePoints[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _currentWaypointIndex = i;
                }
            }
        }
        
    }
}

//TODO: NEED A LIL FIX, INITIAL SPEED AND ANIMATIION AND ALL