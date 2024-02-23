using UnityEditor;
using UnityEngine;

namespace Triggers
{
    public class DirectionCondition : TriggerCondition
    {
#if UNITY_EDITOR
        public Mesh visualisationMesh;
        public Vector3 visualisationScale;
#endif

        public override bool Condition(Collider other)
        {

            return Vector3.Angle(other.transform.forward, transform.forward) < 45;            

        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Gizmos.color = (Selection.activeGameObject == this.gameObject) ? Color.green : Color.yellow;
            var rot = transform.rotation;
            if (visualisationMesh)
            {
                Gizmos.DrawWireMesh(visualisationMesh, transform.position, rot, visualisationScale);
            }
            else
            {
                Debug.LogWarning("No Mesh to visualise");
            }
#endif
        }
    }
}