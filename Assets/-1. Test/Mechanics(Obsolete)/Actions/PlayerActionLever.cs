using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Actions.Conditions;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Actions
{
    public class PlayerActionLever : PlayerActionBase
    {

        public string engageInput;
        public string engageAnimName;
        public string inverseEngageAnim;
        public float engageTransitionTime;
        public float engageTime;
        
        [Space(10)] public string actionInput;
        public bool invertedAction;
        public float actionTime;
        
        public List<TriggerCondition> conditions;


        public UnityEvent triggerAction;
        public UnityEvent unTriggerAction;
        
        private bool _triggered;
        private static readonly int Action = Animator.StringToHash("Action");

        private void Update()
        {
            if(!player) return;
            if(conditions.Any(condition=>condition.Condition(player))) return;

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
            
            player.animator.CrossFade(!_triggered? engageAnimName : inverseEngageAnim, 0.2f, 1);

            float timeElapsed = 0;
            Vector3 initPlayerPos = player.transform.position;
            Quaternion initPlayerRot = player.transform.rotation;
            
            while (timeElapsed <= engageTransitionTime)
            {
                player.transform.position = Vector3.Lerp(initPlayerPos, transform.position, timeElapsed / engageTransitionTime);
                player.transform.rotation = Quaternion.Lerp(initPlayerRot, transform.rotation, timeElapsed / engageTransitionTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }


            yield return new WaitForSeconds(engageTime - timeElapsed);
            
            while (Input.GetButton(engageInput))
            {
                
                if (!_triggered)
                {

                    if ((!invertedAction && Input.GetAxis(actionInput) < 0) ||
                        (invertedAction && Input.GetAxis(actionInput) > 0))
                    {
                        _triggered = true;
                        player.animator.SetBool(Action, true);
                        triggerAction.Invoke();
                        yield return new WaitForSeconds(actionTime);
                    }
                    
                }
                else if (_triggered)
                {
                    
                    if ((!invertedAction && Input.GetAxis(actionInput) > 0) ||
                        (invertedAction && Input.GetAxis(actionInput) < 0))
                    {
                        _triggered = false;
                        player.animator.SetBool(Action, false);
                        unTriggerAction.Invoke();
                        yield return new WaitForSeconds(actionTime);
                    }
                }


                yield return null;
            }

            player.animator.CrossFade("Default", 0.3f, 1);

            yield return new WaitForSeconds(0.4f);
            
            player.DisableAllMovement = false;
            player.characterController.enabled = true;
            
            coroutineExecute = null;
        }
        
    }
}
