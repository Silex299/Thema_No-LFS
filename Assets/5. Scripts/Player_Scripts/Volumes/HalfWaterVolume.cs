using UnityEngine;

namespace Player_Scripts.Volumes
{
    public class HalfWaterVolume : MonoBehaviour
    {
        public GameObject splashEffect;
        public GameObject dragEffect;

        public Vector3 dragEffectOffset;
        public float dragRestrictedY;
        public float velocityThreshold;

        private GameObject _spawnedDragEffect;
        private Transform _playerTransform;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                var playerVelocity = other.GetComponent<PlayerVelocityCalculator>();
                if(playerVelocity.velocity.y < -velocityThreshold)
                {
                    Instantiate(splashEffect, other.transform.position, Quaternion.identity, transform);
                }
                _spawnedDragEffect ??= Instantiate(dragEffect);
                _playerTransform = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                Destroy(_spawnedDragEffect);
                _spawnedDragEffect = null;
                _playerTransform = null;
            }
        }

        private void Update()
        {
            if (_spawnedDragEffect && _playerTransform)
            {
                var dragLocalPos = transform.InverseTransformPoint(_playerTransform.position);
                dragLocalPos.y = dragRestrictedY;
                dragLocalPos += dragEffectOffset;
                _spawnedDragEffect.transform.localPosition = dragLocalPos;
                
            }
        }
    }
}
