using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
    public class BatWeapon : WeaponBase
    {
        [BoxGroup("Weapon Action Params")] public float secondActionDelay = 0.5f;
        [BoxGroup("Weapon Action Params")] public float damageDistance = 1.7f;

        private float _lastAttackTime;

        public override void Fire()
        {
            base.Fire();
        }

        /// <summary>
        /// Called when bat attack animation is finished
        /// </summary>
        public void AttackCallback()
        {
            
            //return if last attack time is less than second action delay
            if (Time.time < _lastAttackTime + secondActionDelay)
            {
                return;
            }
            
            //find distance between player and npc
            float distance = Vector3.Distance(PlayerMovementController.Instance.transform.position, transform.position);
            //if distance is less than damage distance then deal damage

            if (distance < damageDistance)
            {
                PlayerMovementController.Instance.player.Health.TakeDamage(101);
            }
        }
    }
}