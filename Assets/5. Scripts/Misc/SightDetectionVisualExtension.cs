using System.Collections;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;

namespace Misc
{
    public class SightDetectionVisualExtension : MonoBehaviour
    {
        
        [FoldoutGroup("Visual")] public GameObject visualElement;
        [Required] [FoldoutGroup("Visual")] public ChangeMaterials materialChanger;
        
        [FoldoutGroup("Flicker")] public int flickerCount = 1;
        [FoldoutGroup("Flicker")] public float flickerInterval = 0.4f;
        
        [FoldoutGroup("Sound")] public AudioSource audioSource;
        [FoldoutGroup("Sound")] public SoundClip detectionSound;


        private Coroutine _detectionCoroutine;

        [Button("Test Flicker")]
        public void OnDetection()
        {
            _detectionCoroutine ??= StartCoroutine(DetectionFlicker());
        }

        private IEnumerator DetectionFlicker()
        {
            
            audioSource?.PlayOneShot(detectionSound.clip, detectionSound.volume);
            
            for (int i = 0; i < flickerCount; i++)
            {
                visualElement.SetActive(false);
                materialChanger.ChangeMaterial(true);
                yield return new WaitForSeconds(flickerInterval);
                visualElement.SetActive(true);
                materialChanger.ChangeMaterial(false);
                yield return new WaitForSeconds(flickerInterval);
            }
            
            _detectionCoroutine = null;
        }
    }
}