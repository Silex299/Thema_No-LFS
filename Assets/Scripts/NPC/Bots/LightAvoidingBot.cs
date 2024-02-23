using UnityEngine;
using Player_Scripts;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NPC.Bots
{
    public class LightAvoidingBot : MonoBehaviour
    {

        [SerializeField] private AISense sense;
        [SerializeField] private float minimumAttackDistance;
        [SerializeField] private float minimumAvoidDistance;
        [SerializeField] private List<Vector3> defaultPositions;

        [Button("GetPositon", ButtonSizes.Medium)]
        public void GetPosition()
        {
            defaultPositions.Add(transform.position);
        }

        [SerializeField] private float chaseSpeed;
        [SerializeField] private float rotationSmoothness;
        public Transform avoidSource;

        private BotState _botState;
        private float turnSmoothVelocity;
        private float time;
        private int randomIndex;

        private void FixedUpdate()
        {

            if (sense.targetFound)
            {
                if (_botState is not BotState.Chase)
                {
                    _botState = BotState.Chase;
                }
                else
                {
                    ChaseTargte();
                }
            }
            else
            {

                if (_botState is BotState.Idle) return;
                ResetBot();
            }

        }

        private void ChaseTargte()
        {
            if (_botState == BotState.Chase)
            {

                RotateBot();



                var position = transform.position;
                var targetPosition = sense.target.position;
                var newPos = new Vector3(targetPosition.x, position.y, targetPosition.z);

                if (avoidSource)
                {
                    var pos = (transform.position - avoidSource.position).normalized * minimumAvoidDistance + avoidSource.position;
                    newPos = new Vector3(pos.x, newPos.y, pos.z);
                }
                else
                {
                    if (Mathf.Abs((targetPosition - position).magnitude) < minimumAttackDistance)
                    {
                        _botState = BotState.Attack;
                        Attack();
                    }
                }
                transform.position = Vector3.MoveTowards(position, newPos, chaseSpeed * Time.deltaTime);

            }
        }


        private void Attack()
        {
            PlayerController.Instance.Player.Health.PlayerDamage("fall flat");
        }


        private void ResetBot()
        {

            if (_botState is not BotState.Hold or BotState.Return)
            {
                _botState = BotState.Hold;
                randomIndex = Random.Range(0, defaultPositions.Count);
                time = Time.time;
                return;
            }
            else
            {
                if (Time.time > time + 3)
                {
                    if (_botState is BotState.Hold)
                    {
                        _botState = BotState.Return;
                    }
                }

                var position = transform.position;

                if (_botState is BotState.Hold)
                {
                    transform.position = Vector3.MoveTowards(position, defaultPositions[randomIndex], chaseSpeed / 2 * Time.deltaTime);
                }
                else if (_botState is BotState.Return)
                {
                    transform.position = Vector3.MoveTowards(position, defaultPositions[randomIndex], chaseSpeed * Time.deltaTime);

                    if (Mathf.Abs((defaultPositions[randomIndex] - position).magnitude) < 0.01f)
                    {
                        _botState = BotState.Idle;
                    }
                }

                RotateBot();
            }

        }



        private void RotateBot()
        {
            Vector3 direction;
            if (sense.targetFound)
            {
                direction = sense.target.position - transform.position;
            }
            else
            {
                direction = defaultPositions[randomIndex] - transform.position;
            }

            //Rotate the player in desired direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothness);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

    }

    public enum BotState
    {
        Idle,
        Chase,
        Attack,
        Hold,
        Return
    }


}


