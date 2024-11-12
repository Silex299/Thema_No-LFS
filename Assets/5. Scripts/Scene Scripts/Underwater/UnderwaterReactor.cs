using System;
using System.Collections;
using Misc;
using Player_Scripts;
using Sirenix.OdinInspector;
using Thema_Camera;
using UnityEngine;

namespace Scene_Scripts.Underwater
{
    public class UnderwaterReactor : MonoBehaviour
    {

        [TabGroup("Materials", "Bar Material")] public Material reactorBarMaterial;
        [TabGroup("Materials", "Bar Material")] public ReactorBarMaterialProperty deactivatedMaterialProperty;
        [TabGroup("Materials", "Bar Material")] public ReactorBarMaterialProperty activatedMaterialProperty;
        [TabGroup("Materials", "Bar Material")] public ReactorBarMaterialProperty triggeredMaterialProperty;

        [TabGroup("Materials", "Wire Material")] public Material leftPulseWire;
        [TabGroup("Materials", "Wire Material")] public float leftDeactivatedValue;
        [TabGroup("Materials", "Wire Material")] public float leftActivatedValue;
        [TabGroup("Materials", "Wire Material"), Space(5)] public Material rightPulseWire;
        [TabGroup("Materials", "Wire Material")] public float rightDeactivatedValue;
        [TabGroup("Materials", "Wire Material")] public float rightActivatedValue;

        [TabGroup("Materials", "Fill Material")] public Material leftFillMaterial;
        [TabGroup("Materials", "Fill Material")] public Material rightFillMaterial;
        
        


        [FoldoutGroup("Camera Shake")] public CameraShakeEffector cameraShakeEffector;
        [FoldoutGroup("Camera Shake")] public float activatedShakeMultiplier = 1;
        [FoldoutGroup("Camera Shake")] public float triggeredShakeMultiplier = 3;
        
        [FoldoutGroup("References")] public Animator reactorAnimator;
        [FoldoutGroup("References")] public Animator reactorWireAnimator;
        

        private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
        private static readonly int Frequency = Shader.PropertyToID("_Frequency");

        private Coroutine _pulseMaterialCoroutine;
        private Coroutine _fillMaterialCoroutine;
        private static readonly int Slider = Shader.PropertyToID("_Slider");
        private static readonly int Erosion = Shader.PropertyToID("_Erosion");


        private void OnDisable()
        {
            deactivatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
            leftPulseWire.SetFloat(Slider, leftDeactivatedValue);
            rightPulseWire.SetFloat(Slider, rightDeactivatedValue);
            leftFillMaterial.SetFloat(Erosion, 1);
            rightFillMaterial.SetFloat(Erosion, 1);
        }


        [Button]
        public void ActivateReactor()
        {
            reactorAnimator.Play("Activated");
            reactorWireAnimator.Play("Activated");
            activatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
            cameraShakeEffector.TransitionCameraShake(activatedShakeMultiplier, 2f);
            
        }


        public void TriggerReactor(bool leftTrigger)
        {
            if (leftTrigger)
            {
                if(_pulseMaterialCoroutine!= null) StopCoroutine(_pulseMaterialCoroutine);
                _pulseMaterialCoroutine = StartCoroutine(ChangePulseMaterial(leftPulseWire, leftActivatedValue));

                if(_fillMaterialCoroutine!= null) StopCoroutine(_fillMaterialCoroutine);
                _fillMaterialCoroutine = StartCoroutine(ChangeFillMaterial(leftFillMaterial, 0.133f));
            }
            else
            {
                if(_pulseMaterialCoroutine!= null) StopCoroutine(_pulseMaterialCoroutine);
                _pulseMaterialCoroutine = StartCoroutine(ChangePulseMaterial(rightPulseWire, rightActivatedValue));

                if(_fillMaterialCoroutine!= null) StopCoroutine(_fillMaterialCoroutine);
                _fillMaterialCoroutine = StartCoroutine(ChangeFillMaterial(rightFillMaterial, 0.133f));
            }
            
        }

        /// <summary>
        /// Called when only one switch is triggered
        /// </summary>
        public void TriggerIndependentEffects()
        {
            triggeredMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
            cameraShakeEffector.TransitionCameraShake(triggeredShakeMultiplier, 2f);
        }
        
        /// <summary>
        /// Called when both switches are off
        /// </summary>
        public void UnTriggerIndependentEffects()
        {
            activatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
            cameraShakeEffector.TransitionCameraShake(activatedShakeMultiplier, 2f);
        }
        
        public void UnTriggerReactor(bool leftTrigger)
        {
            if (leftTrigger)
            {
                if(_pulseMaterialCoroutine!= null) StopCoroutine(_pulseMaterialCoroutine);
                _pulseMaterialCoroutine = StartCoroutine(ChangePulseMaterial(leftPulseWire, leftDeactivatedValue));
                
                if(_fillMaterialCoroutine!= null) StopCoroutine(_fillMaterialCoroutine);
                _fillMaterialCoroutine = StartCoroutine(ChangeFillMaterial(leftFillMaterial, 1f));
            }
            else
            {
                if(_pulseMaterialCoroutine!= null) StopCoroutine(_pulseMaterialCoroutine);
                _pulseMaterialCoroutine = StartCoroutine(ChangePulseMaterial(rightPulseWire, rightDeactivatedValue));
                
                if(_fillMaterialCoroutine!= null) StopCoroutine(_fillMaterialCoroutine);
                _fillMaterialCoroutine = StartCoroutine(ChangeFillMaterial(rightFillMaterial, 1f));
            }
        }


        public void Exit()
        {
            PlayerMovementController.Instance.player.DisabledPlayerMovement = true;
            CutsceneManager.Instance.PlayClip(0);
        }
        
        
        private IEnumerator ChangePulseMaterial(Material material, float finalValue)
        {

            float timeElapsed = 0;
            float initValue = material.GetFloat(Slider);
            
            while (timeElapsed< 2)
            {
                timeElapsed += Time.deltaTime;
                float value = Mathf.Lerp(initValue, finalValue, timeElapsed/2);
                material.SetFloat(Slider, value);
                yield return null;
            }
            
            _pulseMaterialCoroutine = null;
        }

        private IEnumerator ChangeFillMaterial(Material material, float finalValue)
        {
            float timeElapsed = 0;
            float initValue = material.GetFloat(Erosion);
            
            while (timeElapsed< 0.4f)
            {
                timeElapsed += Time.deltaTime;
                float value = Mathf.Lerp(initValue, finalValue, timeElapsed/ 0.4f);
                material.SetFloat(Erosion, value);
                yield return null;
            }
            
            _pulseMaterialCoroutine = null;
        }
        
        public void Reset()
        {
            //Stop all coroutines
            
            //Reset animator
            reactorAnimator.Play("Default");
            reactorWireAnimator.Play("Default");
            
            //Turn off camera shake
            cameraShakeEffector.TransitionCameraShake(0, 0.1f);
            
            
            //Reset fill materials
            leftFillMaterial.SetFloat(Erosion, 1f);
            rightFillMaterial.SetFloat(Erosion, 1f);
            
            //Reset Wire Materials
            leftPulseWire.SetFloat(Slider, leftDeactivatedValue);
            rightPulseWire.SetFloat(Slider, rightDeactivatedValue);
            
            //Reset bar material
            deactivatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
        }

        [System.Serializable]
        public class ReactorBarMaterialProperty
        {
            public float amplitude;
            public float frequency;
            
            public void ChangeMaterialProperty(Material material)
            {
                material.SetFloat(Amplitude, amplitude);
                material.SetFloat(Frequency, frequency);
            }
        }


        
    }
}
