using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NPCs.New.Other
{
    public class V1NpcAimRigController : MonoBehaviour
    {
        public Rig aimRig;
        public Transform aimTarget;
        public Vector3 aimOffset;
        public float transitionTime;
        
        private bool _aimRigEnabled;
        private Transform _target;
        private Coroutine _aimRigCoroutine;
        
        private void Update()
        {
            if (!_aimRigEnabled) return;
            if (!_target) return;

            aimTarget.position = _target.position + aimOffset;
        }


        private Coroutine _attackResetCoroutine;
        public void Aim(Transform target)
        {
            if (!_aimRigEnabled)
            {
                SetAimRig(target);
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
            _aimRigCoroutine = StartCoroutine(UpdateAimRigWeight(1, transitionTime));

        }
        private void ResetAimRig()
        {
            if(!_aimRigEnabled) return;
            
            if (_aimRigCoroutine != null)
            {
                StopCoroutine(_aimRigCoroutine);
            }
            _aimRigCoroutine = StartCoroutine(UpdateAimRigWeight(0, transitionTime));
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

            aimRig.weight = newWeight;

            //SET AIR RIG ENABLED TO FALSE IF WEIGHT IS 0 only after the transition is done
            if (newWeight == 0)
            {
                _aimRigEnabled = false;
                _target = null;
            }
            
            _aimRigCoroutine = null;
        }
        
        public void Reset()
        {
            aimRig.weight = 0;
            _aimRigEnabled = false;
            _target = null;
        }
    }
}
