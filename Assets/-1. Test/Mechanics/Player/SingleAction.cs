using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Player.Conditions;
using Mechanics.Player.Custom;
using UnityEngine;

namespace Mechanics.Player
{
    public class SingleAction : MonoBehaviour
    {
        public AdvancedCurvedAnimation action;
        public float actionWidth = 0.2f;
        public List<TriggerCondition> conditions;
        
        private Coroutine _executeAction;
        private PlayerV1 _player;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _player = other.GetComponent<PlayerV1>();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _player = null;
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + transform.right * actionWidth, transform.position - transform.right * actionWidth);
        }


        private void Update()
        {
            if (!_player) return;
            if (_executeAction != null) return;
            
            if(conditions.All(condition=>condition.Condition(_player)))
            {
                _executeAction = StartCoroutine(ExecuteAction(_player));
            }

        }


        private IEnumerator ExecuteAction(PlayerV1 player)
        {

            print("Callinmg");
            player.DisableAllMovement = true;
            
            yield return action.PlayAnim(transform, player, actionWidth);
            
            _executeAction = null;
            player.DisableAllMovement = false;
        }
    }
}
