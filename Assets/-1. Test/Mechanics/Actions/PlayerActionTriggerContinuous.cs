using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Actions.Conditions;
using Mechanics.Player.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Actions
{
    public class PlayerActionTriggerContinuous : PlayerActionBase
    {
        
        [FoldoutGroup("Input")] public string engageInput;
        [FoldoutGroup("Input")] public string actionInput;
        [FoldoutGroup("Input")] public float actionTriggerTime;
        [FoldoutGroup("Input")] public List<TriggerCondition> conditions;

        [FoldoutGroup("Animation")] public float engageActionWidth;
        [FoldoutGroup("Animation")] public AdvancedCurvedAnimation engageAnim;
        [FoldoutGroup("Animation")] public float actionTime; //MAY BE CHANGE LATER TO MORE DYNAMIC
        [FoldoutGroup("Animation")] public string actionAnimName; //MAY BE CHANGE LATER TO MORE DYNAMIC
        [FoldoutGroup("Animation")] public float disEngageTime;
        [FoldoutGroup("Animation")] public string disEngageAnimName;


        [FoldoutGroup("Events")] public float actionDelay;
        [FoldoutGroup("Events")] public UnityEvent actionEvent;
        
        
        private bool _actionTriggered;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position - transform.right * engageActionWidth,
                transform.position + transform.right * engageActionWidth);
        }
        
        
        private void Update()
        {
            if(!player) return;
            if (conditions.Any(condition => !condition.Condition(player))) return;
            
            if (Input.GetButtonDown(engageInput))
            {
                coroutineExecute ??= StartCoroutine(Engage());
            }
        }
        
        private IEnumerator Engage()
        {

            player.DisableAllMovement = true;
            player.characterController.enabled = false;
            player.ResetAnimator();
            
            yield return engageAnim.PlayAnim(transform, player, engageActionWidth);
            
            float timeElapsed = 0;

            //UPDATE UI
            //var uiManager = UIManager.Instance;
            //uiManager.UpdateActionFillPos(transform.position, new Vector3(0, 2f, 0f));

            //Action
            while (!Input.GetButtonUp(engageInput))
            {
                if (Input.GetButton(actionInput))
                {
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    timeElapsed = 0;
                }

                //uiManager.UpdateActionFill(Mathf.Clamp01(timeElapsed / actionTriggerTime), actionInput);

                if (timeElapsed >= actionTriggerTime)
                {
                    player.animator.CrossFade(actionAnimName, 0.2f, 1);
                    _actionTriggered = true;
                    
                    yield return new WaitForSeconds(actionDelay);
                    actionEvent.Invoke();

                    yield return new WaitForSeconds(actionTime - actionDelay);
                    break;
                }

                yield return null;
            }

            if (!_actionTriggered)
            {
                player.animator.CrossFade(disEngageAnimName, 0.2f, 1);
                yield return new WaitForSeconds(disEngageTime);
            }
            
            player.characterController.enabled = true;
            player.DisableAllMovement = false;
            _actionTriggered = false;
            coroutineExecute = null;

        }
        
    }
}
