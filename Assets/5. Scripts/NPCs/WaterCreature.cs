using System;
using System.Collections;
using Cinemachine;
using Player_Scripts;
using Triggers;
using UnityEngine;
using UnityEngine.UIElements;
using VLB;

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
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackForce;
        [SerializeField] private float attackInterval;
        [SerializeField] private float restrictedY;
        

        [SerializeField] private bool playerInWater;


        private bool _isAttacking;
        public override void StateEnter(WaterCreature creature)
        {
        }

        public override void StateExit(WaterCreature creature)
        {
        }

        public override void StateFixedUpdate(WaterCreature creature)
        {
            MoveCreature(creature);
            //Rotate the creature in the direction of the player
            creature.Rotate(PlayerMovementController.Instance.transform.position);
        }

        public override void LateUpdate(WaterCreature creature)
        {
            //Restrict the y position of the creature smoothly
            Vector3 position = creature.transform.position;
            position.y = Mathf.Lerp(position.y, restrictedY, 0.1f);
            creature.transform.position = position;
        }

        //Create a function that moves the creature towards the player
        private void MoveCreature(WaterCreature creature)
        {
            
            if(_isAttacking) return;
            
            
            
            Vector3 playerPos = PlayerMovementController.Instance.transform.position;
            Transform creatureTransform = creature.transform;
            Vector3 direction = (playerPos - creature.transform.position).normalized;
            
            //restrict the y position of the creature
            creature.rb.AddForce(direction * attackSpeed);
            
            //if distance between player and creature is less than attackDistance, call the UnderWaterAttack function
            if (!(Vector3.Distance(creatureTransform.position, playerPos) < attackDistance)) return;

            //debug draw a line between player and creature
            Debug.DrawLine(creatureTransform.position, playerPos, Color.green, 0.1f);
            
            creature.StartCoroutine(Attack(creature));

        }

        private IEnumerator Attack(WaterCreature creature)
        {

            _isAttacking = true;
            
            if (playerInWater)
            {
                UnderWaterAttack(creature);
            }
            else
            {
                AboveWaterAttack(creature);
            }

            yield return new WaitForSeconds(attackInterval);

            _isAttacking = false;

        }

        private void UnderWaterAttack(WaterCreature creature)
        {
            //Add force to the creature in the direction of the player
            Vector3 direction = PlayerMovementController.Instance.transform.position - creature.transform.position;
            creature.rb.AddForce(direction.normalized * attackForce, ForceMode.Impulse);
        }
        
        private void AboveWaterAttack(WaterCreature creature)
        {
            //Add force to the creature in the direction of the player
            Vector3 direction = PlayerMovementController.Instance.transform.position - creature.transform.position;
            creature.rb.AddForce(direction.normalized * attackForce, ForceMode.Impulse);
        }
        
        
        public void Collision(WaterCreature creature, Collision other)
        {
            if (other.collider.CompareTag("Player_Main"))
            {
                PlayerMovementController.Instance.player.Health.TakeDamage(101);
            }
        }
        
    }

    public abstract class CreatureState
    {
        public abstract void StateEnter(WaterCreature creature);
        public abstract void StateExit(WaterCreature creature);
        public abstract void StateFixedUpdate(WaterCreature creature);
        public abstract void LateUpdate(WaterCreature creature);
    }
}