using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs
{
    public class WaterCreature : MonoBehaviour
    {
        #region State Settings

        [BoxGroup("State Settings"), SerializeField]
        private GuardStateEnum state;

        [BoxGroup("State Settings"), SerializeField]
        private CreatureRoamState roamState = new CreatureRoamState();

        [BoxGroup("State Settings"), SerializeField]
        private CreatureChaseState chaseState = new CreatureChaseState();
        
        private CreatureState _currentState;

        #endregion

        #region Creature Settings

        [BoxGroup("Creature Settings"), SerializeField]
        private float creatureRotationSpeed;

        [BoxGroup("Creature Settings"), SerializeField]
        internal float creatureSpeed;

        [BoxGroup("Creature Settings"), SerializeField]
        internal bool stayOnSurf;

        #endregion

        #region Creature Components

        [BoxGroup("Creature Components"), SerializeField, Space(10)]
        internal Rigidbody rb;

        [BoxGroup("Creature Components"), SerializeField]
        internal Animator animator;

        #endregion

        #region Position Settings

        [BoxGroup("Position Settings"), SerializeField, Space(10)]
        internal float minY;

        [BoxGroup("Position Settings"), SerializeField]
        internal float defaultY;

        [BoxGroup("Position Settings"), SerializeField]
        internal float maxY;

        private static readonly int InWater = Animator.StringToHash("PlayerInWater");

        #endregion


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

        //Create a function that changes the current state with integer 0 as roam and 1 as chase
        public void ChangeState(int newState)
        {
            ChangeState((GuardStateEnum) newState);
        }
        

        public void PlayerInWater(bool inWater)
        {
            animator.SetBool(InWater, inWater);
            //change PlayerInWater in chase state
            chaseState.playerInWater = inWater;
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

        /// <summary>
        /// Sets the initial state of the creature when it starts roaming.
        /// If the creature stays on the surface, its y position is set to defaultY.
        /// </summary>
        /// <param name="creature">The creature that is entering the state.</param>
        public override void StateEnter(WaterCreature creature)
        {
            // Set the center position to the creature's current position
            _center = creature.transform.position;

            // Check if the creature stays on the surface
            if (creature.stayOnSurf)
            {
                // If it does, set the y value of the creature's position to be defaultY
                creature.transform.position = new Vector3(creature.transform.position.x, creature.defaultY,
                    creature.transform.position.z);
            }
        }

        public override void StateExit(WaterCreature creature)
        {
        }

        public override void StateUpdate(WaterCreature creature)
        {
            MovePlayer(creature);
        }

        public override void OnPlayerDeath(WaterCreature creature)
        {
        }

        /// <summary>
        /// Moves the creature in a circular path around a center point.
        /// The creature's y position is determined based on whether it stays on the surface or not.
        /// </summary>
        /// <param name="creature">The creature that is moving.</param>
        private void MovePlayer(WaterCreature creature)
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
    }

    [System.Serializable]
    public class CreatureChaseState : CreatureState
    {
        [SerializeField] private float attackDistance;
        [SerializeField] private float afterAttackDistance;
        [SerializeField] private float timeToAttack = 1;

        [SerializeField] private GameObject hitEffect;
        [SerializeField] internal bool playerInWater;


        private bool _isAttacking;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Chase = Animator.StringToHash("Chase");

        #region State Methods

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
            Debug.Log("Calling State Exit");
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

        #endregion

        #region Movement and Attack Methods

        /// <summary>
        /// Moves the creature towards the player and initiates an attack if the player is within a certain distance.
        /// The creature's movement is halted if it is currently attacking.
        /// </summary>
        /// <param name="creature">The creature that is performing the movement and potential attack.</param>
        private void MoveCreature(WaterCreature creature)
        {
            // Check if the creature is currently attacking
            if (_isAttacking) return;

            // Get the player's position
            Vector3 targetPosition = PlayerMovementController.Instance.player.transform.position;

            // If the creature stays on the surface, set the y position to defaultY
            // Otherwise, clamp the y position between minY and maxY
            targetPosition.y = creature.stayOnSurf
                ? creature.defaultY
                : Mathf.Clamp(targetPosition.y, creature.minY, creature.maxY);

            // Move the creature towards the player at a speed of creatureSpeed
            creature.transform.position = Vector3.MoveTowards(creature.transform.position, targetPosition,
                creature.creatureSpeed * Time.deltaTime);

            // Rotate the creature towards the player
            creature.Rotate(targetPosition, 0.5f);

            // Calculate the distance between the creature and the player
            float distance = Vector3.Distance(creature.transform.position, targetPosition);

            // If the player is in water and the distance is less than 1, start the underwater attack
            if (playerInWater)
            {
                if (distance < 1)
                {
                    creature.StartCoroutine(AttackPlayerUnderWater(creature));
                }
            }
            // If the distance is less than the attack distance, start the attack
            else if (distance < attackDistance)
            {
                creature.StartCoroutine(AttackPlayer(creature));
            }
        }

        /// <summary>
        /// This coroutine handles the underwater attack of the creature.
        /// It triggers the attack animation, waits for a short period, applies damage to the player, and then ends the attack.
        /// </summary>
        /// <param name="creature">The creature that is performing the attack.</param>
        private IEnumerator AttackPlayerUnderWater(WaterCreature creature)
        {
            _isAttacking = true;

            creature.animator.SetTrigger(Attack);

            yield return new WaitForSeconds(0.2f);

            PlayerMovementController.Instance.player.Health.TakeDamage(101);

            _isAttacking = false;
        }

        /// <summary>
        /// This coroutine handles the attack of the creature.
        /// It triggers the attack animation, moves the creature towards the player, applies damage to the player, and then moves the creature away from the player.
        /// </summary>
        /// <param name="creature">The creature that is performing the attack.</param>
        private IEnumerator AttackPlayer(WaterCreature creature)
        {
            _isAttacking = true;

            Vector3 targetPosition = PlayerMovementController.Instance.player.transform.position;
            Vector3 creaturePosition = creature.transform.position;

            float timeElapse = 0;

            creature.animator.SetTrigger(Attack);

            // Move creature towards the player and rotate it
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

            // Instantiate hit effect
            MonoBehaviour.Instantiate(hitEffect, creature.transform.position, Quaternion.identity);

            // If creature is close to player, trigger player animation
            if (Vector3.Distance(creature.transform.position, PlayerMovementController.Instance.transform.position) < 2)
            {
                PlayerMovementController.Instance.DisablePlayerMovement(true);
                PlayerMovementController.Instance.PlayAnimation("Tripping", 0.2f, 1);
            }

            // Determine direction for creature to move after attack
            float angle = Vector3.Angle(creature.transform.forward, Vector3.right);
            Vector3 moveTo1 = angle > 90
                ? creaturePosition - Vector3.right * afterAttackDistance
                : creaturePosition + Vector3.right * afterAttackDistance;

            moveTo1.y = creature.defaultY;

            // Move creature away from player after attack
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

                creature.transform.position = Vector3.MoveTowards(creature.transform.position, moveTo1,
                    creature.creatureSpeed * Time.deltaTime);
                creature.Rotate(moveTo1);

                if (Vector3.Distance(creature.transform.position, moveTo1) < 0.01f)
                {
                    break;
                }

                yield return null;
            }

            _isAttacking = false;
        }
        
        #endregion
    }

    public abstract class CreatureState
    {
        public abstract void StateEnter(WaterCreature creature);
        public abstract void StateExit(WaterCreature creature);
        public abstract void StateUpdate(WaterCreature creature);
        public abstract void OnPlayerDeath(WaterCreature creature);
    }
}