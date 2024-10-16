using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Player_Scripts.Interactions
{

    public class SuctionEffect : MonoBehaviour
    {

        #region Editor
#if UNITY_EDITOR

        [FoldoutGroup("Visualisation")] public Mesh visualisationMesh;
        [FoldoutGroup("Visualisation")] public Vector3 visualisationScale;
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
        }
#endif

        #endregion


        [BoxGroup("Suction Params"), SerializeField] private float maximumSuction;
        [BoxGroup("Suction Params"), SerializeField] private float maximumThresoldDistance;
        [BoxGroup("Suction Params"), SerializeField] private float minimumThresoldDistance;

        private Coroutine _triggerExit;
        private bool _playerIsInTrigger;

        private Player_Scripts.PlayerMovementController _playerController;


        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag("Player_Main"))
            {
                _playerIsInTrigger = true;
                _playerController = Player_Scripts.PlayerMovementController.Instance;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;
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

        private void LateUpdate()
        {
            if (!enabled) return;
            if (_playerIsInTrigger)
            {
                Vector3 targetPosition = _playerController.transform.position;
                Vector3 pos = transform.position;

                Vector3 pullDirection = (pos - targetPosition).normalized;

                float distance = Vector3.Distance(targetPosition, pos);
                float fraction = Mathf.Clamp01((maximumSuction - distance) / (maximumSuction - minimumThresoldDistance));

                Vector3 pullVector = pullDirection * (fraction * maximumSuction * Time.deltaTime);

                _playerController.player.CController.Move(pullVector);

            }
        }

        
    }




}
