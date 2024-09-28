using System.Collections;
using System.Collections.Generic;
using Thema_Type;
using UnityEngine;

namespace Mechanics.Npc
{
    public class NpcChaseState : NpcStateBase
    {
        private bool _isStopped;
        private float _speedMultiplier = 1;

        private List<int> _path;

        [SerializeField]private bool _isAttacking;
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
                if(!npc.gameObject.activeInHierarchy) continue;
                npc.pathFinder.GetPath(npc.transform.position + npc.transform.up * npc.npcEyeHeight, npc.target.position + npc.target.up * npc.targetOffset, out _path);
                yield return new WaitForSeconds(npc.pathFindingInterval);
            }

            // ReSharper disable once IteratorNeverReturns
        }


        private void Move()
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
            
            ProcessDistanceAndProximity(desiredPos, _path != null);
            Rotate(npc.transform, desiredPos, npc.rotationSpeed * Time.deltaTime);
        }

        private void Attack(bool attack)
        {
            if (attack) npc.onAttack.Invoke();
            
            if (_isAttacking == attack) return;
            _isAttacking = attack;
            npc.animator.SetBool(Attack1, _isAttacking);
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

        private float _pathBlockTime = 0;
        private bool _pathBlocked;
        /// <summary>
        /// Moves if path is not blocked
        /// </summary>
        private void ProcessDistanceAndProximity(Vector3 desiredPos, bool hasPath)
        {

            bool stopMovement = false;
            
            if (!hasPath)
            {
                float distance = ThemaVector.PlannerDistance(npc.transform.position, desiredPos);
                stopMovement = distance < npc.stopDistance;
                
                if (npc.CanAttack)
                {
                    Attack(distance < npc.attackDistance);
                }
                else if (_isAttacking)
                {
                    Attack(false);
                }
                
            }
            else
            {
                if(_isAttacking) Attack(false);
            }
            
            if ((npc.proximityDetection.proximityFlag & ProximityDetection.ProximityFlags.Front) == ProximityDetection.ProximityFlags.Front) //HITTING FRONT
            {
                if (!_pathBlocked)
                {
                    _pathBlocked = true;
                    _pathBlockTime = Time.time;
                    stopMovement = true;
                    npc.animator.SetBool(PathBlocked, true);
                }
                else if(_pathBlockTime + npc.returnInterval < Time.time)
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
                if(!_isStopped) StopMoving();
            }
            else
            {
                if(_isStopped) StartMoving();
            }
            
        }
        
        
        
    }
}