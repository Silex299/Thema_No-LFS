using System.Collections;
using UnityEngine;

namespace PostProcessing
{
    public class GlitchMaterialController : MonoBehaviour
    {
        public Material material;
        public GlitchMaterialSetting materialSetting;


        private static readonly int NoiseWidth = Shader.PropertyToID("_NoiseWidth");
        private static readonly int NoiseFrequency = Shader.PropertyToID("_NoiseFrequency");
        private static readonly int GlitchScale = Shader.PropertyToID("_GlitchScale");
        private static readonly int ScanlineStrength = Shader.PropertyToID("_ScanlineStrength");
        private Coroutine _glitchCoroutine;


        private void OnDisable()
        {
            material.SetFloat(NoiseWidth, 0);
            material.SetFloat(NoiseFrequency, 0);
            material.SetFloat(GlitchScale, 0);
            material.SetFloat(ScanlineStrength, 0);
        }

        public void SetGlitch(float time = 1)
        {
            if (_glitchCoroutine != null)
                StopCoroutine(_glitchCoroutine);
            _glitchCoroutine = StartCoroutine(ChangeGlitch(materialSetting, time));
        }
        public void ResetGlitch(float time = 1)
        {
            if (_glitchCoroutine != null)
                StopCoroutine(_glitchCoroutine);

            _glitchCoroutine = StartCoroutine(ChangeGlitch(new GlitchMaterialSetting(), time));
        }
        private IEnumerator ChangeGlitch(GlitchMaterialSetting setting, float time = 1)
        {
            var currentSetting = new GlitchMaterialSetting(
                material.GetFloat(NoiseWidth),
                material.GetFloat(NoiseFrequency),
                material.GetFloat(GlitchScale),
                material.GetFloat(ScanlineStrength));


            float timeElapsed = 0;
            while (timeElapsed < time)
            {
                timeElapsed += Time.deltaTime;
                var progress = timeElapsed / time;
                material.SetFloat(NoiseWidth, Mathf.Lerp(currentSetting.noiseWidth, setting.noiseWidth, progress));
                material.SetFloat(NoiseFrequency, Mathf.Lerp(currentSetting.noiseFrequency, setting.noiseFrequency, progress));
                material.SetFloat(GlitchScale, Mathf.Lerp(currentSetting.glitchScale, setting.glitchScale, progress));
                material.SetFloat(ScanlineStrength, Mathf.Lerp(currentSetting.scanlineStrength, setting.scanlineStrength, progress));

                yield return null;
            }

            material.SetFloat(NoiseWidth, setting.noiseWidth);
            material.SetFloat(NoiseFrequency, setting.noiseFrequency);
            material.SetFloat(GlitchScale, setting.glitchScale);
            material.SetFloat(ScanlineStrength, setting.scanlineStrength);
            _glitchCoroutine = null;
        }


        [System.Serializable]
        public class GlitchMaterialSetting
        {
            public float noiseWidth = 0;
            public float noiseFrequency = 0;
            public float glitchScale = 0;
            [Range(0,1)] public float scanlineStrength = 0;

            public GlitchMaterialSetting(float noiseWidth = 0, float noiseFrequency = 0, float glitchScale = 0, float scanlineStrength = 0)
            {
                this.noiseWidth = noiseWidth;
                this.noiseFrequency = noiseFrequency;
                this.glitchScale = glitchScale;
                this.scanlineStrength = scanlineStrength;
            }
        }
    }
}