using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace NPCs.New
{
    public class InfectedRigController : MonoBehaviour
    {
        public Rig aimRig;
        public Transform aimTarget;
        public Vector3 aimOffset;

        private bool _aimRigEnabled;
        private Transform _target;
        private Coroutine _aimRigCoroutine;

        private void Update()
        {
            if (!_aimRigEnabled) return;
            if (!_target) return;

            aimTarget.position = _target.position + aimOffset;
        }

        public void SetAimRig(Transform target)
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
        public void ResetAimRig()
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