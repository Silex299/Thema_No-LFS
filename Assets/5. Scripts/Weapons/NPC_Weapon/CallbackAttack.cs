using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons.NPC_Weapon
{
    //TODO: DON'T INHERIT IT FROM WEAPON BASE
    public class CallbackAttack : WeaponBase
    {
        public Transform overrideSocket;
        
        [BoxGroup("Weapon Action Params")] public float secondActionDelay = 0.5f;
        [BoxGroup("Weapon Action Params")] public float damageDistance = 1.7f;

        private float _lastAttackTime;
        private Coroutine _attackCoroutine;

        public void StartAttack()
        {
            if(_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);
            
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }

        public void EndAttack()
        {
            if (_attackCoroutine!=null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }

        }


        public IEnumerator AttackCoroutine()
        {
            while (true)
            {
                Vector3 socketPosition = overrideSocket ? overrideSocket.position : transform.position; float distance = Vector3.Distance(PlayerMovementController.Instance.transform.position, socketPosition);
                //if distance is less than damage distance then deal damage
                if (distance < damageDistance)
                {
                    PlayerMovementController.Instance.player.Health.TakeDamage(101);
                    _attackCoroutine = null;
                    yield break;
                }

                yield return null;
            }
            
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