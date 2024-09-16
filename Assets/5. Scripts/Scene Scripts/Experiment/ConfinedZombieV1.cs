using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Scene_Scripts.Experiment
{
    public class ConfinedZombieV1 : MonoBehaviour
    {
        #region ExposeedVariables

        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public Transform[] waypoints;
        [FoldoutGroup("References")] public Transform rigAimTarget;
        [FoldoutGroup("References")] public Rig aimRig;
        [FoldoutGroup("References")] public Transform target;

        [FoldoutGroup("Movement")] public float rotationSpeed = 10f;
        [FoldoutGroup("Movement")] public float stopDistance = 1f;

        [FoldoutGroup("Animation and Rig")] public string entryAnimation;

        [FoldoutGroup("Misc")] public ZombieTentacles tentacles;

        #endregion

        #region States

        private ConfinedZombieBase _currentState;
        private ZombieState _currentStateType = ZombieState.None;

        private readonly ConfinedZombieIdle _idleState = new ConfinedZombieIdle();
        private readonly ConfinedZombieFollow _followState = new ConfinedZombieFollow();
        private readonly ConfinedZombieScream _screamState = new ConfinedZombieScream();

        #endregion

        #region Getter & Setter
        public Transform Target
        {
            get
            {
                if (!target)
                {
                    target = PlayerMovementController.Instance.transform;
                }
                return target;
            }
            set => target = value;
        }
        #endregion


        #region Built-in Methods

        private void Start()
        {
            ChangeState(ZombieState.Idle);
        }

        private void Update()
        {
            _currentState.UpdateState(this);
        }

        #endregion

        #region Other Methods

        public void ChangeState(int stateIndex)
        {
            ChangeState((ZombieState)stateIndex);
        }

        public void ChangeState(ZombieState newState)
        {
            if (_currentStateType == newState)
            {
                return;
            }

            _currentStateType = newState;
            _currentState = newState switch
            {
                ZombieState.Idle => _idleState,
                ZombieState.Follow => _followState,
                ZombieState.Scream => _screamState,
                _ => _currentState
            };
            _currentState.EnterState(this);
        }

        public Vector3 GetDesiredPos()
        {
            return ThemaVector.GetClosestPointToLine(waypoints[0].position, waypoints[1].position, Target.position);
        }

        public void FollowAimRig()
        {
            rigAimTarget.position = Target.position;
        }

        public void Reset()
        {
            ChangeState(ZombieState.Idle);
            Target = null;
        }
        
        #endregion
    }

    #region Zombie States & Type

    public abstract class ConfinedZombieBase
    {
        public abstract void EnterState(ConfinedZombieV1 zombie);
        public abstract void UpdateState(ConfinedZombieV1 zombie);
    }

    public class ConfinedZombieIdle : ConfinedZombieBase
    {
        private static readonly int StateIndex = Animator.StringToHash("State");

        public override void EnterState(ConfinedZombieV1 zombie)
        {
            zombie.animator.SetInteger(StateIndex, 0);
            zombie.animator.CrossFade(zombie.entryAnimation, 0.2f);
            zombie.aimRig.weight = 0;
            if(zombie.tentacles) zombie.tentacles.StopScream();
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
        }
    }

    public class ConfinedZombieFollow : ConfinedZombieBase
    {
        private static readonly int State = Animator.StringToHash("State");
        private static readonly int Speed = Animator.StringToHash("Speed");


        public override void EnterState(ConfinedZombieV1 zombie)
        {
            if (!zombie.Target)
            {
                zombie.ChangeState(ZombieState.Idle);
                return;
            }

            zombie.animator.SetInteger(State, 1);
            if(zombie.tentacles) zombie.tentacles.StopScream();
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
            Vector3 desiredPos = zombie.GetDesiredPos();

            float plannerDistance = ThemaVector.PlannerDistance(desiredPos, zombie.transform.position);

            float speed = 0;

            if (plannerDistance > zombie.stopDistance)
            {
                if (desiredPos.z > zombie.transform.position.z)
                {
                    speed = 1;
                }
                else
                {
                    speed = -1;
                }

                if (Mathf.Approximately(zombie.aimRig.weight, 0))
                {
                    zombie.aimRig.weight = 1;
                }
                zombie.FollowAimRig();
            }
            else
            {
                if (Mathf.Approximately(zombie.aimRig.weight, 1))
                {
                    zombie.aimRig.weight = 0;
                }
            }


            Rotate(zombie, desiredPos, speed);
            zombie.animator.SetFloat(Speed, speed, 0.5f, Time.deltaTime);
        }
        
        
        private void Rotate(ConfinedZombieV1 zombie, Vector3 lookAt, float dir = 0)
        {
            Vector3 forward;
            if (dir == 0)
            {
                forward = zombie.transform.forward * 10;
            }
            else if (dir > 0)
            {
                forward = lookAt - zombie.transform.position;
            }
            else
            {
                forward = zombie.transform.position - lookAt;
            }

            forward.y = 0;
            //Rotate the zombie to look at the target, but only on the Y axis
            Quaternion desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, desiredRotation, zombie.rotationSpeed * Time.deltaTime);
        }

        
    }

    public class ConfinedZombieScream : ConfinedZombieBase
    {
        private static readonly int State = Animator.StringToHash("State");

        public override void EnterState(ConfinedZombieV1 zombie)
        {
            if (!zombie.Target)
            {
                zombie.ChangeState(ZombieState.Idle);
                return;
            }

            zombie.animator.SetInteger(State, 2);
            if(zombie.tentacles) zombie.tentacles.Scream();
        }

        public override void UpdateState(ConfinedZombieV1 zombie)
        {
            Rotate(zombie);
        }

        private void Rotate(ConfinedZombieV1 zombie)
        {
            Vector3 forward = zombie.Target.position - zombie.transform.position;
            forward.y = 0;
            //Rotate the zombie to look at the target, but only on the Y axis
            Quaternion desiredRotation = Quaternion.LookRotation(forward, Vector3.up);
            //rotate desiredRotation by 90 degrees on the Y axis
            desiredRotation *= Quaternion.Euler(0, -90, 0);
            
            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, desiredRotation, zombie.rotationSpeed * Time.deltaTime);
        }
    }

    public enum ZombieState
    {
        Idle,
        Follow,
        Scream,
        None
    }

    #endregion
}