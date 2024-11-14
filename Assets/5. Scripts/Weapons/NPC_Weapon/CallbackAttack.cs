using System;
using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons.NPC_Weapon
{
    //TODO: DON'T INHERIT IT FROM WEAPON BASE
    public class CallbackAttack : WeaponBase
    {
        public Transform overrideSocket;
        
        [BoxGroup("Weapon Action Params")] public float damageDistance = 1.7f;

        protected Coroutine attackCoroutine;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 socketPosition = overrideSocket ? overrideSocket.position : transform.position;
            Gizmos.DrawWireSphere(socketPosition, damageDistance);
        }

        private void OnEnable()
        {
            PlayerMovementController.Instance.player.Health.onDeath += EndAttack;
        }

        public virtual void StartAttack()
        {
            print("Staring Attack");
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }

        public virtual void EndAttack()
        {
            if (attackCoroutine!=null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

        }


        protected virtual IEnumerator AttackCoroutine()
        {
            while (true)
            {
                AttackCallback();
                yield return null;
            }
            
        }
        

        /// <summary>
        /// Called when bat attack animation is finished
        /// </summary>
        protected virtual void AttackCallback()
        {
            
            print("Staring Attack 111");
            Vector3 socketPosition = overrideSocket ? overrideSocket.position : transform.position;
            
            //find distance between player and npc
            float distance = Vector3.Distance(PlayerMovementController.Instance.transform.position, socketPosition);
            //if distance is less than damage distance then deal damage

            print(distance);
            
            if (distance < damageDistance)
            {
                PlayerMovementController.Instance.player.Health.TakeDamage(101);
            }
            
        }
        
        
    }
}