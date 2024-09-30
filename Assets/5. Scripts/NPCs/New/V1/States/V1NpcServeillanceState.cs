using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;

namespace NPCs.New.V1.States
{
    public class V1NpcServeillanceState : V1NpcBaseState
    {
        #region Variables

        #region Editor Exposed Variables

        public Vector3[] serveillancePoints;
        public float serveillanceWaitTime;

        #endregion

        private float _speedMultiplier = 1;
        private int _currentWaypointIndex = 0;
        private Coroutine _speedCoroutine;
        private Coroutine _changeWaypointCoroutine;
        private bool _canRotate = true;


        private static readonly int StateIndex = Animator.StringToHash("StateIndex");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion


        #region Ediotr

#if UNITY_EDITOR

        [Button]
        public void SetServeillancePoints(Transform[] points)
        {
            serveillancePoints = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                serveillancePoints[i] = points[i].position;
            }
        }

        private void OnDrawGizmos()
        {
            if (serveillancePoints.Length == 0) return;
            foreach (var t in serveillancePoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(t, 0.25f);
            }
        }
#endif

        #endregion


        #region Overriden Methods

        public override void Enter(V1Npc npc)
        {
            SetInitialAnimatorState(npc);
            npc.navigationAgent.updateRotation = false;
            _currentWaypointIndex = 0;
        }

        public override void UpdateState(V1Npc npc)
        {
            var navAgent = npc.navigationAgent;
            navAgent.SetDestination(serveillancePoints[_currentWaypointIndex]);


            if (CheckForWaypointThreshold(npc))
            {
                _changeWaypointCoroutine ??= StartCoroutine(ChangeWaypoint(npc));
            }


            if (_canRotate) npc.Rotate(npc.transform.position + navAgent.desiredVelocity, _speedMultiplier * npc.rotationSpeed * Time.deltaTime);
            npc.animator.SetFloat(Speed, _speedMultiplier);
        }

        public override void Exit(V1Npc npc)
        {
            if (_changeWaypointCoroutine != null)
            {
                StopCoroutine(_changeWaypointCoroutine);
                _changeWaypointCoroutine = null;
            }

            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
                _speedCoroutine = null;
            }
        }

        #endregion

        #region Custom Methods

        private bool CheckForWaypointThreshold(V1Npc npc)
        {
            return ThemaVector.PlannerDistance(npc.transform.position, serveillancePoints[_currentWaypointIndex]) < npc.stopDistance;
        }

        private IEnumerator ChangeWaypoint(V1Npc npc)
        {
            _canRotate = false;

            if (serveillanceWaitTime != 0)
            {
                yield return Accelerate(npc, 0);
                //wait 
                yield return new WaitForSeconds(serveillanceWaitTime);
            }

            _currentWaypointIndex = (_currentWaypointIndex + 1) % serveillancePoints.Length;
            _canRotate = true;

            if (Mathf.Approximately(_speedMultiplier, 0))
            {
                yield return Accelerate(npc, 1);
            }

            _changeWaypointCoroutine = null;
        }


        private void SetInitialAnimatorState(V1Npc npc)
        {
            npc.animator.SetInteger(StateIndex, 0);
            npc.animator.SetBool(Attack, false);
            npc.animator.SetBool(PathBlocked, false);
            _speedMultiplier = 0;
            if (serveillancePoints.Length != 0)
            {
                _speedCoroutine = npc.StartCoroutine(Accelerate(npc, 1));
            }
        }

        private IEnumerator Accelerate(V1Npc npc, float targetSpeed)
        {
            float currentSpeed = _speedMultiplier;

            float timeElapsed = 0;

            while (timeElapsed <= npc.accelerationTime)
            {
                timeElapsed += Time.deltaTime;
                _speedMultiplier = Mathf.Lerp(currentSpeed, targetSpeed, timeElapsed / npc.accelerationTime);
                yield return null;
            }

            _speedMultiplier = targetSpeed;

            _speedCoroutine = null;
        }

        #endregion
    }
}