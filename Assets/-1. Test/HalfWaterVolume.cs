using NP_Interactions;
using Player_Scripts;
using UnityEngine;

public class HalfWaterVolume : MonoBehaviour
{

    public GameObject splashPrefab;
    public GameObject dragPrefab;
    
    public Vector3 offset;
    
    private bool _playerInTrigger;
    private GameObject _spawnedEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            if (!_spawnedEffect)
            {
                _spawnedEffect = Instantiate(dragPrefab, transform, true);
            }
            _playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _playerInTrigger = false;
            if (_spawnedEffect)
            {
                Destroy(_spawnedEffect, 1);
            }
        }
    }

    private void Update()
    {
        if (_playerInTrigger)
        {
            if (_spawnedEffect)
            {
                var playerTransform = PlayerMovementController.Instance.transform;
                _spawnedEffect.transform.position = playerTransform.position + offset.z * playerTransform.forward + offset.y * playerTransform.up + offset.x * playerTransform.right; 
            }
        }
    }
    
    
}
