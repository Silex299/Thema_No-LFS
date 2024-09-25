using System;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Path_Scripts
{
    public class FollowCurvePathOverride : MonoBehaviour
    {
        public Transform[] waypoints;
        public float progressSpeed;
        public bool invertRotation;

        [SerializeField, OnValueChanged(nameof(Move))]
        private float pathProgress;
        private Player _player;
        public bool _engagedOverride;

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
        }
#endif
        #endregion

        private void OnEnable()
        {
            _player = PlayerMovementController.Instance.player;
        }

        private void Update()
        {
            if(!_engagedOverride) return;
            if (!_player) return;

            float input = (_player.UseHorizontal) ? Input.GetAxis("Horizontal") : Input.GetAxis("Vertical");
            pathProgress += input * pathProgress * Time.deltaTime;
            pathProgress = Mathf.Clamp(pathProgress, 0, waypoints.Length - 1);
            Move();
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
            playerPos.y = _player.transform.position.y;
#if UNITY_EDITOR
            mainPoint = playerPos;
            forwardPoint = playerPos + playerForward;
#endif
            _player.transform.position = playerPos;
            _player.transform.rotation = Quaternion.LookRotation(playerForward, Vector3.up);
        }
    }
}