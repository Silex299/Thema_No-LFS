using System.Collections;
using UnityEngine;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;
using Thema_Camera;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Triggers
{

    public class ContinuousActionTrigger : MonoBehaviour
    {
        [InfoBox("User for things like pushing barriers and all")]

        [SerializeField, BoxGroup("Trigger")] private string activationInput;
        [SerializeField, BoxGroup("Trigger")] private string actionInput;
        [SerializeField, BoxGroup("Trigger")] private float timeToTrigger;


        [SerializeField, BoxGroup("Animation")] private string engageActionName;
        [SerializeField, BoxGroup("Animation")] private string actionName;

        [SerializeField, BoxGroup("UI")] private Image progressBar;
        [SerializeField, BoxGroup("UI")] private TextMeshProUGUI actionText;
        [SerializeField, BoxGroup("UI")] private Vector3 visualOffset;


        [SerializeField, BoxGroup("Action")] private Transform pointOfAction;
        [SerializeField, BoxGroup("Action")] private bool careRotation = true;
        [SerializeField, BoxGroup("Action")] private float actionDelay;
        [SerializeField, BoxGroup("Action")] private float[] timings = { 0.8f, 0.5f, 1.8f };
        [SerializeField, BoxGroup("Action"), Space(10)] private UnityEvent action;



        private Transform _target;

        private bool _playerIsInTrigger;
        private bool _movePlayer;
        private bool _playerEngaged;
        [SerializeField] private bool _triggerd;

        private float _actionTriggerTime;
        private Coroutine _resetTrigger;


        private Vector3 _initialPos;
        private Quaternion _initialRot;
        private float _timeElapsed;


        #region Editor

#if UNITY_EDITOR


        //TODO: REMOVE ALL

        private bool _preview;
        private PlayerMovementController player;
        private Vector3 _initialPlayerPos;
        private Vector3 _initialPlayerRot;

        [Button("Preview", ButtonSizes.Large), GUIColor(0.1f, 0.6f, 0f)]
        private void Preview()
        {
            if (!_preview)
            {
                EditorApplication.update += Preview;

                player = FindObjectOfType<PlayerMovementController>();
                player.player.AnimationController.Play(engageActionName, 1, 0);
                _preview = true;


                var tran = player.transform;

                _initialPlayerPos = tran.position;
                _initialPlayerRot = tran.eulerAngles;

                tran.position = pointOfAction.position;
                tran.rotation = pointOfAction.rotation;

                Invoke("ResetPreview", 3f);
            }
            else
            {
                player.player.AnimationController.Update(Time.deltaTime);
            }
        }

        [Button("Force Preview Stop", ButtonSizes.Large), GUIColor(1, 0.2f, 0.2f)]
        public void ResetPreview()
        {
            EditorApplication.update -= Preview;
            _preview = false;
            Transform trans = player.transform;
            trans.position = _initialPlayerPos;
            trans.eulerAngles = _initialPlayerRot;

            player.PlayAnimation("Default", 1);
            player.player.AnimationController.Update(0);

        }


#endif

        #endregion

        #region Built-in methods

        private void OnTriggerStay(Collider other)
        {
            if (_triggerd) return;
            if (other.CompareTag("Player_Main"))
            {
                print(other.tag);
                _playerIsInTrigger = true;

                if (_resetTrigger != null)
                {

                    StopCoroutine(_resetTrigger);
                }


                _resetTrigger = StartCoroutine(ResetTrigger());

            }

        }

        private void Update()
        {

            if (_triggerd) return;
            if (!_playerIsInTrigger) return;



            if (Input.GetButtonDown(activationInput))
            {
                //Engage player
                Engage();
            }

            if (Input.GetButtonUp(activationInput))
            {
                _actionTriggerTime = 0;
                Disengage();
            }


            if (!_playerEngaged || _movePlayer) return;



            if (Input.GetButton(actionInput))
            {
                VisualUI((Time.time - _actionTriggerTime) / timeToTrigger);

                if (_actionTriggerTime == 0)
                {
                    _actionTriggerTime = Time.time;
                }


                if (_actionTriggerTime + timeToTrigger < Time.time)
                {
                    //Play Trigger;
                    Debug.Log("I want to masterbater so bad");
                    _actionTriggerTime = Time.time;
                    StartCoroutine(Trigger());

                }

            }
            else
            {
                VisualUI(0);
            }

            if (Input.GetButtonUp(actionInput))
            {
                _actionTriggerTime = 0;
            }


        }

        private void LateUpdate()
        {
            if (!_triggerd)
            {
                if (_movePlayer && !_playerEngaged)
                {
                    Engage();
                }
                else if (_movePlayer && _playerEngaged)
                {
                    Disengage(() =>
                    {
                        PlayerMovementController.Instance.DisablePlayerMovement(false);
                    });
                }
            }
            else
            {
                if (_movePlayer)
                {
                    ResetRotation();
                }
            }

        }

        #endregion

        #region Custom Methods

        private void Engage()
        {
            if (!_movePlayer)
            {
                _movePlayer = true;
                _target = PlayerMovementController.Instance.transform;

                _initialPos = _target.position;
                _initialRot = _target.rotation;
                _timeElapsed = 0;

                PlayerMovementController.Instance.DisablePlayerMovement(true);
                PlayerMovementController.Instance.PlayAnimation(engageActionName, 0.4f, 1);
            }

            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / 0.4f;

                _target.position = Vector3.Lerp(_initialPos, pointOfAction.position, fraction);

                _target.rotation = Quaternion.Slerp(_initialRot, pointOfAction.rotation, fraction);

                if (fraction >= 1)
                {
                    _movePlayer = false;
                    _playerEngaged = true;
                }

            }
        }

        private void Disengage(Action action = null)
        {

            if (!careRotation)
            {
                _movePlayer = false;
                _playerEngaged = false;
                PlayerMovementController.Instance.PlayAnimation("Default", 0.3f, 1);
                PlayerMovementController.Instance.DisablePlayerMovement(false);
                return;
            }


            if (!_movePlayer)
            {
                _movePlayer = true;
                _timeElapsed = 0;
                _initialRot = _target.rotation;
                PlayerMovementController.Instance.PlayAnimation("Default", 1f, 1);
            }
            else
            {
                ResetRotation(() =>
                {
                    PlayerMovementController.Instance.DisablePlayerMovement(false);
                });

            }
        }


        private void ResetRotation(Action action = null)
        {
            if (!_movePlayer)
            {
                _movePlayer = true;
                _timeElapsed = 0;
                _initialRot = _target.rotation;
            }

            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / 0.5f;

                _target.rotation = Quaternion.Slerp(_initialRot, new Quaternion(0, 0, 0, 1), fraction);

                if (fraction >= 1)
                {
                    _movePlayer = false;
                    _playerEngaged = false;
                    action?.Invoke();
                }

            }
        }

        private IEnumerator ResetTrigger()
        {

            yield return new WaitForSeconds(0.1f);

            _playerIsInTrigger = false;
            _playerEngaged = false;
            _actionTriggerTime = 0;
        }


        private IEnumerator Trigger()
        {

            if (_triggerd)
            {
                yield break;
            }


            yield return new WaitForSeconds(actionDelay);

            _triggerd = true;
            PlayerMovementController.Instance.DisablePlayerMovement(true);
            PlayerMovementController.Instance.PlayAnimation(actionName, 1);

            yield return new WaitForSeconds(timings[0]);

            action?.Invoke();

            //TODO: make the time delay dynamic if needed;
            yield return new WaitForSeconds(timings[1]);

            if (careRotation)
            {
                ResetRotation();
            }

            yield return new WaitForSeconds(timings[2]);


            PlayerMovementController.Instance.DisablePlayerMovement(false);

        }



        private Coroutine UICoroutine;
        private void VisualUI(float fill)
        {
            var camera = CameraFollow.Instance.myCamera;

            var pos = camera.WorldToScreenPoint(transform.position + visualOffset);
            progressBar.rectTransform.position = pos;
            progressBar.fillAmount = fill;

            if (!progressBar.gameObject.activeInHierarchy)
            {
                progressBar.gameObject.SetActive(true);

                //TODO: CHANGE IT
                actionText.text = ">";
            }

            if (UICoroutine != null)
            {
                StopCoroutine(UICoroutine);
            }

            UICoroutine = StartCoroutine(ResetUI());


        }

        private IEnumerator ResetUI()
        {
            yield return new WaitForSeconds(0.1f);

            progressBar.gameObject.SetActive(false);

        }

        public void ReActivate()
        {
            _triggerd = false;
        }

        #endregion 



    }
}
