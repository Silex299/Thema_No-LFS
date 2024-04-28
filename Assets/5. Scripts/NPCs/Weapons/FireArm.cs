using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

// ReSharper disable once CheckNamespace
namespace NPCs.Weapons
{
    public sealed class FireArm : SerializedMonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private VisualEffect muzzle;
        [SerializeField] private LayerMask fireMask;
        [SerializeField] private float tracerLifetime;

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
            Debug.LogError("deom");
            if (Time.time < _lastFireTime + (1 / fireRate))
            {
                return;
            }

            Vector3 direction = (PlayerMovementController.Instance.transform.position + new Vector3(0, 0.5f, 0)) - transform.position;
            direction = direction.normalized;

            if (bulletPrefab)
            {
                Transform muzzleSocket = muzzle.transform;
                GameObject obj = Instantiate(bulletPrefab, muzzleSocket.position, muzzleSocket.rotation);
                StartCoroutine(MoveTracer(obj, direction));
            }

            if (muzzle)
            {
                muzzle.Play();
            }


            if (source)
            {
                source.PlayOneShot(firingSound);
            }

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
                    if (hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("Player"))
                    {
                        PlayerMovementController.Instance.player.Health.TakeDamage(101);
                    }

                    SpawnEffects(hit.collider.tag, hit.point);
                    yield break;
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

        public void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.OnDeath -= ResetWeapon;
        }

        public void AutomaticFire(bool autoFire)
        {
            if (autoFire == true)
            {
                PlayerMovementController.Instance.player.Health.OnDeath += ResetWeapon;
            }

            _autoFire = autoFire;
        }

        private void ResetWeapon()
        {
            AutomaticFire(false);
        }

        private void Update()
        {
            if (_autoFire)
            {
                FireBullet();
            }
        }
    }
}