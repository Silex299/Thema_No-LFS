using Player_Scripts;
using UnityEngine;

public class InteractionEffectVolume : MonoBehaviour
{
    public GameObject splashPrefab;
    public GameObject dragPrefab;

    public float playerVelocityThreshold = -2f;
    public Vector3 offset;
    public bool varyingTerrain;
    public float surfaceHeight = 0.7f;

    public float destroyEffectDelay = 1f;
    private bool _playerInTrigger;
    private GameObject _spawnedEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            if (_playerInTrigger) return;

            if (splashPrefab)
            {
                if (PlayerVelocityCalculator.Instance.velocity.y < playerVelocityThreshold)
                {
                    GameObject splash = Instantiate(splashPrefab, transform, true);
                    var playerTransform = PlayerMovementController.Instance.transform;
                    var newPos = playerTransform.position;
                    newPos.y = surfaceHeight;
                    splash.transform.position = newPos;
                    
                }
            }

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
                Destroy(_spawnedEffect, destroyEffectDelay);
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
                Vector3 playerPos = playerTransform.position;
                if (!varyingTerrain)
                {
                    playerPos.y = surfaceHeight;
                }
                _spawnedEffect.transform.position = playerPos + offset.z * playerTransform.forward +
                                                    offset.y * playerTransform.up + offset.x * playerTransform.right;
            }
        }
    }
}