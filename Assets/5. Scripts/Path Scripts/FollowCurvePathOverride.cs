using System;
using Managers.Checkpoints;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace Path_Scripts
{
    public class FollowCurvePathOverride : MonoBehaviour
    {
        [InfoBox("Automatically resets on checkpoint load")]
        
        public Transform[] waypoints;
        public float progressSpeed = 1;
        public float interpolationSpeed = 6;
        public bool invertRotation;        
        public Vector3 rotationOffset;
        

        [SerializeField, OnValueChanged(nameof(Move))]
        private float pathProgress;

        private Player _player;
        private float _sectionDistance = 1;
        private bool _engageOverride;
        public bool EngageOverride
        {
            get=>_engageOverride;
            set
            {
                _engageOverride = value;
                if (value)
                {
                    InitialProgress();
                }
            }
        }

        #region Editor

#if UNITY_EDITOR

        public Vector3 forwardPoint;
        public Vector3 mainPoint;

        private void OnDrawGizmos()
        {
            //DRAW WHITE WIRE SPHERE AT FORWARD POINT
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(forwardPoint, 0.5f);
            //RED at path point
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(mainPoint, 0.5f);
            
            //draw lines for waypoints
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            
        }
#endif

        #endregion

        private void OnEnable()
        {
            _player = PlayerMovementController.Instance.player;
            CheckpointManager.Instance.onCheckpointLoad += Reset;
        }
        private void OnDisable()
        {
            CheckpointManager.Instance.onCheckpointLoad -= Reset;
            Reset(0);
        }

        private void LateUpdate()
        {
            if (!EngageOverride) return;
            if (_player.OverrideFlags) return;
            
            MoveProgress();
            Move();
        }
        
        private void MoveProgress()
        {
            float input = (_player.UseHorizontal) ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");
            pathProgress += (input * progressSpeed * Time.deltaTime) / _sectionDistance;
            pathProgress = Mathf.Clamp(pathProgress, 0, waypoints.Length - 1);
        }
        private void Move()
        {
            int lowerRoundOff = Mathf.FloorToInt(pathProgress);
            int higherRoundOff = Mathf.CeilToInt(pathProgress);
            float reminder = pathProgress - lowerRoundOff;

            if (lowerRoundOff == higherRoundOff) return;

            Vector3 direction = waypoints[higherRoundOff].position - waypoints[lowerRoundOff].position;
            Vector3 playerPos = waypoints[lowerRoundOff].position + direction * reminder;
            Vector3 playerForward = (invertRotation ? 1 : -1) * Vector3.Cross(direction, Vector3.up).normalized;
            playerForward = Quaternion.Euler(rotationOffset) * playerForward;
            
            if (direction.magnitude != 0)
            {
                _sectionDistance = direction.magnitude;
            }
            

#if UNITY_EDITOR
            mainPoint = playerPos;
            forwardPoint = playerPos + playerForward;
            //if(!_player) return;
#endif
            playerPos.y = _player.transform.position.y;
            _player.transform.position = Vector3.Lerp(_player.transform.position, playerPos, interpolationSpeed * Time.deltaTime);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, Quaternion.LookRotation(playerForward, Vector3.up), interpolationSpeed * Time.deltaTime);
        }
        private void InitialProgress()
        {
            float startDistance = ThemaVector.PlannerDistance(waypoints[0].position, _player.transform.position);
            float endDistance = ThemaVector.PlannerDistance(waypoints[^1].position, _player.transform.position);
            pathProgress = startDistance < endDistance ? 0 : waypoints.Length - 1;
        }
        public void Engage()
        {
            if (_engageOverride) return;
            _engageOverride = true;
            
            //find the closest point to the player
            float minDistance = Mathf.Infinity;
            int index = 0;
            for (int i = 0; i < waypoints.Length; i++)
            {
                float distance = ThemaVector.PlannerDistance(waypoints[i].position, _player.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
            pathProgress = index;

        }
        private void Reset(int index)
        {
            _engageOverride = false;
        }
    }
}