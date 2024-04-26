using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs.Weapons
{
    public class PowerUpWeapon : SerializedMonoBehaviour
    {
        [SerializeField] private float timeBetweenFire;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletDestroyTime;
        
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private Dictionary<string, GameObject> hitEffects;
        [SerializeField] private bool _enabled = true;

        

        private float _lastFireTime;
        private bool _traceBullet;
        

        public void Fire(Vector3 target)
        {
            //fire bullet at firRate
            if (!(Time.time - _lastFireTime > timeBetweenFire)) return;
            
            animator.Play("Power Up");
            StartCoroutine(TraceBullet(target));
            _lastFireTime = Time.time;
        }

        private IEnumerator TraceBullet(Vector3 target)
        {
            float timeElapsed = 0;
            
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).transform;

            while (timeElapsed < bulletDestroyTime)
            {
                if (Physics.Linecast(transform.position, bullet.position, out RaycastHit hit))
                {
                    //If hit collider tag is playerMain give 101 damage to player health
                    if (hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("Player"))
                    {
                        Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(101);
                    }
                    
                    print(hit.collider.name);
                    
                    SpawnEffects(hit.point, hit.collider.tag);
                    
                    Destroy(bullet.gameObject);
                    break;
                }
                
                
                Vector3 dir = (target + Vector3.up * 0.2f) - transform.position;
                //Move the bullet in dir direction with speed of bulletSpeed
                bullet.position += dir.normalized * (bulletSpeed * Time.deltaTime);
                
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(bullet.gameObject);
        }

        private void SpawnEffects(Vector3 point, string colliderTag)
        {
            //Instantiate the hit effect at the point of collision
            if (hitEffects.TryGetValue(colliderTag, out GameObject obj))
            {
                Instantiate(obj, point, Quaternion.identity);
            }
        }

        public void LookForPlayer(Transform target)
        {
            //look at the target, but looking speed at 3
            var transform1 = transform;
            transform.rotation = Quaternion.Slerp(transform1.rotation,
                Quaternion.LookRotation(target.position - transform1.position), 1f * Time.deltaTime);
        }
    }
}