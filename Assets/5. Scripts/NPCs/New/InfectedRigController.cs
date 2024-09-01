using System;
using System.Collections;
using Mechanics.Npc;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NPCs.New
{
    public class InfectedRigController : MonoBehaviour
    {
        public Npc npc;
        public Rig aimRig;
        public Transform aimTarget;
        public Vector3 aimOffset;

        private bool _aimRigEnabled;
        private Transform _target;
        private Coroutine _aimRigCoroutine;


        private void Start()
        {
            npc.onAttack += OnAttack;
        }

        private void OnDisable()
        {
            npc.onAttack -= OnAttack;
        }

        private void Update()
        {
            if (!_aimRigEnabled) return;
            if (!_target) return;

            aimTarget.position = _target.position + aimOffset;
        }


        private Coroutine _attackResetCoroutine;
        
        private void OnAttack()
        {
            if (!_aimRigEnabled)
            {
                SetAimRig(npc.pathFinder.target);
            }
            else
            {
                if (_attackResetCoroutine != null)
                {
                    StopCoroutine(_attackResetCoroutine);
                }
                _attackResetCoroutine = StartCoroutine(AttackResetCoroutine());
            }
        }

        private IEnumerator AttackResetCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            ResetAimRig();
            _attackResetCoroutine = null;
        }


        private void SetAimRig(Transform target)
        {
            if (_aimRigEnabled) return;

            _aimRigEnabled = true;
            _target = target;
            
            if (_aimRigCoroutine != null)
            {
                StopCoroutine(_aimRigCoroutine);
            }
            _aimRigCoroutine = StartCoroutine(UpdateAimRigWeight(1));

        }

        private void ResetAimRig()
        {
            if(!_aimRigEnabled) return;
            
            _aimRigEnabled = false;
            _target = null;
            
            if (_aimRigCoroutine != null)
            {
                StopCoroutine(_aimRigCoroutine);
            }
            _aimRigCoroutine = StartCoroutine(UpdateAimRigWeight(0));
        }
        
        private IEnumerator UpdateAimRigWeight(float newWeight, float time = 0.5f)
        {
            float elapsedTime = 0;
            float currentWeight = aimRig.weight;

            while (elapsedTime < time)
            {
                aimRig.weight = Mathf.Lerp(currentWeight, newWeight, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _aimRigCoroutine = null;
        }
    }
}