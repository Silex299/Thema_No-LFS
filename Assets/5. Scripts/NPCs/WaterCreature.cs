using System.Collections;
using Player_Scripts;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class WaterCreature : MonoBehaviour
    {
        [SerializeField] private GuardStateEnum state;
        [SerializeField] private float creatureRotationSpeed;
        [SerializeField] internal float creatureSpeed;
        [SerializeField] internal bool stayOnSurf;

        [SerializeField, Space(10)] internal Rigidbody rb;
        [SerializeField] internal Animator animator;

        /**
         * minY and maxY are the limits for the y position of the creature when moving roam and chase states
         * defaultY is the y position of the creature moves away from player after attack;
         **/
        [SerializeField, Space(10)] internal float minY;

        [SerializeField] internal float defaultY;
        [SerializeField] internal float maxY;


        private CreatureState _currentState;
        [SerializeField] private CreatureRoamState roamState = new CreatureRoamState();
        [SerializeField] private CreatureChaseState chaseState = new CreatureChaseState();


        private void Start()
        {
            ChangeState(state);
        }

        private void OnEnable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath -= OnPlayerDeath;
        }

        private void Update()
        {
            if (!enabled) return;

            _currentState.StateUpdate(this);
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

            _currentState?.StateEnter(this);
        }


        //Create a function that rotates the creature towards the player
        public void Rotate(Vector3 rotateTowards, float speedMultiplier = 1)
        {
            Vector3 direction = rotateTowards - transform.position;


            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
                creatureRotationSpeed * speedMultiplier * Time.fixedDeltaTime);
        }

        private void OnPlayerDeath()
        {
            _currentState?.OnPlayerDeath(this);
        }
    }


    [System.Serializable]
    public class CreatureRoamState : CreatureState
    {
        [SerializeField] private float roamRadius;
        [SerializeField] private float roamSpeed;
        private Vector3 _center;

        public override void StateEnter(WaterCreature creature)
        {
            _center = creature.transform.position;
            if (creature.stayOnSurf)
            {
                //set y value to be defaultY
                creature.transform.position = new Vector3(creature.transform.position.x, creature.defaultY,
                    creature.transform.position.z);
            }
        }

        public override void StateExit(WaterCreature creature)
        {
        }

        public override void StateUpdate(WaterCreature creature)
        {
            //move the creature in a circular path around the center with a radius 10  
            float angle = Time.time * roamSpeed;
            float x = Mathf.Cos(angle) * roamRadius;
            float z = Mathf.Sin(angle) * roamRadius;

            float y;
            if (creature.stayOnSurf)
            {
                y = creature.defaultY;
            }
            else
            {
                y = Mathf.Sin(angle) * (creature.maxY - creature.minY) / 2 + (creature.maxY + creature.minY) / 2;
            }

            //set the y position to the default y position
            Vector3 newPos = new Vector3(_center.x + x, y, _center.z + z);
            creature.transform.position = newPos;

            //get tangent to the circle
            Vector3 tangent = new Vector3(-z, y, x).normalized;
            creature.Rotate(newPos + tangent, 5);
        }

        public override void OnPlayerDeath(WaterCreature creature)
        {
        }
    }

    [System.Serializable]
    public class CreatureChaseState : CreatureState
    {
        [SerializeField] private float attackDistance;
        [SerializeField] private float afterAttackDistance;
        [SerializeField] private float timeToAttack = 1;

        [SerializeField] private GameObject hitEffect;
        [SerializeField] private bool playerInWater;


        private bool _isAttacking;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Chase = Animator.StringToHash("Chase");

        public override void StateEnter(WaterCreature creature)
        {
            if (creature.stayOnSurf)
            {
                creature.transform.position = new Vector3(creature.transform.position.x, creature.defaultY,
                    creature.transform.position.z);
            }

            creature.animator.SetBool(Chase, true);
        }

        public override void StateExit(WaterCreature creature)
        {
            _isAttacking = false;
            creature.animator.SetBool(Chase, false);
        }


        public override void StateUpdate(WaterCreature creature)
        {
            MoveCreature(creature);
        }

        public override void OnPlayerDeath(WaterCreature creature)
        {
            creature.ChangeState(GuardStateEnum.Guard);
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

            targetPosition.y =
                creature.stayOnSurf ? creature.defaultY : Mathf.Clamp(targetPosition.y, creature.minY, creature.maxY);

            creature.transform.position = Vector3.MoveTowards(creature.transform.position, targetPosition,
                creature.creatureSpeed * Time.deltaTime);

            creature.Rotate(targetPosition, 0.5f);

            float distance = Vector3.Distance(creature.transform.position, targetPosition);

            if (playerInWater)
            {
                if (distance < 1)
                {
                    creature.StartCoroutine(AttackPlayerUnderWater(creature));
                }
            }
            else
            {
                if (distance < attackDistance)
                {
                    creature.StartCoroutine(AttackPlayer(creature));
                }
            }
        }

        private IEnumerator AttackPlayerUnderWater(WaterCreature creature)
        {
            _isAttacking = true;
            creature.animator.SetTrigger(Attack);
            yield return new WaitForSeconds(0.2f);

            PlayerMovementController.Instance.player.Health.TakeDamage(101);

            _isAttacking = false;
        }

        //Create a function that attacks the player
        private IEnumerator AttackPlayer(WaterCreature creature)
        {
            _isAttacking = true;

            Vector3 targetPosition = PlayerMovementController.Instance.player.transform.position;
            Vector3 creaturePosition = creature.transform.position;

            float timeElapse = 0;

            creature.animator.SetTrigger(Attack);

            while (timeElapse < timeToAttack)
            {
                timeElapse += Time.deltaTime;
                float fraction = timeElapse / timeToAttack;

                var y = Mathf.Lerp((targetPosition.y - 2), Mathf.Clamp(targetPosition.y, -100, creature.maxY),
                    Mathf.Pow(fraction, 2));

                Vector3 moveTo = new Vector3(targetPosition.x, y, targetPosition.z);

                creature.transform.position = Vector3.Lerp(creaturePosition, moveTo, fraction);
                creature.Rotate(moveTo);

                yield return null;
            }


            MonoBehaviour.Instantiate(hitEffect, creature.transform.position, Quaternion.identity);
             
            //If distance between the creature and the player are less than 2, Player plays animation 
            if (Vector3.Distance(creature.transform.position, PlayerMovementController.Instance.transform.position) < 2)
            {
                PlayerMovementController.Instance.DisablePlayerMovement(true);
                PlayerMovementController.Instance.PlayAnimation("Tripping", 0.2f, 1);
            }

            
            
            float angle = Vector3.Angle(creature.transform.forward, Vector3.right);

            Vector3 moveTo1 = angle > 90
                ? creaturePosition - Vector3.right * afterAttackDistance
                : creaturePosition + Vector3.right * afterAttackDistance;

            moveTo1.y = creature.defaultY;


            timeElapse = 0;
            while (true)
            {
                timeElapse += Time.deltaTime;

                if (timeElapse < 0.5)
                {
                    moveTo1.y = creature.defaultY - 2;
                }
                else
                {
                    moveTo1.y = creature.defaultY;
                }

                //move creature to moveTo1 with speed creatureSpeed
                creature.transform.position = Vector3.MoveTowards(creature.transform.position, moveTo1,
                    creature.creatureSpeed * Time.deltaTime);
                creature.Rotate(moveTo1);

                //if the distance between the creature and the moveTo1 is less than 0.1f break the loop
                if (Vector3.Distance(creature.transform.position, moveTo1) < 0.01f)
                {
                    break;
                }

                yield return null;
            }

            
           
            _isAttacking = false;
        }
        
    }

    public abstract class CreatureState
    {
        public abstract void StateEnter(WaterCreature creature);
        public abstract void StateExit(WaterCreature creature);
        public abstract void StateUpdate(WaterCreature creature);
        public abstract void OnPlayerDeath(WaterCreature creature);
    }
}