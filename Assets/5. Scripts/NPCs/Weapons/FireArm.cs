using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NPCs.Weapons
{
    public sealed class FireArm : SerializedMonoBehaviour
    {

        [SerializeField] private GameObject bulletPrefab;
        
        // ReSharper disable once CollectionNeverUpdated.Global
        [SerializeField] internal Dictionary<string, GameObject> hitEffects;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip firingSound;
        
        [SerializeField, Space(10)] internal float bulletSpeed;
        [SerializeField] private float fireRate = 60;

        private bool _autoFire;
        private float _lastFireTime;
        
        // ReSharper disable once MemberCanBePrivate.Global
        public void FireBullet()
        {
            if (Time.time < _lastFireTime + (1 / fireRate))
            {
                return;
            }
            
            var transform1 = transform;
            
            if (bulletPrefab)
            {
                //TODO: If you can change these costly commands
                Instantiate(bulletPrefab, transform.position, transform1.rotation).GetComponent<Bullet>().BulletFire(this, PlayerMovementController.Instance.transform.position + (Vector3.up));
            }

            if (source)
            {
                source.PlayOneShot(firingSound);
            }

            _lastFireTime = Time.time;
        }

        public void OnEnable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath += ResetWeapon;
        }
        public void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath -= ResetWeapon;
        }

        public void AutomaticFire(bool autoFire)
        {
            _autoFire = autoFire;
        }

        private void ResetWeapon()
        {
            AutomaticFire(false);
        }
        
        private void Update()
        {
            if(_autoFire)
            {
                FireBullet();
            }
        }
        
        
    }

}
