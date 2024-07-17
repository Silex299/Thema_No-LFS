using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Thema_Camera;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Triggers
{
    public class ContinuousActionTrigger : MonoBehaviour
    {
        [InfoBox("User for things like pushing barriers and all")] [SerializeField, BoxGroup("Trigger")]
        private string activationInput;

        [SerializeField, BoxGroup("Trigger")] private string actionInput;
        [SerializeField, BoxGroup("Trigger")] private float engageTime;
        [SerializeField, BoxGroup("Trigger")] private float timeToTrigger;


        [SerializeField, BoxGroup("Animation")]
        private string engageActionName;

        [SerializeField, BoxGroup("Animation")]
        private string actionName;

        [SerializeField, BoxGroup("Animation")]
        private float actionAnimationTime = 1;

        [SerializeField, BoxGroup("UI")] private string actionString;
        [SerializeField, BoxGroup("UI")] private Image progressBar;
        [SerializeField, BoxGroup("UI")] private TextMeshProUGUI actionText;
        [SerializeField, BoxGroup("UI")] private Vector3 visualOffset;


        [SerializeField, BoxGroup("Action")] private Transform pointOfAction;

        [FormerlySerializedAs("maxiumActionCount")] [SerializeField, BoxGroup("Action")]
        private int maximumActionCount;

        [SerializeField, BoxGroup("Action")] private float actionDelay;

        [SerializeField, BoxGroup("Action"), Space(10)]
        private UnityEvent action;


        private bool _playerInTrigger;
        private int _actionCounter;
        private Coroutine _resetTrigger;
        private bool _engaged;


        #region Editor

#if UNITY_EDITOR


        //REMOVE ALL

        private bool _preview;
        private PlayerMovementController _player;
        private Vector3 _initialPlayerPos;
        private Vector3 _initialPlayerRot;

        [Button("Preview", ButtonSizes.Large), GUIColor(0.1f, 0.6f, 0f)]
        private void Preview()
        {
            if (!_preview)
            {
                EditorApplication.update += Preview;

                _player = FindObjectOfType<PlayerMovementController>();
                _player.player.AnimationController.Play(engageActionName, 1, 0);
                _preview = true;


                var tran = _player.transform;

                _initialPlayerPos = tran.position;
                _initialPlayerRot = tran.eulerAngles;

                tran.position = pointOfAction.position;
                tran.rotation = pointOfAction.rotation;

                Invoke("ResetPreview", 3f);
            }
            else
            {
                _player.player.AnimationController.Update(Time.deltaTime);
            }
        }

        [Button("Force Preview Stop", ButtonSizes.Large), GUIColor(1, 0.2f, 0.2f)]
        public void ResetPreview()
        {
            EditorApplication.update -= Preview;
            _preview = false;
            Transform trans = _player.transform;
            trans.position = _initialPlayerPos;
            trans.eulerAngles = _initialPlayerRot;

            _player.PlayAnimation("Default", 1);
            _player.player.AnimationController.Update(0);
        }


#endif

        #endregion

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;

            if (_resetTrigger != null)
            {
                StopCoroutine(_resetTrigger);
            }

            _resetTrigger = StartCoroutine(ResetTrigger());

            if (!_playerInTrigger) _playerInTrigger = true;
        }

        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _playerInTrigger = false;
            _resetTrigger = null;
        }


        private void Update()
        {
            if (!_playerInTrigger) return;

            if (_actionCounter >= maximumActionCount) return;

            if (Input.GetButton(activationInput))
            {
                if (!_engaged) StartCoroutine(EngageAction());
            }
        }


        private IEnumerator EngageAction()
        {
            _engaged = true;

            var movementController = PlayerMovementController.Instance;
            movementController.player.DisabledPlayerMovement = true;
            movementController.player.CController.enabled = false;

            Vector3 initialPos = movementController.player.transform.position;
            Quaternion initialRot = movementController.player.transform.rotation;


            float timeElapsed = 0;

            movementController.PlayAnimation(engageActionName, 0.2f, 1);
            while (timeElapsed < engageTime)
            {
                timeElapsed += Time.deltaTime;

                movementController.player.transform.position =
                    Vector3.Lerp(initialPos, pointOfAction.position, timeElapsed / engageTime);
                movementController.player.transform.rotation =
                    Quaternion.Lerp(initialRot, pointOfAction.rotation, timeElapsed / engageTime);

                yield return null;
            }


            timeElapsed = 0;

            while (Input.GetButton(activationInput))
            {
                movementController.player.transform.position = pointOfAction.position;
                movementController.player.transform.rotation = pointOfAction.rotation;


                if (Input.GetButton(actionInput))
                {
                    VisualUI(timeElapsed / timeToTrigger);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    timeElapsed = 0;
                }

                if (timeElapsed >= timeToTrigger)
                {
                    yield return Trigger();
                    break;
                }

                yield return null;
            }

            yield return DisEngage(initialPos, initialRot);
        }

        private IEnumerator Trigger()
        {
            _actionCounter++;
            PlayerMovementController.Instance.PlayAnimation(actionName, 0.2f, 1);

            yield return new WaitForSeconds(actionDelay);
            ResetVisualUI();

            action.Invoke();
            yield return new WaitForSeconds(actionAnimationTime - actionDelay);
        }

        private IEnumerator DisEngage(Vector3 initialPos, Quaternion initialRot)
        {
            _engaged = false;

            var movementController = PlayerMovementController.Instance;
            movementController.player.DisabledPlayerMovement = false;
            movementController.player.CController.enabled = true;
            movementController.PlayAnimation("Default", 0.2f, 1);
            
            float timeElapsed = 0;

            Vector3 initPos = movementController.player.transform.position;
            Quaternion initRot = movementController.player.transform.rotation;
            
            while (timeElapsed < 0.5f)
            {
                timeElapsed += Time.deltaTime;

                movementController.player.transform.position =
                    Vector3.Lerp(initPos, initialPos, timeElapsed / 0.5f);
                movementController.player.transform.rotation =
                    Quaternion.Lerp(initRot, initialRot, timeElapsed / 0.5f);

                yield return null;
            }
        }


        private void VisualUI(float fill)
        {
            var myCamera = CameraFollow.Instance.myCamera;

            var pos = myCamera.WorldToScreenPoint(transform.position + visualOffset);
            progressBar.rectTransform.position = pos;
            progressBar.fillAmount = fill;

            if (!progressBar.gameObject.activeInHierarchy)
            {
                progressBar.gameObject.SetActive(true);
                actionText.text = actionString;
            }
        }

        private void ResetVisualUI()
        {
            progressBar.gameObject.SetActive(false);
            progressBar.fillAmount = 0;
        }


        public void ResetTracker()
        {
            _actionCounter = 0;
        }
    }
}