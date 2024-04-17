using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Player_Scripts;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Thema_Camera;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class OpenHatch : MonoBehaviour
    {

        [SerializeField, BoxGroup("Properties")] private Transform animationLocation;
        [SerializeField, BoxGroup("Properties")] private float movementSmothness;
        [SerializeField, BoxGroup("Properties")] private float rotationSmoothness;

        [SerializeField, BoxGroup("Properties")] private PlayableDirector director;
        [SerializeField, BoxGroup("Properties")] private float timeToTrigger;

        [SerializeField, BoxGroup("UI")] private Image visual;
        [SerializeField, BoxGroup("UI")] private Vector3 visualOffset;


        [SerializeField, BoxGroup("Actions")] private float actionDelay;
        [SerializeField, BoxGroup("Actions")] private UnityEvent action;


        private bool _isInTrigger;
        private bool _isEquiped;
        private bool _hatchOpened;
        private bool _isCallingAnimation;
        private bool _movePlayer;
        private float _triggerStartTime;
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _isInTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _isInTrigger = false;
            }
        }



        private void Update()
        {

            if (!_isInTrigger || _hatchOpened) return;


            if (Input.GetButtonDown("e"))
            {
                if (!_isEquiped && !_isCallingAnimation)
                {
                    if (CheckRotation())
                    {
                        StartCoroutine(PlayerEnterHatch());
                    }
                }
            }
            if (Input.GetButtonUp("e"))
            {
                StartCoroutine(PlayerExitHatch());
            }


            if (!_isEquiped || _isCallingAnimation) return;

            if (Input.GetButton("Vertical"))
            {
                if (_triggerStartTime == 0)
                {
                    _triggerStartTime = Time.time;
                }

                if (Time.time > _triggerStartTime + timeToTrigger)
                {
                    StartCoroutine(PlayerPullHatch());
                    StartCoroutine(Trigger());
                    return;
                }

                UpdateVisual();
            }
            else if(_triggerStartTime != 0)
            {
                _triggerStartTime = Mathf.MoveTowards(_triggerStartTime, Time.time, Time.deltaTime * 4);
                UpdateVisual();
            }


        }

        private void LateUpdate()
        {
            if (!_isInTrigger || _hatchOpened) return;

            if (_movePlayer) MovePlayer();
        }


        private void UpdateVisual()
        {
            var camera = CameraFollow.Instance.myCamera;
            var pos = camera.WorldToScreenPoint(transform.position + visualOffset);
            visual.rectTransform.position = pos;

            var fill = (Time.time - _triggerStartTime) / timeToTrigger;

            visual.fillAmount = fill;
            visual.gameObject.SetActive(fill > 0);
        }

        private void MovePlayer()
        {
            var target = PlayerMovementController.Instance.transform;

            target.position = Vector3.Lerp(target.position, animationLocation.position, Time.deltaTime * movementSmothness);
            target.rotation = Quaternion.Slerp(target.rotation, animationLocation.rotation, Time.deltaTime * rotationSmoothness);
        }

        private bool CheckRotation()
        {
            return Vector3.Angle(animationLocation.forward, PlayerMovementController.Instance.transform.forward) < 45;
        }


        #region PlayerAnimations

        private IEnumerator PlayerEnterHatch()
        {
            _isCallingAnimation = true;
            _movePlayer = true;
            _triggerStartTime = 0;

            PlayerMovementController.Instance.DisablePlayerMovement(true);
            PlayerMovementController.Instance.PlayAnimation("Start Hatch", 0.2f, 1);

            yield return new WaitForSeconds(1.15f);

            _isEquiped = true;
            _isCallingAnimation = false;
        }

        private IEnumerator PlayerExitHatch()
        {
            _isCallingAnimation = true;
            visual.fillAmount = 0;
            visual.gameObject.SetActive(false);
            PlayerMovementController.Instance.PlayAnimation("Default", 0.3f, 1);

            yield return new WaitForSeconds(0.3f);

            PlayerMovementController.Instance.DisablePlayerMovement(false);


            _isCallingAnimation = false;
            _isEquiped = false;
            _movePlayer = false;
        }

        private IEnumerator PlayerPullHatch()
        {
            _isCallingAnimation = true;
            _triggerStartTime = 0;
            visual.gameObject.SetActive(false);
            visual.fillAmount = 0;

            PlayerMovementController.Instance.PlayAnimation("Hatch Pull", 1);
            director.Play();

            yield return new WaitForSeconds(2.4f);

            PlayerMovementController.Instance.DisablePlayerMovement(false);

            _isCallingAnimation = false;
            _isEquiped = false;
            _movePlayer = false;
            _hatchOpened = true;

        }

        private IEnumerator Trigger()
        {
            yield return new WaitForSeconds(actionDelay);
            action?.Invoke();
        }


        #endregion
    }
}
