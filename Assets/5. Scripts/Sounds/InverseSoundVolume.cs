using UnityEngine;

namespace Sounds
{
    public class InverseSoundVolume : SoundVolume
    {
        
        public bool createCollider = true;
        
        protected override void CreateBoxCollider()
        {
            if(!createCollider) return;
            
            //create a box collider with the bounds
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size + (Vector3.one * 2 * (fadeDistance+0.5f));
            boxCollider.isTrigger = true;
        }

        protected override void UpdateVolumeFraction()
        {
            Vector3 objectLocalPosition = transform.InverseTransformPoint(Target.position);
            Vector3 closestPoint = bounds.ClosestPoint(objectLocalPosition);
            Vector3 closedPointWorld = transform.TransformPoint(closestPoint);
            float distance = Vector3.Distance(Target.position, closedPointWorld);
            
            Debug.DrawLine(closedPointWorld, Target.position, Color.red);
            fadeFraction = Mathf.Clamp01(distance / fadeDistance);
        }
    }
}