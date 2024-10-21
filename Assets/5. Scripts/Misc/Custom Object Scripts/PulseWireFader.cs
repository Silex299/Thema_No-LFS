using System;
using System.Collections;
using UnityEngine;

namespace Misc.Custom_Object_Scripts
{
    public class PulseWireFader : MonoBehaviour
    {


        public Material pulseWireMaterial;
        public float defaultSliderValue;
        public float targetSliderValue;
        public float transitionTime = 1;
        
        private static readonly int Slider = Shader.PropertyToID("_Slider");
        private Coroutine _glowCoroutine;
        private bool _isGlowing;


        private void OnDisable()
        {
            pulseWireMaterial.SetFloat(Slider, defaultSliderValue);
        }

        public void GlowWire(bool glow)
        {
            if(glow == _isGlowing) return;
            
            if(_glowCoroutine != null) StopCoroutine(_glowCoroutine);
            _glowCoroutine = StartCoroutine(Glow(glow));
        }
        IEnumerator Glow(bool glow)
        {
            _isGlowing = glow;
            
            float initGlow = pulseWireMaterial.GetFloat(Slider);
            float targetGlow = glow ? targetSliderValue : defaultSliderValue;
            
            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                pulseWireMaterial.SetFloat(Slider, Mathf.Lerp(initGlow, targetGlow, timeElapsed/transitionTime));
                yield return null;
            }
            
            pulseWireMaterial.SetFloat(Slider, targetGlow);

            _glowCoroutine = null;

        }


    }
}
