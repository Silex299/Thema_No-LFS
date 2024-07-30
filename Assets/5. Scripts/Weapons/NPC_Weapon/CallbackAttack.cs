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
        
        public virtual void StartAttack()
        {
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