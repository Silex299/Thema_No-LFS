using System;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NPCs.Weapons
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 _direction;
        private FireArm _weapon;
        
        [SerializeField] private float damagePercentage = 101;
        [SerializeField] private float selfDestructTime = 2f;

        private bool _fired;
        
        private void OnCollisionEnter(Collision other)
        {
            Debug.LogError(other.collider.name);
            
            if (other.collider.CompareTag("Player"))
            {
                PlayerMovementController.Instance.player.Health.TakeDamage(damagePercentage);
            }
            
            SpawnEffect(other.collider.tag, other.contacts[0].point);
        }

        public void BulletFire(FireArm weapon, Vector3 target)
        {
            _weapon = weapon;
            _direction = target - transform.position;
            _direction = _direction.normalized;
            _fired = true;
            
            Invoke(nameof(SelfDestroy), selfDestructTime);
        }
        
        private void SpawnEffect(string colliderTag, Vector3 contactPoint)
        {
            try
            {
                if(_weapon.hitEffects.TryGetValue(colliderTag, out GameObject obj))
                {
                    Instantiate(obj, contactPoint, Quaternion.identity);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            SelfDestroy();
        }

        private void SelfDestroy()
        {
            Destroy(this.gameObject);
        }
        
        private void Update()
        {
            if(!_fired) return;
            transform.position += _direction.normalized * (_weapon.bulletSpeed * Time.deltaTime);
        }

        
    }
}
