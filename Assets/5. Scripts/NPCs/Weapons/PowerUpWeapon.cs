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
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private Dictionary<string, GameObject> hitEffects;


        private bool _isEnabled = true;
        private float _lastFireTime;
        private bool _traceBullet;
        

        public void Fire(Vector3 target, float error)
        {
            if(!_isEnabled) return;
            
            //fire bullet at firRate
            if (!(Time.time - _lastFireTime > timeBetweenFire)) return;
            
            StartCoroutine(TraceBullet(target, error));
            _lastFireTime = Time.time;
        }

        private IEnumerator TraceBullet(Vector3 target, float error)
        {

            animator.Play("Power Up");

            var position = transform.position;
            Vector3 dir = target - position - Vector3.up * error;
            dir = dir.normalized;
            
            
            var bullet = Instantiate(bulletPrefab, position, Quaternion.identity).transform;
            
            float timeElapsed = 0;
            while (timeElapsed < bulletDestroyTime)
            {
                //Move the bullet in dir direction with speed of bulletSpeed
                bullet.position += dir * (bulletSpeed * Time.deltaTime);
                
                if (Physics.Linecast(transform.position, bullet.position, out RaycastHit hit, layerMask))
                {

                    //If hit collider tag is playerMain give 101 damage to player health
                    if (hit.collider.CompareTag("Player_Main") || hit.collider.CompareTag("Player"))
                    {
                        Player_Scripts.PlayerMovementController.Instance.player.Health.TakeDamage(101);
                    }
                    
                    
                    SpawnEffects(hit.point, hit.collider.tag);
                    
                    Debug.DrawLine(transform.position, hit.point, Color.green, 10f);
                    
                    Destroy(bullet.gameObject);
                    break;
                }
                
                
                
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

        public void SetEnabled(bool status)
        {
            _isEnabled = status;
        }

    }
}