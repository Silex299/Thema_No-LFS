using System;
using System.Collections;
using Managers;
using Player_Scripts;
using UnityEngine;

namespace Scene_Scripts.Shift_1
{
    public class PushableLadder : MonoBehaviour
    {
        public Animator ladderAnimator;
        public float pushTime;
        
        public Transform pushPosition;
        public float transitionTime;
        
        public Vector3 actionFillOffset;
            
        private Player _player;
        private UIManager _uiManager;
        private bool _playerInTrigger;
        private Coroutine _resetTrigger;
        private bool _triggered;


        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {

                _playerInTrigger = true;
                
                if (_resetTrigger != null)
                {
                    StopCoroutine(_resetTrigger);
                }
                _resetTrigger = StartCoroutine(ResetTrigger());
            }
        }
        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _playerInTrigger = false;
        }


        private void Start()
        {
            _player = PlayerMovementController.Instance.player;
            _uiManager = UIManager.Instance;
        }


        private void Update()
        {
            if(!_playerInTrigger || _triggered) return;

            if (Input.GetButtonDown("e"))
            {
                print("Starting");
                StartCoroutine(EngagePlayer());
            }
        }


        private IEnumerator EngagePlayer()
        {
            
            Vector3 initPlayerPos = _player.transform.position;
            Quaternion initPlayerRot = _player.transform.rotation;

            #region Move Player and Engage

            EngageAction();
            float elapsedTime = 0;
            while (elapsedTime < transitionTime)
            {
                _player.transform.position = Vector3.Lerp(initPlayerPos, pushPosition.position, elapsedTime / transitionTime);
                _player.transform.rotation = Quaternion.Lerp(initPlayerRot, pushPosition.rotation, elapsedTime / transitionTime);
                
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            
            _player.transform.position = pushPosition.position;
            _player.transform.rotation = pushPosition.rotation;
            #endregion
            
            
            _uiManager.UpdateActionFillPos(transform.position, actionFillOffset);
            while (Input.GetButton("e"))
            {

                float pushedTime = 0;
                _uiManager.UpdateActionFill(0, "D");
                
                while (Input.GetButton("d"))
                {
                    pushedTime += Time.deltaTime;
                    _uiManager.UpdateActionFill(pushedTime/pushTime, "D");
                    
                    if (pushedTime >= pushTime)
                    {
                        PushAction();
                        _uiManager.UpdateActionFill(1, "D");
                        yield break;
                    }
                    yield return null;
                }
                

                yield return null;
            }
            
            
            _uiManager.UpdateActionFill(0, "D");
            DisEngageAction();
            
        }

        private void EngageAction()
        {
            _player.AnimationController.CrossFade("Side Single Push Start", 0.2f, 1);
        }
        
        private void DisEngageAction()
        {
            _player.AnimationController.CrossFade("Default", 0.2f, 1);
        }
        
        private void PushAction()
        {
            _player.AnimationController.Play("Side Single Push End", 1);
            ladderAnimator.Play("Ladder Move");
            _triggered = true;
        }
        
        
    }
}
