using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Actions.Conditions;
using Mechanics.Player.Types;
using Mechanics.Types;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Actions
{
    public class PlayerActionTrigger : PlayerActionBase
    {

        public AdvancedCurvedAnimation animAction;
        public float actionWidth = 0.4f;
        public List<TriggerCondition> conditions;

        [Range(0,1), Space(10)] public float actionTiming;
        public UnityEvent action;
        
        
        private void Update()
        {
            if(!player) return;

            if (conditions.Any(condition => !condition.Condition(player))) return;
            
            coroutineExecute ??= StartCoroutine(Execute());
            
        }
        

        private IEnumerator Execute()
        {
            
            //disable player movements and character controller
            player.DisableAllMovement = true;
            player.characterController.enabled = false;
            player.ResetAnimator();
            
            //play animation
            TimedAction timedAction = new TimedAction(actionTiming, () => action.Invoke());
            yield return animAction.PlayAnim(transform, player, actionWidth, timedAction);

            //enable player movements and character controller
            
            player.characterController.enabled = true;
            player.DisableAllMovement = false;
            coroutineExecute = null;

        }
    }
}
