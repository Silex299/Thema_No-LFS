using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sounds
{
    public class DirectionalVolumeFader : MonoBehaviour
    {
        
        
        [InfoBox("Fades out as players moves away from its position")]public Vector3 extents;
        public SoundSource[] soundSources;
        

        private float _fadeFraction;
        private int _objectCount;
        private Transform _target;

        private Transform Target
        {
            get
            {
                if (!_target) _target = PlayerMovementController.Instance.transform;
                return _target;
            }
        }
        
        #region Editor
#if UNITY_EDITOR


        [FoldoutGroup("Visual")] public bool fadeInDirection = true;
        [FoldoutGroup("Visual")] public Color color;
        [FoldoutGroup("Visual")]  public Mesh arrow;
        [FoldoutGroup("Visual")] public Vector3 scale;

        public void OnDrawGizmos()
        {
            
            Gizmos.color = color;
            Gizmos.DrawWireMesh(arrow, transform.position, Quaternion.LookRotation((fadeInDirection ? 1 : -1) * transform.forward, transform.up), scale);
            
            
            //draw a wire cube with the bounds and left center at the transform position
            Gizmos.color = new Color(1f, 1, 0.5f);
            Gizmos.DrawWireCube( transform.position + (transform.forward * extents.z), 2 * extents);
            
        }
#endif
        #endregion

        
        
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
      
        private void Start()
        {
            CreateBoxCollider();
            UpdateFadeFraction();
            UpdateSoundSources();
        }
        private void Update()
        {
            if (_objectCount <= 0) return;
            
            UpdateFadeFraction();
            UpdateSoundSources();
        }

        private void CreateBoxCollider()
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = transform.forward * extents.z;
            boxCollider.size = 2 * extents + Vector3.one;
            boxCollider.isTrigger = true;
        }
        private void UpdateFadeFraction()
        {
            Vector3 targetLocalPos = transform.InverseTransformPoint(Target.position);
            float distance = targetLocalPos.z;
            _fadeFraction = Mathf.Clamp01(1 - (distance / (2*extents.z)));
        }
        private void UpdateSoundSources()
        {
            foreach (var soundSource in soundSources)
            {
                soundSource.source.volume = soundSource.maximumVolume * _fadeFraction;
            }
        }
    }
}