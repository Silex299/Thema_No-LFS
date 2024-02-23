using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Weapon
{
    public class Rifle : SerializedMonoBehaviour
    {


        [SerializeField, BoxGroup("Weapon Properties")] private float bulletDamage = 20f;
        [SerializeField, BoxGroup("Weapon Properties")] private float burstRate = 300;
        [SerializeField, BoxGroup("Weapon Properties")] private int BurstBullet = 4;
        [SerializeField, BoxGroup("Weapon Properties")] private int BurstInterval = 1;
        [SerializeField, BoxGroup("Weapon Properties")] private LayerMask layermask;

        [SerializeField, BoxGroup("Effects")] private VisualEffect muzzleFlash;
        [SerializeField, BoxGroup("Effects")] private AudioClip muzzleSoundEffect;
        [SerializeField, BoxGroup("Effects")] private AudioSource weaponAudio;
        [SerializeField, BoxGroup("Effects")] private Dictionary<string, ParticleSystem> hitEffects;


        private bool _fireWeapon;
        public bool Firing
        {
            get => _fireWeapon;
        }
        private Vector3 _target;

        private float _lastFireTime;
        private int _burstMag;

        private void Start()
        {
            _burstMag = BurstBullet;
        }

        private void Update()
        {
            if (!_fireWeapon) return;

            BurstFire();

        }

        private void BurstFire()
        {
            if (_burstMag == 0)
            {
                if (Time.time > BurstInterval + _lastFireTime)
                {
                    _burstMag = BurstBullet;
                }
            }

            if (_burstMag != 0)
            {
                if (Time.time > _lastFireTime + (60 / burstRate))
                {
                    _lastFireTime = Time.time;
                    Fire();
                    _burstMag--;
                }
            }
        }

        private void Fire()
        {
            if (muzzleFlash) muzzleFlash.Play();
            if (weaponAudio && muzzleSoundEffect) weaponAudio.PlayOneShot(muzzleSoundEffect);

            Vector3 muzzlePosition = muzzleFlash.transform.position;

            if (Physics.Raycast(muzzlePosition, (_target - muzzlePosition).normalized, out RaycastHit hit, Mathf.Infinity, layermask))
            {

                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Player_Main"))
                {
                    //Add damage 
                    Player_Scripts.PlayerController.Instance.Player.Health.TakeDamage(bulletDamage);
                }

                if (hitEffects.TryGetValue(hit.collider.tag, out ParticleSystem hitEffect))
                {
                    Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                else
                {
                    Instantiate(hitEffects["Default"], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }

            }

        }

        public void FireWeapon(bool fire, Vector3 target)
        {
            _target = target;
            if (fire == _fireWeapon) return;

            StartCoroutine(DelayedResponse(fire));
        }

        private IEnumerator DelayedResponse(bool fire)
        {
            yield return new WaitForSeconds(1.5f);
            _fireWeapon = fire;
            yield return null;
        }


    }
}