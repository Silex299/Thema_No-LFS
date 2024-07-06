using System;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class PlayerDirectionTrigger : MonoBehaviour
    {
        public MoveDirection moveDirection;
        public float threshold = 0;

        public UnityEvent entryAction;
        public UnityEvent exitAction;
        
        private  bool _playerIsInTrigger;
        private bool _calledEntry;
        private bool _calledExit;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = true;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = false;
                _calledEntry = false;
                _calledExit = false;
            }
        }


        private void Update()
        {
            if (_playerIsInTrigger)
            {
                bool result = moveDirection switch
                {
                    MoveDirection.Forward => Input.GetAxis("Horizontal") > threshold,
                    MoveDirection.Backward => Input.GetAxis("Horizontal") < threshold,
                    MoveDirection.Left => Input.GetAxis("Vertical") < threshold,
                    MoveDirection.Right => Input.GetAxis("Vertical") > threshold,
                    _ => false
                };
                
                if (result)
                {
                    if (_calledEntry) return;
                    
                    entryAction.Invoke();
                    _calledExit = false;
                    _calledEntry = true;
                }
                else
                {
                    if (_calledExit) return;
                    
                    exitAction.Invoke();
                    _calledEntry = false;
                    _calledExit = true;
                }
            }
        }


        [Serializable]
        public enum MoveDirection
        {
            Forward,Backward,Left,Right
        }
    }
}
