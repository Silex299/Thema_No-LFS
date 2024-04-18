using NPCs.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class GuardWithWeapon : Guard
    {
        [SerializeField] internal FireArm rifle;
        
        
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
        
        
    }


    [System.Serializable]
    public class AdvancedSurveillanceState : SurveillanceState
    {
        public override void StateUpdate(Guard guard)
        {
            Vector3 nextPoint = guardPoints[currentGuardPoint].position;

            float distance = Vector3.Distance(guard.transform.position, nextPoint);


            if (distance < stopDistance)
            {
                nexPointCoroutine ??= guard.StartCoroutine(ChangeGuardPoint());
                if (walk)
                {
                    guard.animator.CrossFade("Basic Idle", 0.3f, 0);
                    walk = false;
                }
            }
            else
            {
                if (!walk)
                {
                    guard.animator.CrossFade("Walk", 0.3f, 0);
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

            float distance = Vector3.Distance(targetPos, guardPos);
            
            float speed = 0;

            if (distance > chaseDistance)
            {
                speed = 1;
            }
            else if(distance < 1.4f)
            {
                speed = 0;
            }

            guard.animator.SetBool(Attack1,distance < attackDistance);

            if (distance < attackDistance)
            {
                Attack(guard);
            }
            
            guard.animator.SetFloat(Speed, speed, 0.2f, Time.deltaTime);
           
            
            guard.Rotate(targetPos);
            
        }

        protected override void Attack(Guard guard)
        {
            var parentGuard = guard as GuardWithWeapon;
            
            // ReSharper disable once PossibleNullReferenceException
            parentGuard.rifle.FireBullet();

            lastAttackTime = Time.time;
        }
    }
}