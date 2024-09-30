using System.Collections;
using NPCs.New.Other;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Weapons.NPC_Weapon;

namespace NPCs.New.V1.States
{
    public class V1NpcChaseState : V1NpcBaseState
    {
        #region Variables

        public float attackDistance;
        public int stateIndexOnTargetLost = -1;
        public WeaponBase weapon;

        [FormerlySerializedAs("returnInterval")] [HideIf("stateIndexOnTargetLost", -1)]
        public float returnOnTargetLostInterval;


        private float _speedMultiplier = 1;
        private float _pathBlockTime;
        private bool _pathBlocked;

        private NavMeshPathStatus _pathStatus;

        private bool _isAttacking;
        private Coroutine _speedCoroutine;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");

        #endregion

        public override void Enter(V1Npc npc)
        {
            SetInitialAnimatorState(npc);
            npc.navigationAgent.updateRotation = false;
        }

        public override void UpdateState(V1Npc npc)
        {
            var navAgent = npc.navigationAgent;
            navAgent.SetDestination(npc.target.position);


            var plannerDistance = ThemaVector.PlannerDistance(npc.target.position, npc.transform.position);
            bool attack = plannerDistance < attackDistance;
            bool stop = plannerDistance < npc.stopDistance;

            if (stop && _speedMultiplier > 0)
            {
                if (_speedCoroutine != null) StopCoroutine(_speedCoroutine);
                _speedCoroutine ??= StartCoroutine(StopMovement(npc, true));
            }
            else if (!stop && Mathf.Approximately(_speedMultiplier, 0))
            {
                if (_speedCoroutine != null) StopCoroutine(_speedCoroutine);
                _speedCoroutine ??= StartCoroutine(StopMovement(npc, false));
            }

            Attack(npc, attack);

            npc.Rotate(npc.transform.position + navAgent.desiredVelocity, _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
            
            ProcessProximity(npc);

            npc.animator.SetFloat(Speed, _speedMultiplier);
        }

        public override void Exit(V1Npc npc)
        {
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }


        private void ProcessProximity(V1Npc npc)
        {
            var proximity = npc.proximityDetection;

            if (!proximity) return;

            if ((proximity.proximityFlag & ProximityDetection.ProximityFlags.Front) == ProximityDetection.ProximityFlags.Front) //FRONT HTTING
            {
                if (!_pathBlocked)
                {
                    _pathBlocked = true;
                    _pathBlockTime = Time.time;
                    npc.animator.SetBool(PathBlocked, true);
                }
                else if (_pathBlockTime + returnOnTargetLostInterval < Time.time)
                {
                    if (stateIndexOnTargetLost > 0)
                    {
                        npc.ChangeState(stateIndexOnTargetLost);
                    }
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
        }

        private void Attack(V1Npc npc, bool attack)
        {
            if (attack)
            {
                weapon?.Fire();
                npc.aimRigController?.Aim(npc.target);
            }

            if (attack == _isAttacking) return;
            _isAttacking = attack;
            npc.animator.SetBool(Attack1, _isAttacking);
        }

        private IEnumerator StopMovement(V1Npc npc, bool stop)
        {
            float targetSpeed = stop ? 0 : 1;

            float currentSpeed = _speedMultiplier;
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

        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 1);
            _speedCoroutine = StartCoroutine(StopMovement(npc, false));
            _speedMultiplier = 1;
        }
    }
}