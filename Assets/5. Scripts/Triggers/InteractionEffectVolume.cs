using System;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Triggers
{
    public class InteractionEffectVolume : BetterTriggerBase
    {
        [FoldoutGroup("Properties")] public GameObject dragPrefab;
        [FoldoutGroup("Properties")] public Vector3 offset;

        [FoldoutGroup("References")] public FollowCurve followCurve;

        private GameObject _spawnedEffect;
        private Transform PlayerTransform => PlayerMovementController.Instance.transform;


        private void Update()
        {
            if (playerInTrigger)
            {
                if (_spawnedEffect)
                {
                    _spawnedEffect.transform.position = PlayerTransform.TransformPoint(offset);
                }
                else
                {
                    SpawnEffect();
                }
            }
            else if (_spawnedEffect)
            {
                Destroy(_spawnedEffect);
            }
        }

        private void LateUpdate()
        {
            if (!playerInTrigger) return;

            followCurve.UpdatePosition(PlayerTransform);
        }

        private void SpawnEffect()
        {
            _spawnedEffect = Instantiate(dragPrefab);
        }
    }
}