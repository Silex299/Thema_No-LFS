using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;


namespace NavMesh_NPCs
{
    public class NavMeshNpcController : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField]
        private Transform target;

        [BoxGroup("References")] public NavMeshAgent agent;
        [BoxGroup("References")] public Animator animator;


        [BoxGroup("Surveillance")] public float surveillanceThreshold;
        [BoxGroup("Surveillance")] public Vector3[] surveillancePoints;


        [BoxGroup("Movement")] public float walkSpeed;
        [BoxGroup("Movement")] public float runSpeed;
        [BoxGroup("Movement")] public float waitTime = 1f;
        [BoxGroup("Movement")] public float velocityThreshold;


#if UNITY_EDITOR

        [Button("Get Positions", ButtonSizes.Large), GUIColor(1, 0.3f, 0.1f)]
        public void GetPositions(Transform[] transforms)
        {
            surveillancePoints = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                surveillancePoints[i] = transforms[i].position;
            }
        }

#endif


        private bool _isJumping;
        private bool _unreachable;
        private int _currentSurveillancePoint;
        private bool _hasTarget;
        private Coroutine _changeSurveillancePointCoroutine;


        //REMOVE NOW
        public float currentVelocity;

        public Transform Target
        {
            set
            {
                target = value;
                if (target && !_hasTarget)
                {
                    _hasTarget = true;
                    agent.speed = runSpeed;
                }
                else if (!target && _hasTarget)
                {
                    _hasTarget = false;
                    agent.speed = walkSpeed;
                }
            }
            get => target;
        }

        public void Update()
        {
            if (target)
            {
                //some function
                agent.SetDestination(Target.position);
            }
            else
            {
                //some function
                if (PlannerDistance(transform.position, surveillancePoints[_currentSurveillancePoint]) <
                    surveillanceThreshold)
                {
                    _changeSurveillancePointCoroutine ??= StartCoroutine(ChangeSurveillancePoint(waitTime));
                }

                currentVelocity = agent.velocity.magnitude;
            }
        }

        private IEnumerator ChangeSurveillancePoint(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            _currentSurveillancePoint = (_currentSurveillancePoint + 1) % surveillancePoints.Length;
            agent.SetDestination(surveillancePoints[_currentSurveillancePoint]);
            _changeSurveillancePointCoroutine = null;
        }

        private static float PlannerDistance(Vector3 pos1, Vector3 pos2)
        {
            pos2.y = 0;
            pos1.y = 0;
            return Vector3.Distance(pos1, pos2);
        }


        [System.Serializable]
        public enum NpcStates
        {
            Surveillance,
            Chase,
            Attack,
            AfterAttack
        }
    }
}