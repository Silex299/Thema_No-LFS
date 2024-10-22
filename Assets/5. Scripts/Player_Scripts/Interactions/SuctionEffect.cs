using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player_Scripts.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class SuctionEffect : MonoBehaviour
    {
        #region Editor

#if UNITY_EDITOR

        [FoldoutGroup("Visualisation")] public Mesh visualisationMesh;
        [FoldoutGroup("Visualisation")] public Vector3 visualisationScale = new Vector3(20, 20, 20);
        [FoldoutGroup("Visualisation")] public Vector3 visualisationPos;

        private void OnDrawGizmos()
        {
            Gizmos.color = (Selection.activeGameObject == this.gameObject) ? Color.green : Color.yellow;
            var rot = transform.rotation;

            if (visualisationMesh)
            {
                Gizmos.DrawWireMesh(visualisationMesh, transform.position + visualisationPos, rot, visualisationScale);
            }
            else
            {
                Debug.LogWarning("No Mesh to visualise");
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minimumThresholdDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, maximumThresholdDistance);
        }
#endif

        #endregion


        [field: SerializeField] public bool Suck { private get; set; } = true;
        
        [BoxGroup("Suction Params"), SerializeField]
        private bool adjustRotation;
        [BoxGroup("Suction Params"), SerializeField]
        private float maximumSuction;
        
        [FormerlySerializedAs("minimumThresoldDistance")] [BoxGroup("Suction Params"), SerializeField]
        private float minimumThresholdDistance;

        [FormerlySerializedAs("maximumThresoldDistance")] [BoxGroup("Suction Params"), SerializeField]
        private float maximumThresholdDistance;

   
        private Coroutine _triggerExit;
        private Coroutine _suctionSpeedCoroutine;
        private bool _playerIsInTrigger;
        private float _currentSuction;

        private static PlayerMovementController PlayerController => PlayerMovementController.Instance;


        private void OnTriggerEnter(Collider other)
        {
            if (!Suck) return;
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!Suck) return;
            if (other.CompareTag("Player_Main"))
            {
                if (_triggerExit != null)
                {
                    StopCoroutine(_triggerExit);
                }

                _triggerExit = StartCoroutine(TriggerExit());
            }
        }

        private IEnumerator TriggerExit()
        {
            yield return new WaitForSeconds(0.4f);
            _playerIsInTrigger = false;
        }


        private void Start()
        {
            if(Suck) _currentSuction = maximumSuction;
        }

        private void LateUpdate()
        {
            if (!Suck) return;
            if (!_playerIsInTrigger) return;
            
            Vector3 targetPosition = PlayerController.transform.position;
            Vector3 pos = transform.position;

            Vector3 pullDirection = (pos - targetPosition).normalized;

            float distance = Vector3.Distance(targetPosition, pos);

            float fraction = 1;
            
            if (distance > minimumThresholdDistance)
            {
                fraction = Mathf.Clamp01((maximumThresholdDistance - distance) / (maximumThresholdDistance - minimumThresholdDistance));
            }
            

            Vector3 pullVector = pullDirection * (fraction * _currentSuction * Time.deltaTime);
            
            PlayerController.player.CController.Move(pullVector);
        }


        public void StopSuction(float time)
        {
            if(_suctionSpeedCoroutine!= null) StopCoroutine(_suctionSpeedCoroutine);
            
            _suctionSpeedCoroutine = StartCoroutine(ChangeSuckSpeed(0, time));
        }
        
        public void StartSuction(float time)
        {
            if(_suctionSpeedCoroutine!= null) StopCoroutine(_suctionSpeedCoroutine);
            
            Suck = true;
            _suctionSpeedCoroutine = StartCoroutine(ChangeSuckSpeed(maximumSuction, time));
        }
        
        private IEnumerator ChangeSuckSpeed(float targetSuction, float time)
        {
            float timeElapsed = 0;
            
            float initialSuction = _currentSuction;
            while (timeElapsed < time)
            {
                timeElapsed += Time.deltaTime;
                _currentSuction = Mathf.Lerp(initialSuction, targetSuction, timeElapsed / time);
                //Call an event with fraction like for increasing or decreasing volume of a sound source;
                yield return null;
            }
            
            _currentSuction = targetSuction;
            _suctionSpeedCoroutine = null;
            
            Suck = targetSuction != 0;
        }
        
    }
}