using System;
using NPCs.New.V1;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scene_Scripts.Shift_1
{
    public class CanineTargetLostHelper : MonoBehaviour
    {
        public V1Npc[] npcs;
        public V1NpcCanineTargetLostState[] sates;
        public int exitStateIndex = 1;
        public int lostStateIndex = 3;

        [FormerlySerializedAs("nearPos")] public Vector3 leftPos;
        [FormerlySerializedAs("farPos")] public Vector3 rightPos;

        private bool _active;
        private bool[] _isInLeft;
        private bool _isPlayerInLeft;

        public void OnDrawGizmos()
        {
            if(Application.isPlaying) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.TransformPoint(leftPos), 0.2f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.TransformPoint(rightPos), 0.2f);
        }


        private void Start()
        {
            leftPos = transform.TransformPoint(leftPos);
            rightPos = transform.TransformPoint(rightPos);
        }


        [Button]
        public void SetCanineActionPos(bool active)
        {
            _active = active;

            if (active)
            {
                _isInLeft = new bool[npcs.Length];
                
                for (int i = 0; i < npcs.Length; i++)
                {
                    
                    
                    //calculate the distance from the left and right position
                    float leftDistance = Vector3.Distance(npcs[i].transform.position, leftPos);
                    float rightDistance = Vector3.Distance(npcs[i].transform.position, rightPos);
                    
                    //if the left distance is less than the right distance, then the npc is in the left position
                    _isInLeft[i] = leftDistance < rightDistance;
                    print(leftDistance + "::::" + rightDistance);
                }

                CheckPlayerPositionState();
                UpdatePlayerPositionState();
            }
        }

        public void FixedUpdate()
        {
            if(!_active) return;

            if (CheckPlayerPositionState())
            {
                UpdatePlayerPositionState();
            }
        }

        private bool CheckPlayerPositionState()
        {
            var playerPos = PlayerMovementController.Instance.transform.position;
            //Check if player is near to left or right
            float leftDistance = Vector3.Distance(playerPos, leftPos);
            float rightDistance = Vector3.Distance(playerPos, rightPos);
            
            print(leftDistance + "::::" + rightDistance);
            
            bool isPlayerInLeft = leftDistance < rightDistance;
            
            if(isPlayerInLeft != _isPlayerInLeft)
            {
                _isPlayerInLeft = isPlayerInLeft;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void UpdatePlayerPositionState()
        {
            for (int i = 0; i < npcs.Length; i++)
            {
                if (_isInLeft[i] == _isPlayerInLeft)
                {
                    npcs[i].ChangeState(exitStateIndex);
                }
                else
                {
                    npcs[i].ChangeState(lostStateIndex);
                    sates[i].actionPosition = _isInLeft[i] ? leftPos : rightPos;
                }
            }
        }
        
    }
}