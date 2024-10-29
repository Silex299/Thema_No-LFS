using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Triggers
{
    public class PlayerDirectionTrigger : BetterTriggerBase
    {
        public MoveDirection moveDirection;
        public float threshold = 0;
        public bool buffer;
        [ShowIf(nameof(buffer))]public float bufferThreshold;

        public UnityEvent entryAction;
        public UnityEvent exitAction;
        
        private bool _calledEntry;
        private bool _calledExit;
      
        protected override bool OnTriggerExitBool(Collider other)
        {
            if (!base.OnTriggerExitBool(other)) return true;
            
            _calledEntry = false;
            _calledExit = false;

            return true;
        }


        private void Update()
        {
            if (playerInTrigger)
            {

                if (buffer)
                {
                    float lower = threshold - bufferThreshold;
                    float upper = threshold + bufferThreshold;
                    
                    bool result1 = moveDirection switch
                    {
                        MoveDirection.Right => Input.GetAxis("Horizontal") > lower && Input.GetAxis("Horizontal") < upper,
                        MoveDirection.Left => Input.GetAxis("Horizontal") > lower && Input.GetAxis("Horizontal") < upper,
                        MoveDirection.Down => Input.GetAxis("Vertical") > lower && Input.GetAxis("Vertical") < upper,
                        MoveDirection.Up => Input.GetAxis("Vertical") > lower && Input.GetAxis("Vertical") < upper,
                        _ => false
                    };
                    
                    if (result1) return;
                }
                
                
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
                    print("Entry");
                    _calledExit = false;
                    _calledEntry = true;
                }
                else
                {
                    if (_calledExit) return;
                    
                    print("Exit");
                    exitAction.Invoke();
                    _calledEntry = false;
                    _calledExit = true;
                }
            }
        }
        
    }
}


//IMPORTANT:: FIX MOVE DIRECTION IN PLAYER DIRECTION TRIGGER