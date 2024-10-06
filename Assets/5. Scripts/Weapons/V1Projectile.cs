using System.Collections.Generic;
using Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
    public class V1Projectile : SerializedMonoBehaviour
    {
        public Rigidbody rb;
        public float damageRadius = 1;
        public LayerMask damageLayers;

        public AudioSource source;
        public Dictionary<string, GameObject> hitEffects;
        public Dictionary<string, AudioClip> hitSounds;


        public bool hit;
        private static readonly Collider[] Colliders = new Collider[6];

        private void FixedUpdate()
        {
            if (!hit) transform.forward = rb.velocity.normalized;
        }
        private void OnCollisionEnter(Collision other)
        {
            //stop the projectile
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            DamageHealth(other.GetContact(0).point);

            var otherTag = other.gameObject.tag;
            print(other.gameObject.name);
            
            

            if (hit) return;
            
            hit = true;
            //HIT EFFECT
            if (hitEffects.TryGetValue(otherTag, out var hitEffect1))
            {
                Instantiate(hitEffect1, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal));
            }
            else
            {
                if (hitEffects.TryGetValue("Default", out var hitEffect2))
                {
                    Instantiate(hitEffect2, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal));
                }
            }
            //HIT SOUND
            if (hitSounds.TryGetValue(otherTag, out var hitSound1))
            {
                source.PlayOneShot(hitSound1);
            }
            else
            {
                if (hitSounds.TryGetValue("Default", out var hitSound2))
                {
                    source.PlayOneShot(hitSound2);
                }
            }
            
        }
        
        
        private void DamageHealth(Vector3 point)
        {
            int numColliders = Physics.OverlapSphereNonAlloc(point, damageRadius, Colliders, damageLayers);

            for (int i = 0; i < numColliders; i++)
            {
                if (Colliders[i].TryGetComponent<HealthBaseClass>(out var health))
                {
                    health.TakeDamage(101);
                }
            }
        }
        
        
    }
}