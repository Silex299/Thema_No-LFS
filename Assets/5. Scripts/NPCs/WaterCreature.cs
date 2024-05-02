using System.Collections;
using Player_Scripts;
using UnityEngine;


namespace NPCs
{
    public class WaterCreature : MonoBehaviour
    {
        [SerializeField] private GuardStateEnum state;
        [SerializeField] private float creatureRotationSpeed;
        [SerializeField] internal float creatureSpeed;
        [SerializeField] internal Rigidbody rb;


        private CreatureState _currentState;
        [SerializeField] private CreatureRoamState roamState = new CreatureRoamState();
        [SerializeField] private CreatureChaseState chaseState = new CreatureChaseState();


        private void OnCollisionEnter(Collision other)
        {
            //if state is chase call the Collision function in chaseState
            if (state == GuardStateEnum.Chase)
            {
                chaseState.Collision(this, other);
            }
        }

        private void Start()
        {
            ChangeState(state);
        }

        private void Update()
        {
            if (!enabled) return;

            _currentState.StateUpdate(this);
        }

        private void FixedUpdate()
        {
            if (!enabled) return;

            _currentState.StateFixedUpdate(this);
        }

        private void LateUpdate()
        {
            if (!enabled) return;

            _currentState.LateUpdate(this);
        }

        //Create a function that changes the current state
        public void ChangeState(GuardStateEnum newState)
        {
            _currentState?.StateExit(this);
            state = newState;

            switch (state)
            {
                case GuardStateEnum.Guard:
                    _currentState = roamState;
                    break;
                case GuardStateEnum.Chase:
                    _currentState = chaseState;
                    break;
            }

            _currentState.StateEnter(this);
        }


        //Create a function that rotates the creature towards the player
        public void Rotate(Vector3 rotateTowards)
        {
            Vector3 direction = rotateTowards - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
                creatureRotationSpeed * Time.fixedDeltaTime);
        }
    }


    [System.Serializable]
    public class CreatureRoamState : CreatureState
    {
        [SerializeField] private Transform[] roamPoints;
        [SerializeField] private float stopDistance;


        private int _currentRoamPoint;

        public override void StateEnter(WaterCreature creature)
        {
        }

        public override void StateExit(WaterCreature creature)
        {
        }

        public override void StateUpdate(WaterCreature creature)
        {
        }

        public override void StateFixedUpdate(WaterCreature creature)
        {
            MoveCreature(creature);
            //Rotate the creature in the direction of the current roam point
            creature.Rotate(roamPoints[_currentRoamPoint].position);
        }

        public override void LateUpdate(WaterCreature creature)
        {
        }


        //Create a function to move the creature to the next roam point and change to the next roam point if the distance between creature and roam point is less than stopDistance
        private void MoveCreature(WaterCreature creature)
        {
            if (Vector3.Distance(creature.transform.position, roamPoints[_currentRoamPoint].position) < stopDistance)
            {
                ChangeRoamPoint();
            }
            else
            {
                creature.transform.position = Vector3.MoveTowards(creature.transform.position,
                    roamPoints[_currentRoamPoint].position, creature.creatureSpeed * Time.fixedDeltaTime);
            }
        }


        //Create a function that changes the currentRoamPoint to the next one in the array
        private void ChangeRoamPoint()
        {
            _currentRoamPoint = (_currentRoamPoint + 1) % roamPoints.Length;
        }
    }

    [System.Serializable]
    public class CreatureChaseState : CreatureState
    {
        [SerializeField] private float attackDistance;

        [SerializeField] private float minY;
        [SerializeField] private float defaultY;
        [SerializeField] private float maxY;


        [SerializeField] private bool playerInWater;


        private bool _isAttacking;

        public override void StateEnter(WaterCreature creature)
        {
        }

        public override void StateExit(WaterCreature creature)
        {
        }


        public override void StateUpdate(WaterCreature creature)
        {
            MoveCreature(creature);
        }

        public override void StateFixedUpdate(WaterCreature creature)
        {
        }

        public override void LateUpdate(WaterCreature creature)
        {
        }


        //Create a function that moves the creature towards the player
        private void MoveCreature(WaterCreature creature)
        {
            if (_isAttacking)
            {
                return;
            }

            //Move the creature towards the player with restricting the y position
            Vector3 targetPosition = PlayerMovementController.Instance.player.transform.position;

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            creature.transform.position = Vector3.MoveTowards(creature.transform.position, targetPosition,
                creature.creatureSpeed * Time.deltaTime);


            //Attack the player if the distance between the player and the creature is less than attackDistance
            if (!(Vector3.Distance(creature.transform.position, targetPosition) < attackDistance)) return;

            Debug.Log("calling again and again");
            creature.StartCoroutine(AttackPlayer(creature));
        }

        public float timeToAttack = 1;

        //Create a function that attacks the player
        private IEnumerator AttackPlayer(WaterCreature creature)
        {
            _isAttacking = true;

            Vector3 targetPosition = PlayerMovementController.Instance.player.transform.position;
            Vector3 creaturePosition = creature.transform.position;

            float y = targetPosition.y - 2;
            float timeElapse = 0;

            while (timeElapse < timeToAttack)
            {
                timeElapse += Time.deltaTime;
                float fraction = timeElapse / timeToAttack;

                y = Mathf.Lerp((targetPosition.y - 2), Mathf.Clamp(targetPosition.y, -100, maxY),
                    Mathf.Pow(fraction, 2));

                Vector3 moveTo = new Vector3(targetPosition.x, y, targetPosition.z);

                creature.transform.position = Vector3.Lerp(creaturePosition, moveTo, fraction);
                creature.Rotate(moveTo);

                yield return null;
            }

            creaturePosition = creature.transform.position;

            float angle = Vector3.Angle(creature.transform.forward, Vector3.right);

            Vector3 moveTo_1 = angle > 90
                ? creaturePosition - Vector3.right * (attackDistance + 1)
                : creaturePosition + Vector3.right * (attackDistance + 1);
            moveTo_1.y = defaultY;

            timeElapse = 0;

            while (timeElapse < timeToAttack)
            {
                timeElapse += Time.deltaTime;
                float fraction = timeElapse / timeToAttack;

                creature.transform.position = Vector3.Lerp(creaturePosition, moveTo_1, fraction);
                creature.Rotate(moveTo_1);

                yield return null;
            }

            _isAttacking = false;
        }


        public void Collision(WaterCreature creature, Collision other)
        {
            Debug.Log(other.collider.name);
        }
    }

    public abstract class CreatureState
    {
        public abstract void StateEnter(WaterCreature creature);
        public abstract void StateExit(WaterCreature creature);
        public abstract void StateUpdate(WaterCreature creature);
        public abstract void StateFixedUpdate(WaterCreature creature);
        public abstract void LateUpdate(WaterCreature creature);
    }
}