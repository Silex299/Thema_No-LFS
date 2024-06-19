using System;
using System.Collections;
using System.Linq;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class DimensionShifter : MonoBehaviour
    {
        public UniversalRendererData[] rendererData;

        [BoxGroup("Dimension Shift")] public GameObject alternateDimensionSpecific;
        [BoxGroup("Dimension Shift")] public GameObject realDimensionSpecific;

        public float alternateDimensionDamageRate = 5f;

        private Coroutine _glitchCoroutine;
        private static readonly int NoiseAmount = Shader.PropertyToID("_NoiseAmount");
        private static readonly int ScanlineStrength = Shader.PropertyToID("_scanlineStregth");

        private bool realDimension = true;

        private void Start()
        {
            foreach (var data in rendererData)
            {
                data.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault()?.SetActive(false);
            }
        }


        private void OnDisable()
        {
            foreach (var data in rendererData)
            {
                data.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault()?.SetActive(false);
            }
        }


        private void Update()
        {
            if (!realDimension)
            {
                PlayerMovementController.Instance.player.Health.TakeDamage(
                    alternateDimensionDamageRate * Time.deltaTime, "RAY");
            }
        }

        public void ShiftDimension(bool active)
        {
            realDimension = !active;
            if (realDimension)
            {
                PlayerMovementController.Instance.player.Health.ResetHealth();
            }

            if (alternateDimensionSpecific)
            {
                alternateDimensionSpecific.SetActive(active);
            }

            if (realDimensionSpecific)
            {
                realDimensionSpecific.SetActive(!active);
            }
        }

        public void EnableFullScreenRenderer(bool active)
        {
            if (_glitchCoroutine != null)
            {
                return;
            }

            foreach (var data in rendererData)
            {
                var fullScreenPass = data.rendererFeatures.OfType<FullScreenPassRendererFeature>().FirstOrDefault();

                if (fullScreenPass != null)
                {
                    _glitchCoroutine = StartCoroutine(GlitchMaterialTransition(fullScreenPass, active));
                }
            }
        }

        private IEnumerator GlitchMaterialTransition(FullScreenPassRendererFeature fullScreenPass, bool active)
        {
            if (active)
            {
                fullScreenPass.SetActive(true);
            }

            float timeElapsed = 0;
            float fraction = 0;

            while (fraction < 1)
            {
                timeElapsed += Time.deltaTime;

                float noiseAmount;
                float scanlineStrength;
                if (active)
                {
                    fraction = timeElapsed / 1f;
                    noiseAmount = Mathf.Lerp(0, 10, fraction);
                    scanlineStrength = Mathf.Lerp(1, 0, fraction);
                }
                else
                {
                    fraction = timeElapsed / 5f;
                    noiseAmount = Mathf.Lerp(10, 0, fraction);
                    scanlineStrength = Mathf.Lerp(0, 1, fraction);
                }

                fullScreenPass.passMaterial.SetFloat(NoiseAmount, noiseAmount);
                fullScreenPass.passMaterial.SetFloat(ScanlineStrength, scanlineStrength);

                yield return null;
            }


            if (!active)
            {
                fullScreenPass.SetActive(false);
            }

            _glitchCoroutine = null;
        }
    }
}