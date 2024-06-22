using Misc;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class GuardWithWeapon : Guard
    {
        [SerializeField] internal FireArm rifle;
        [SerializeField] internal SightDetection sightSensor;
        
        
        [SerializeField, BoxGroup("States")] 
        private AdvancedSurveillanceState advancedSurveillanceState = new AdvancedSurveillanceState();

        [SerializeField, BoxGroup("States")] 
        private AdvancedChaseState advancedChaseState = new AdvancedChaseState();
 
        public override void ChangeState(GuardStateEnum newState)
        {
            currentState?.StateExit(this);
            currentGuardState = newState;

            switch (currentGuardState)
            {
                case GuardStateEnum.Guard:
                    currentState = advancedSurveillanceState;
                    currentState.StateEnter(this);
                    break;
                case GuardStateEnum.Chase:
                    currentState = advancedChaseState;
                    currentState.StateEnter(this);
                    break;
            }
        }

        private void OnEnable()
        {
            sightSensor.enabled = true;
        }

        private void OnDisable()
        {
            sightSensor.enabled = false;
        }
    }


    [System.Serializable]
    public class AdvancedSurveillanceState : SurveillanceState
    {
        public override void StateUpdate(Guard guard)
        {
            Vector3 nextPoint = guardPoints[currentGuardPoint].position;
            var transform = guard.transform;
            var position = transform.position;
            
            nextPoint.y = position.y;
            float distance = Vector3.Distance(position, nextPoint);


            if (distance < stopDistance)
            {
                nexPointCoroutine ??= guard.StartCoroutine(ChangeGuardPoint());
                if (walk)
                {
                    guard.animator.CrossFade("Basic Idle", 0.1f, 0);
                    walk = false;
                }
            }
            else
            {
                if (!walk)
                {
                    guard.animator.CrossFade("Walk", 0.1f, 0);
                    walk = true;
                }

                guard.Rotate(nextPoint);
            }
        }
    }

    [System.Serializable]
    public class AdvancedChaseState : ChaseState
    {

        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private bool _isAttacking;
        private float _firstFireTime;

        public override void StateUpdate(Guard guard)
        {
            if (stopChasing) return;

            if (playerDead)
            { 
                guard.StartCoroutine(StopChaseAnimationUpdate(guard));
                stopChasing = true;
                return;
            }
 
            Vector3 targetPos = target.position;
            Vector3 guardPos = guard.transform.position;

            targetPos.y = guardPos.y;

            if (advancedPathFinding)
            {
                if (!AdvancedPathFinding(guard))
                {
                    ChaseAction(guard, targetPos, guardPos);
                }
                else
                {
                    guard.animator.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                }
                
            }
            else
            {  
                ChaseAction(guard, targetPos, guardPos);
            }
            
            
            guard.Rotate(targetPos, 40f);
            
        }

        protected override void ChaseAction(Guard guard, Vector3 targetPos, Vector3 guardPos)
        {
            float distance = Vector3.Distance(targetPos, guardPos);
            
            float speed = distance > chaseDistance ? 1 : 0;

            if (distance < attackDistance)
            {
                if (CheckAngle(guard))
                {
                    guard.animator.SetBool(Attack1,true);
                    Attack(guard);
                }
                else
                {
                    guard.animator.SetBool(Attack1,false);
                    _isAttacking = false;
                }
            }
            else if (_isAttacking)
            {
                if (CheckAngle(guard))
                {
                    guard.animator.SetBool(Attack1,true);
                    Attack(guard);
                }
                else
                {
                    guard.animator.SetBool(Attack1,false);
                    _isAttacking = false;
                }
            }

            guard.animator.SetFloat(Speed, speed, 0.2f, Time.deltaTime);
        }


        protected override void Attack(Guard guard)
        {

            if (!_isAttacking)
            {
                _isAttacking = true;
                _firstFireTime = Time.time;
                return;
            }
            
            
            if (Time.time < _firstFireTime + 0.8f)
            {
                return;
            }
            
            var parentGuard = guard as GuardWithWeapon;
            
            // ReSharper disable once PossibleNullReferenceException
            parentGuard.rifle.Fire();

        }

        private bool CheckAngle(Guard guard)
        {
            var transform = guard.transform;
            Vector3 guardForward = transform.forward;
            Vector3 direction = PlayerMovementController.Instance.transform.position - transform.position;
            direction = direction.normalized;

            guardForward.y = 0;
            direction.y = 0;
            
            float angle = Vector3.Angle(guardForward, direction);
            
            return angle < 2;
        }
    }
}