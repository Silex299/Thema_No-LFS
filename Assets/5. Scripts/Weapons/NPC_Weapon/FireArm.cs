using System;
using System.Collections;
using System.Collections.Generic;
using NPCs.New;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapons.NPC_Weapon
{
    public sealed class FireArm : MonoBehaviour
    {
        [BoxGroup("Fire Property")] public Npc npc;
        
        [SerializeField, BoxGroup("References")] private GameObject bulletPrefab;
        [SerializeField, BoxGroup("References")] private ParticleSystem muzzle;
        [SerializeField, BoxGroup("References")] private AudioSource source;
        
        [SerializeField, BoxGroup("Fire Property")] private float damage = 101;
        [SerializeField, BoxGroup("Fire Property")] private bool canFire = true;
        [SerializeField, BoxGroup("Fire Property")] private float tracerLifetime;
        [SerializeField, BoxGroup("Fire Property")] internal float bulletSpeed;
        [SerializeField, BoxGroup("Fire Property")] private float fireRate = 60;
        [SerializeField, BoxGroup("Fire Property")] private LayerMask fireMask;

        
        [SerializeField, BoxGroup("Effects")] private AudioClip firingSound;
        [SerializeField, BoxGroup("Effects")] internal Dictionary<string, GameObject> hitEffects;


        private bool _autoFire;
        private float _lastFireTime;
        
        
        public bool CanFire
        {
            get => canFire;
            set => canFire = value;
        }


        private void Start()
        {
            npc.onAttack += Fire;
        }
        private void OnDisable()
        {
            npc.onAttack -= Fire;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Fire()
        {
            if(!canFire) return;

            if (_lastFireTime == 0)
            {
                _lastFireTime = Time.time + 0.2f;
            }
            
            if (Time.time < _lastFireTime + (1 / fireRate))
            {
                return;
            }

            Vector3 direction = (PlayerMovementController.Instance.transform.position + new Vector3(0, 1f, 0)) - muzzle.transform.position;
            direction = direction.normalized;

            if (bulletPrefab)
            {
                Transform muzzleSocket = muzzle.transform;
                GameObject obj = Instantiate(bulletPrefab, muzzleSocket.position, muzzleSocket.rotation);
                StartCoroutine(MoveTracer(obj, direction));
            }
            
            muzzle.Play();
            source.PlayOneShot(firingSound);

            _lastFireTime = Time.time;
        }
        IEnumerator MoveTracer(GameObject tracer, Vector3 direction)
        {
            float startTime = Time.time;

            float time = Time.time;

            while (Time.time - startTime < tracerLifetime)
            {
                float deltaTime = Time.time - time;
                time = Time.time;
                tracer.transform.position += direction * (bulletSpeed * deltaTime);
                Transform muzzleSocket = muzzle.transform;

                if (Physics.Linecast(muzzleSocket.position, tracer.transform.position, out RaycastHit hit, fireMask))
                {
                    print(hit.collider.gameObject.name);
                    if (hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("Player"))
                    {
                        PlayerMovementController.Instance.player.Health.TakeDamage(damage);
                    }

                    SpawnEffects(hit.collider.tag, hit.point);
                    break;
                }

                yield return null;
            }

            Destroy(tracer);
        }
        private void SpawnEffects(string hitTag, Vector3 hitPoint)
        {
            try
            {
                if (hitEffects.TryGetValue(hitTag, out GameObject obj))
                {
                    Instantiate(obj, hitPoint, Quaternion.identity);
                }
            }
            catch 
            {
                // ignored
            }
        }

    }
}