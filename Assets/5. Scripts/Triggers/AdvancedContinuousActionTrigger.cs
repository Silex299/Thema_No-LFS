using System.Collections;
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
        [BoxGroup("Input")] public string engageInput;
        [BoxGroup("Input")] public string actionInput;
        [BoxGroup("Input")] public float actionTriggerTime;

        [BoxGroup("Animation")] public bool simpleEngage;
        [BoxGroup("Animation")] public AdvancedCurvedAnimation engageAnim;
        [BoxGroup("Animation")] public string actionAnimName; //MAY BE CHANGE LATER TO MORE DYNAMIC


        [BoxGroup("Events")] public float actionDelay;
        [BoxGroup("Events")] public UnityEvent actionEvent;


        private Coroutine _engagedCoroutine;
        private bool _playerInTrigger;

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player_Main")) return;

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

            if (simpleEngage)
            {
                yield return engageAnim.SimpleAnim(player.AnimationController, player.transform, transform);
            }
            else
            {
                yield return engageAnim.PlayAnim(player.AnimationController, player.transform, transform);
            }


            float timeElapsed = 0;

            var uiManager = UIManager.Instance;
            uiManager.UpdateActionFillPos(transform.position, new Vector3(0, 2f, 0f));

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

                uiManager.UpdateActionFill(Mathf.Clamp01(timeElapsed / actionTriggerTime), actionInput);

                if (timeElapsed >= actionTriggerTime)
                {
                    player.AnimationController.CrossFade(actionAnimName, 0.2f, 1);
                    yield return new WaitForSeconds(actionDelay);
                    actionEvent.Invoke();
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