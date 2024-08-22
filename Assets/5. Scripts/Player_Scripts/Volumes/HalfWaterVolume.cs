using System;
using System.Collections;
using UnityEngine;

namespace Player_Scripts.Volumes
{
    public class HalfWaterVolume : MonoBehaviour
    {
        public bool continuousCheck;
        
        public GameObject splashEffect;
        public GameObject dragEffect;

        public Vector3 dragEffectOffset;
        public float dragRestrictedY;
        public float velocityThreshold;

        private GameObject _spawnedDragEffect;
        private Transform _playerTransform;
        
        private void OnTriggerEnter(Collider other)
        {
            if(continuousCheck)return;
            if (other.CompareTag("Player_Main"))
            {
                SpawnEffects(other.transform);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(continuousCheck)return;
            if (other.CompareTag("Player_Main"))
            {
                Destroy(_spawnedDragEffect);
                _spawnedDragEffect = null;
                _playerTransform = null;
            }
        }

        #region Contiunous Check
        
        private Coroutine _resetTrigger;
        private void OnTriggerStay(Collider other)
        {
            if (!continuousCheck) return;
            
            if (other.CompareTag("Player_Main"))
            {
                if (_resetTrigger == null)
                {
                    SpawnEffects(other.transform);
                }
                else
                {
                    StopCoroutine(_resetTrigger);
                }
                _resetTrigger = StartCoroutine(ResetPlayerTrigger());
            }
        }
        IEnumerator ResetPlayerTrigger()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(_spawnedDragEffect);
            _spawnedDragEffect = null;
            _playerTransform = null;
            _resetTrigger = null;
        }

        #endregion
        
        

        private void Update()
        {
            if (_spawnedDragEffect && _playerTransform)
            {
                var dragLocalPos = transform.InverseTransformPoint(_playerTransform.position);
                dragLocalPos.y = dragRestrictedY;
                dragLocalPos += _playerTransform.TransformDirection(dragEffectOffset);
                _spawnedDragEffect.transform.localPosition = dragLocalPos;
                
            }
        }

        private void SpawnEffects(Transform other)
        {
            
            var playerVelocity = other.GetComponent<PlayerVelocityCalculator>();
            
            print(playerVelocity.velocity.y);
            
            if(playerVelocity.velocity.y < -velocityThreshold)
            {
                var desiredPos = other.transform.position;
                desiredPos.y = (transform.transform.up * dragRestrictedY).y;
                Instantiate(splashEffect, desiredPos, Quaternion.identity);
                
            }
            _spawnedDragEffect ??= Instantiate(dragEffect, parent: transform);
            _playerTransform = other;
        }
        
    }
}
