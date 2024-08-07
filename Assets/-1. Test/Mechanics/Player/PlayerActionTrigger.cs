using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Player.Conditions;
using UnityEngine;
using Mechanics.Player.Custom;
using UnityEngine.Events;

namespace Mechanics.Player
{
    public class PlayerActionTrigger : MonoBehaviour
    {

        public AdvancedCurvedAnimation animAction;
        public float actionWidth = 0.4f;
        public List<TriggerCondition> conditions;

        [Range(0,1), Space(10)] public float actionTiming;
        public UnityEvent action;


        private PlayerV1 _player;
        private Coroutine _coroutineExit;
        private Coroutine _coroutineExecute;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            
            if (!_player)
            {
                _player = other.GetComponent<PlayerV1>();
                _player.CanJump = false;
            }
                
            if (_coroutineExit != null)
            {
                StopCoroutine(_coroutineExit);
            }
            _coroutineExit = StartCoroutine(TriggerExit());
        }

        private IEnumerator TriggerExit()
        {
            yield return new WaitForSeconds(0.2f);

            yield return new WaitUntil(() => _coroutineExecute == null);
            _player.CanJump = true;
            _player = null;
            _coroutineExit = null;

        }


        private void Update()
        {
            if(!_player) return;

            if (conditions.All(condition => condition.Condition(_player)))
            {
                _coroutineExecute ??= StartCoroutine(Execute());
            }
        }
        

        private IEnumerator Execute()
        {
            
            //disable player movements and character controller
            _player.DisableAllMovement = true;
            _player.characterController.enabled = false;
            
            //play animation
            TimedAction timedAction = new TimedAction(actionTiming, () => action.Invoke());
            yield return animAction.PlayAnim(transform, _player, actionWidth, timedAction);

            //enable player movements and character controller
            _player.characterController.enabled = true;
            _player.DisableAllMovement = false;
            _coroutineExecute = null;

        }

    }
}
