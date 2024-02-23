using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPC.Bots
{
    public class SimpleAttackingBot : MonoBehaviour
    {

        [SerializeField] protected AISense sensor;
        [SerializeField, Space(20)] protected List<Vector3> defaultPositions;
        [SerializeField] protected float chaseSpeed;
        [SerializeField] protected float minimumAttackDistance;

        [Button("GetDefaultPosition", ButtonSizes.Medium)]
        public void GetDefaultPosition()
        {
            defaultPositions.Add(transform.position);
        }


        protected int _randomRestingIndex;
        protected BotState botState;
        protected float _time;

        protected virtual void Update()
        {
            if (sensor.targetFound)
            {
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (botState != BotState.Chase)
                {
                    botState = BotState.Chase;
                }
                ChaseTarget();
            }
            else
            {
                if (botState == BotState.Chase && botState != BotState.Attack)
                {
                    _randomRestingIndex = Random.Range(0, defaultPositions.Count);
                    botState = BotState.Hold;
                    _time = Time.time;
                }
                ResetBot();
            }
        }

        protected virtual void ResetBot()
        {
            if (botState == BotState.Rest) return;

            if (botState == BotState.Attack)
            {
                transform.position = Vector3.MoveTowards(transform.position, sensor.target.position, chaseSpeed / 2);
            }
            else if (botState == BotState.Hold)
            {
                if (Time.time > _time + 3f)
                {
                    botState = BotState.ReturnToDefault;
                }
                transform.position = Vector3.MoveTowards(transform.position, defaultPositions[_randomRestingIndex], chaseSpeed / 4);
            }
            else if (botState == BotState.ReturnToDefault)
            {
                transform.position = Vector3.MoveTowards(transform.position, defaultPositions[_randomRestingIndex], chaseSpeed);

                if (Mathf.Abs((transform.position - defaultPositions[_randomRestingIndex]).magnitude) < 0.1f)
                {
                    botState = BotState.Rest;
                }
            }


        }

        protected virtual void ChaseTarget()
        {

            if (Mathf.Abs((transform.position - sensor.target.position).magnitude) < minimumAttackDistance)
            {
                StartCoroutine(AttackTarget());
            }

            transform.position = Vector3.MoveTowards(transform.position, sensor.target.position, chaseSpeed);

        }

        protected IEnumerator AttackTarget()
        {
            sensor.targetFound = false;
            botState = BotState.Attack;
            PlayerController.Instance.Player.Health.PlayerDamage("Drag death");
            yield return new WaitForSeconds(1f);
            _randomRestingIndex = Random.Range(0, defaultPositions.Count);
        }


        #region Custom Type


        protected enum BotState
        {
            Rest,
            Chase,
            ReturnToDefault,
            Hold,
            Attack
        }


        #endregion

    }
}
