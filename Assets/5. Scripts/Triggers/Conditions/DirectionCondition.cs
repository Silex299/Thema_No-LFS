using UnityEditor;
using UnityEngine;

namespace Triggers
{
    public class DirectionCondition : TriggerCondition
    {

        public Vector3 offset;
        
#if UNITY_EDITOR
        public Mesh visualisationMesh;
        public Vector3 visualisationScale = Vector3.one * 10f;
#endif

        public override bool Condition(Collider other)
        {

            return Vector3.Angle(other.transform.forward, transform.forward + offset) < 45;            

        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = (Selection.activeGameObject == this.gameObject) ? Color.green : Color.yellow;
            
            var rot1 = transform.forward + offset;
            var rot2 = Quaternion.LookRotation(rot1);
            if (visualisationMesh)
            {
                Gizmos.DrawWireMesh(visualisationMesh, transform.position, rot2, visualisationScale);
            }
            else
            {
                Debug.LogWarning("No Mesh to visualise");
            }
        }
#endif
    }
}