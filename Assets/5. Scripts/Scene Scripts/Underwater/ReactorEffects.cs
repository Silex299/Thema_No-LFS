using System;
using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts.Underwater
{
    public class ReactorEffects : MonoBehaviour
    {


        [FoldoutGroup("References")] public Material beamMaterial;
        [FoldoutGroup("References")] public ParticleSystem windParticle;
        
        [FoldoutGroup("Parameters")] public float transitionTime = 1f;


        private Coroutine _transitionCoroutine;
        private bool _isOn;
        private static readonly int FresnelSub = Shader.PropertyToID("_Fresnel_Sub");

        
        public void PlayEffect()
        {
            _isOn = true;
            windParticle.Play(true);
            if(_transitionCoroutine!=null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionBeam(0));
        }
        public void StopEffect()
        {
            _isOn = false;
            windParticle.Stop(true);
            if(_transitionCoroutine!=null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionBeam(-1));
        }
        private IEnumerator TransitionBeam(float targetFresnelSubValue = 0)
        {


            float initValue = beamMaterial.GetFloat(FresnelSub);
            float timeElapsed = 0;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                float value = Mathf.Lerp(initValue, targetFresnelSubValue, timeElapsed / transitionTime);
                beamMaterial.SetFloat(FresnelSub, value);
                yield return null;
            }

            beamMaterial.SetFloat(FresnelSub, targetFresnelSubValue);
            
            
            _transitionCoroutine = null;
        }
        private void OnDisable()
        {
            beamMaterial.SetFloat(FresnelSub, -1);
        }
        
        
        
        
        //Damage
        [FoldoutGroup("Damage")] public GameObject lightningBolt;
        [FoldoutGroup("Damage")] public Vector3 lightningBoltOffset;
        [FoldoutGroup("Damage")] public float killDelay = 1.5f;
        
        [FoldoutGroup("Damage")] public float nextEffectDelay;

        private float _lastFireTime = 0;

        private void OnTriggerStay(Collider other)
        {
            if(!_isOn) return;
            
            if (other.CompareTag("Player"))
            {
                if (Time.time - _lastFireTime < nextEffectDelay) return;
                
                if (!PlayerMovementController.Instance.player.Health.IsDead)
                {
                    //instantiate lightning bolt at player position
                    Instantiate(lightningBolt).transform.position = other.transform.position + lightningBoltOffset;
                    _lastFireTime = Time.time;
                    Invoke(nameof(KillPlayer), killDelay);
                }
            }
        }

        private void KillPlayer()
        {
            PlayerMovementController.Instance.player.Health.TakeDamage(101);
        }
    }
}
