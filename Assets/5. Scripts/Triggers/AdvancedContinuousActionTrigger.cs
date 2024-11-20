using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class AdvancedContinuousActionTrigger : MonoBehaviour
    {
        
        [FoldoutGroup("Input")] public string engageInput; //get one Thema input type for this also
        [FoldoutGroup("Input")] public string actionInput;
        [FoldoutGroup("Input")] public ThemaInput actionInputType = new ThemaInput(){inputType = KeyInputType.Key_Axis}; 
        [FoldoutGroup("Input")] public float actionTriggerTime;
        [FoldoutGroup("Input")] public List<TriggerCondition> conditions;

        [FoldoutGroup("Animation")] public Transform actionTransform;
        [FoldoutGroup("Animation")] public bool simpleEngage;
        [FoldoutGroup("Animation")] public float engageActionWidth;
        [FoldoutGroup("Animation")] public AdvancedCurvedAnimation engageAnim;
        [FoldoutGroup("Animation")] public string actionAnimName; //MAY BE CHANGE LATER TO MORE DYNAMIC


        [FoldoutGroup("Misc")] public Vector3 uiElementOffset = new Vector3(0, 2f, 0f);
        [FoldoutGroup("Misc")] public string actionInputVisualText;
        
        [FoldoutGroup("Events")] public float actionDelay;
        [FoldoutGroup("Events")] public float exitDelay;
        [FoldoutGroup("Events")] public UnityEvent actionEvent;


        private Coroutine _engagedCoroutine;
        private bool _playerInTrigger;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position - transform.right * engageActionWidth,
                transform.position + transform.right * engageActionWidth);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player_Main")) return;

            if (conditions.Any(condition => !condition.Condition(other))) return;
            if (_playerInTrigger) return;
            
            _playerInTrigger = true;

            if (_engagedCoroutine != null) StopCoroutine(ResetTrigger());
            StartCoroutine(ResetTrigger());
        }


        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.5f);
            _playerInTrigger = false;
        }

        private void Update()
        {
            if (!_playerInTrigger) return;

            if (Input.GetButtonDown(engageInput))
            {
                _engagedCoroutine ??= StartCoroutine(Engage());
            }
        }


        private IEnumerator Engage()
        {
            var player = PlayerMovementController.Instance.player;

            player.CanRotate = false;
            player.CController.enabled = false;
            player.DisabledPlayerMovement = true;
            player.MovementController.ResetAnimator();

            if (simpleEngage)
            {
                yield return engageAnim.SimpleAnim(player.AnimationController, player.transform, actionTransform? actionTransform: transform, engageActionWidth);
            }
            else
            {
                yield return engageAnim.PlayAnim(player.AnimationController, player.transform, actionTransform? actionTransform: transform, engageActionWidth);
            }


            float timeElapsed = 0;

            var uiManager = UIManager.Instance;
            uiManager.UpdateActionFillPos(transform.position, uiElementOffset);

            //Action
            while (!Input.GetButtonUp(engageInput))
            {
                if (actionInputType.GetInput(actionInput))
                {
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    timeElapsed = 0;
                }

                uiManager.UpdateActionFill(Mathf.Clamp01(timeElapsed / actionTriggerTime), actionInputVisualText);

                if (timeElapsed >= actionTriggerTime)
                {
                    player.AnimationController.CrossFade(actionAnimName, 0.2f, 1);
                    yield return new WaitForSeconds(actionDelay);
                    actionEvent.Invoke();
                    yield return new WaitForSeconds(exitDelay);
                    break;
                }

                yield return null;
            }


            player.AnimationController.CrossFade("Default", 0.1f, 1);
            player.DisabledPlayerMovement = false;
            player.CController.enabled = true;
            player.CanRotate = true;
            _engagedCoroutine = null;
        }
    }


    
}