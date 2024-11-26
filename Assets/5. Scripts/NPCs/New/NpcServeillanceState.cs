using System.Collections;
using System.Collections.Generic;
using Mechanics.Types;
using Thema;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mechanics.Npc
{
    public class NpcServeillanceState : NpcStateBase
    {
        private float _speedMultiplier = 1;
        private int _currentWaypointIndex = 0;

        private Coroutine _changeWaypointCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private bool _isReachable;
        private List<int> _path;
        private Coroutine _pathCoroutine;
        private Coroutine _speedCoroutine;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");


        public override void Enter(NPCs.New.Npc parentNpc)
        {
            npc = parentNpc;
            SetInitialAnimatorState();
            _currentWaypointIndex = 0;
            _pathCoroutine = npc.StartCoroutine(GetPath());
        }

        public override void Update()
        {
            Move();

            if (npc.serveillancePoints.Count == 0) return;
            if (CheckForWaypointThreshold())
            {
                _changeWaypointCoroutine ??= npc.StartCoroutine(ChangeWaypoint());
            } //Change waypoint
        }

        public override void Exit()
        {
            if (_pathCoroutine != null)
            {
                npc.StopCoroutine(_pathCoroutine);
                _pathCoroutine = null;
            }

            if (_changeWaypointCoroutine != null)
            {
                npc.StopCoroutine(_changeWaypointCoroutine);
                _changeWaypointCoroutine = null;
            }
        }


        private IEnumerator GetPath()
        {
            while (true)
            {
                
                if(!npc.gameObject.activeInHierarchy) continue;
                
                if (npc.serveillancePoints.Count == 0)
                {
                    _pathCoroutine = null;
                    yield break;
                }

                _isReachable = npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, npc.serveillancePoints[_currentWaypointIndex], out _path);
                yield return new WaitForSeconds(npc.pathFindingInterval);
            }
        }


        private void Move()
        {
            //TODO: Make it more dynamic
            npc.animator.SetFloat(Speed, _speedMultiplier);

            if (npc.serveillancePoints.Count == 0) return;

            Vector3 desiredPos = npc.serveillancePoints[_currentWaypointIndex];

            if (_path != null)
            {
                desiredPos = npc.pathFinder.GetDesiredPosition(_path[0]);

                if (_path.Count > 1)
                {
                    if (ThemaVector.PlannerDistance(desiredPos, npc.transform.position) < npc.stopDistance)
                    {
                        desiredPos = npc.pathFinder.GetDesiredPosition(_path[1]);
                    }
                }
            }


            Debug.DrawLine(npc.transform.position, desiredPos, Color.cyan);
            Rotate(npc.transform, desiredPos, _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
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
            
            if(_speedCoroutine!=null)
            {
                npc.StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
            
            if (npc.serveillanceWaitTime != 0)
            {
                
                yield return Accelerate(0);
                //wait 
                yield return new WaitForSeconds(npc.serveillanceWaitTime);
            }
            
            _currentWaypointIndex = (_currentWaypointIndex + 1) % npc.serveillancePoints.Count;
            
            if (Mathf.Approximately(_speedMultiplier, 0))
            {
                yield return Accelerate(1);
            }
            
            _changeWaypointCoroutine = null;
        }

        private void SetInitialAnimatorState()
        {
            npc.animator.SetInteger(StateIndex, 0);
            npc.animator.SetBool(Attack, false);
            npc.animator.SetBool(PathBlocked, false);
            _speedMultiplier = 0;
            if (npc.serveillancePoints.Count != 0)
            {
                _speedCoroutine = npc.StartCoroutine(Accelerate(1));
            }
            //npc.animator.CrossFade(npc.entryAnim, 0.25f);
        }

        private IEnumerator Accelerate(float targetSpeed)
        {
            float currentSpeed = _speedMultiplier;

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

//TODO: NEED A LIL FIX, INITIAL SPEED AND ANIMATIION AND ALL