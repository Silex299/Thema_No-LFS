using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scene_Scripts.Underwater
{
    public class UnderwaterReactor : MonoBehaviour
    {

        [FoldoutGroup("Materials")] public Material reactorBarMaterial;
        [FoldoutGroup("Materials")] public ReactorBarMaterialProperty deactivatedMaterialProperty;
        [FoldoutGroup("Materials")] public ReactorBarMaterialProperty activatedMaterialProperty;
        [FoldoutGroup("Materials")] public ReactorBarMaterialProperty triggeredMaterialProperty;

        [FoldoutGroup("References")] public Animator reactorAnimator;
        [FoldoutGroup("References")] public Animator reactorWireAnimator;
        

        private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
        private static readonly int Frequency = Shader.PropertyToID("_Frequency");


        private void OnDisable()
        {
            deactivatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
        }


        [Button]
        public void ActivateReactor()
        {
            reactorAnimator.Play("Activated");
            reactorWireAnimator.Play("Activated");
            activatedMaterialProperty.ChangeMaterialProperty(reactorBarMaterial);
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
