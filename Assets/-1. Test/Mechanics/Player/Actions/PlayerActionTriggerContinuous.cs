using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Player.Conditions;
using Mechanics.Player.Custom;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Actions
{
    public class PlayerActionTriggerContinuous : MonoBehaviour
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
        
        
        private PlayerV1 _player;
        private Coroutine _coroutineExit;
        private Coroutine _coroutineExecute;
        private bool _actionTriggered;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position - transform.right * engageActionWidth,
                transform.position + transform.right * engageActionWidth);
        }
        
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
            if (!conditions.All(condition => condition.Condition(_player))) return;
            
            if (Input.GetButtonDown(engageInput))
            {
                _coroutineExecute ??= StartCoroutine(Engage());
            }
        }
        
        private IEnumerator Engage()
        {

            _player.DisableAllMovement = true;
            _player.characterController.enabled = false;

            
            yield return engageAnim.PlayAnim(transform, _player, engageActionWidth);
            
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
                    _player.animator.CrossFade(actionAnimName, 0.2f, 1);
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
                _player.animator.CrossFade(disEngageAnimName, 0.2f, 1);
                yield return new WaitForSeconds(disEngageTime);
            }
            
            
            _player.characterController.enabled = true;
            _player.DisableAllMovement = false;
            _actionTriggered = false;
            _coroutineExecute = null;

        }
        
    }
}
