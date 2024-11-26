using System;
using System.Linq;
using Managers.Checkpoints;
using Player_Scripts;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Thema;
using UnityEngine;

namespace Path_Scripts
{
    public class FollowCurvePathOverride : MonoBehaviour
    {
        [InfoBox("Automatically resets on checkpoint load")]
        
        public Transform[] waypoints;
        public float progressSpeed = 1;
        public float interpolationSpeed = 6;
        public float interpolationRotationSpeed = 12;
        public bool invertRotation;        
        public Vector3 rotationOffset;
        

        [SerializeField, OnValueChanged(nameof(Move))]
        private float pathProgress;

        private Player _player;
        private float _sectionDistance = 1;
        private bool _engageOverride;
        private Vector3 _playerForward;
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

        [Header("Debug")]
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
            
            
            Vector3 newPlayerForward = (invertRotation ? 1 : -1) * Vector3.Cross(direction, Vector3.up).normalized;
            newPlayerForward = Quaternion.Euler(rotationOffset) * newPlayerForward;
            
            if (direction.magnitude != 0)
            {
                _sectionDistance = direction.magnitude;
            }
            

#if UNITY_EDITOR
            mainPoint = playerPos;
            forwardPoint = playerPos + _playerForward;
            //if(!_player) return;
#endif
            playerPos.y = _player.transform.position.y;
            _playerForward = Vector3.Lerp(_playerForward, newPlayerForward, interpolationRotationSpeed * Time.deltaTime);
            
            _player.transform.position = Vector3.Lerp(_player.transform.position, playerPos, interpolationSpeed * Time.deltaTime);
            _player.transform.rotation = Quaternion.LookRotation(_playerForward);
        }
        private void InitialProgress()
        {
            var playerPos = PlayerMovementController.Instance.transform.position;


            if (waypoints.Length > 2)
            {
                 var points = waypoints.Select(point => point.position);
                 var closetPointIndex = ThemaVector.ClosestPoint(points, playerPos, out var closestPoint);

                 if (closetPointIndex == 0)
                 {
                     var point1 = waypoints[0].position;
                     var point2 = waypoints[1].position;

                     Vector3 point = ThemaVector.GetClosestPointToLine(point1, point2, playerPos);
                     float fraction = ThemaVector.PlannerDistance(point1, point) / ThemaVector.PlannerDistance(point1, point2);
                     pathProgress = fraction;

                 }
                 else if (closetPointIndex == (waypoints.Length - 1))
                 {
                     var point1 = waypoints[^2].position;
                     var point2 = waypoints[^1].position;

                     Vector3 point = ThemaVector.GetClosestPointToLine(point1, point2, playerPos);
                     float fraction = ThemaVector.PlannerDistance(point1, point) / ThemaVector.PlannerDistance(point1, point2);
                     pathProgress = (waypoints.Length - 2) + fraction;
                 }
                 else
                 {
                     Vector3 point1 = waypoints[closetPointIndex - 1].position;
                     Vector3 point2 = waypoints[closetPointIndex].position;
                     Vector3 point3 = waypoints[closetPointIndex + 1].position;

                     Vector3 closestPoint1 = ThemaVector.GetClosestPointToLine(point1, point2, playerPos);
                     Vector3 closestPoint2 = ThemaVector.GetClosestPointToLine(point2, point3, playerPos);

                     float distance1 = ThemaVector.PlannerDistance(closestPoint1, playerPos);
                     float distance2 = ThemaVector.PlannerDistance(closestPoint2, playerPos);

                     if (distance1 < distance2)
                     {
                         float fraction = ThemaVector.PlannerDistance(point2, closestPoint1) / ThemaVector.PlannerDistance(point1, point2);
                         pathProgress = closetPointIndex - fraction;
                     }
                     else
                     {
                         float fraction = ThemaVector.PlannerDistance(point2, closestPoint2) / ThemaVector.PlannerDistance(point2, point3);
                         pathProgress = closetPointIndex + fraction;
                     }

                 }
                 
            }
            else
            {
                var point1 = waypoints[0].position;
                var point2 = waypoints[1].position;
                Vector3 closestPoint = ThemaVector.GetClosestPointToLine(point1, point2, playerPos);
                float fraction = ThemaVector.PlannerDistance(point1, closestPoint) / ThemaVector.PlannerDistance(point1, point2);
                pathProgress = fraction;
            }

            _playerForward = PlayerMovementController.Instance.transform.forward;

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