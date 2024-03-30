using System.Collections;
using UnityEngine;
using Player_Scripts;
using Sirenix.OdinInspector;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Triggers
{

    public class ContinuousActionTrigger : MonoBehaviour
    {

        [SerializeField] private string activationInput;
        [SerializeField] private string actionInput;
        [SerializeField] private float timeToTrigger;
        [SerializeField] private float actionDelay;

        [SerializeField, Space(10)] private string engageActionName;
        [SerializeField] private string actionName;

        [SerializeField] private Transform pointOfAction;

        private Transform _target;

        private bool _playerIsInTrigger;
        private bool _movePlayer;
        private bool _playerEngaged;
        [SerializeField] private bool _triggerd;
        private Coroutine _resetTrigger;
        private float _actionTriggerTime;



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

        private IEnumerator ResetTrigger()
        {

            yield return new WaitForSeconds(0.1f);

            print("wtf");
            _playerIsInTrigger = false;
            _playerEngaged = false;
            _actionTriggerTime = 0;
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
                    Disengage();
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


        private Vector3 _initialPos;
        private Quaternion _initialRot;
        private float _timeElapsed;
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
                PlayerMovementController.Instance.PlayAnimation(engageActionName, 0.6f, 1);
            }

            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / 0.5f;

                _target.position = Vector3.Lerp(_initialPos, pointOfAction.position, fraction);

                _target.rotation = Quaternion.Slerp(_initialRot, pointOfAction.rotation, fraction);

                if (fraction >= 1)
                {
                    _movePlayer = false;
                    _playerEngaged = true;
                }

            }
        }



        private void Disengage()
        {
            if (!_movePlayer)
            {
                _movePlayer = true;
                _timeElapsed = 0;
                PlayerMovementController.Instance.PlayAnimation("Default", 0.2f, 1);
            }

            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / 0.5f;

                _target.position = Vector3.Lerp(pointOfAction.position, _initialPos, fraction);
                _target.rotation = Quaternion.Slerp(pointOfAction.rotation, _initialRot, fraction);

                if (fraction >= 1)
                {
                    _movePlayer = false;
                    _playerEngaged = false;
                    PlayerMovementController.Instance.DiablePlayerMovement(false);
                }

            }
        }

        private void ResetRotation()
        {
            if (!_movePlayer)
            {
                _timeElapsed = 0;
                _movePlayer = true;
            }
            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / 1f;

                _target.rotation = Quaternion.Slerp(pointOfAction.rotation, _initialRot, fraction);

                if (fraction >= 1)
                {
                    _movePlayer = false;
                }
            }
            
        }


        private IEnumerator Trigger()
        {

            if (_triggerd) yield return null;

            yield return new WaitForSeconds(actionDelay);

            _triggerd = true;

            PlayerMovementController.Instance.PlayAnimation(actionName, 1);

            yield return new WaitForSeconds(1.5f);


            ResetRotation();

            yield return new WaitForSeconds(1.8f);

            PlayerMovementController.Instance.DiablePlayerMovement(false);

        }





    }
}
