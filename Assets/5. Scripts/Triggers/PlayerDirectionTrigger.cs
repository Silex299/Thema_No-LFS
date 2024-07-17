using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
                    MoveDirection.Right => Input.GetAxis("Horizontal") > threshold,
                    MoveDirection.Left => Input.GetAxis("Horizontal") < threshold,
                    MoveDirection.Down => Input.GetAxis("Vertical") < threshold,
                    MoveDirection.Up => Input.GetAxis("Vertical") > threshold,
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
        
    }
}


//IMPORTANT:: FIX MOVE DIRECTION IN PLAYER DIRECTION TRIGGER