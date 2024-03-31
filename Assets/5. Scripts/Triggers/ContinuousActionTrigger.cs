using System.Collections;
using UnityEngine;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Triggers
{

    public class ContinuousActionTrigger : MonoBehaviour
    {

        [SerializeField, BoxGroup("Trigger")] private string activationInput;
        [SerializeField, BoxGroup("Trigger")] private string actionInput;
        [SerializeField, BoxGroup("Trigger")] private float timeToTrigger;


        [SerializeField, BoxGroup("Animation")] private string engageActionName;
        [SerializeField, BoxGroup("Animation")] private string actionName;


        [SerializeField, BoxGroup("Action")] private float actionDelay;
        [SerializeField, BoxGroup("Action")] private Transform pointOfAction;
        [SerializeField, BoxGroup("Action")] private UnityEvent action;



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

                Invoke("Reset", 1f);
            }
            else
            {
                player.player.AnimationController.Update(Time.deltaTime);
            }
        }

        private void Reset()
        {
            _preview = false;
            Transform trans = player.transform;
            trans.position = _initialPlayerPos;
            trans.eulerAngles = _initialPlayerRot;

            player.PlayAnimation("Default", 1);
            player.player.AnimationController.Update(0);

            EditorApplication.update -= Preview;
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
                if (_actionTriggerTime == 0)
                {
                    _actionTriggerTime = Time.time;
                }


                if (_actionTriggerTime + timeToTrigger < Time.time)
                {
                    //Play Trigger;
                    StartCoroutine(Trigger());
                }

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
                        PlayerMovementController.Instance.DiablePlayerMovement(false);
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

                PlayerMovementController.Instance.DiablePlayerMovement(true);
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
                    PlayerMovementController.Instance.DiablePlayerMovement(false);
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

            if (_triggerd) yield return null;

            yield return new WaitForSeconds(actionDelay);

            _triggerd = true;

            PlayerMovementController.Instance.PlayAnimation(actionName, 1);

            yield return new WaitForSeconds(0.8f);

            action?.Invoke();

            //TODO: make the time delay dynamic if needed;
            yield return new WaitForSeconds(0.5f);

            ResetRotation();

            yield return new WaitForSeconds(1.8f);


            PlayerMovementController.Instance.DiablePlayerMovement(false);

        }


        #endregion 



    }
}
