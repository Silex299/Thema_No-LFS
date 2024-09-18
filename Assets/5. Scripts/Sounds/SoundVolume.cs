using System;
using Player_Scripts;
using Thema_Type;
using UnityEngine;

namespace Sounds
{
    public class SoundVolume : MonoBehaviour
    {
        
        public Bounds bounds;
        public float fadeDistance;
        public SoundSource[] soundSources;

        protected float fadeFraction;
        private int _objectCount;
        private Transform _target;

        protected Transform Target
        {
            get
            {
                if (!_target) _target = PlayerMovementController.Instance.transform;
                return _target;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _objectCount++;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _objectCount--;
            }
        }
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            //draw a wire cube with the bounds 
            Gizmos.color = new Color(0.1f, 1, 0.1f);
            Gizmos.DrawWireCube( transform.TransformPoint(bounds.center), 2* bounds.extents);
            
            //Draw another wire cube with the bounds + fade distance
            Gizmos.color = new Color(0.8f, 1, 0.7f);
            Gizmos.DrawWireCube(transform.TransformPoint(bounds.center), 2 * (bounds.extents + Vector3.one * fadeDistance));
            
            
        }
        
        protected void Start()
        {
            CreateBoxCollider();
            UpdateVolumeFraction();
            UpdateSoundSources();
        }

        protected void Update()
        {
            if (_objectCount > 0)
            {
                UpdateVolumeFraction();
                UpdateSoundSources();
            }
        }

        private void UpdateSoundSources()
        {
            foreach (var soundSource in soundSources)
            {
                soundSource.source.volume = soundSource.maximumVolume * fadeFraction;
            }
        }

        protected virtual void UpdateVolumeFraction()
        {
            Vector3 objectLocalPosition = transform.InverseTransformPoint(Target.position);
            Vector3 closestPoint = bounds.ClosestPoint(objectLocalPosition);
            Vector3 closedPointWorld = transform.TransformPoint(closestPoint);
            float distance = Vector3.Distance(Target.position, closedPointWorld);
            
            Debug.DrawLine(closedPointWorld, Target.position, Color.red);
            fadeFraction = 1 - Mathf.Clamp01(distance / fadeDistance);
        }
        
        protected virtual void CreateBoxCollider()
        {
            //create a box collider with the bounds
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size + (Vector3.one * 2 * fadeDistance);
            boxCollider.isTrigger = true;
            
        }
    }
}
