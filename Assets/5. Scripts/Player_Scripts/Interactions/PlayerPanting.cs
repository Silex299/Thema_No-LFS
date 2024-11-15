using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Interactions
{
    public class PlayerPanting : MonoBehaviour
    {

        [InfoBox("After what time of sprinting the player starts panting")]public float pantingThreshold = 1f;
        public float volumeTransitionSpeed = 1f;
        public float maximumVolume = 1f;
        private Player MyPlayer => PlayerMovementController.Instance.player;
        private Coroutine _pantingCoroutine;
        private float _startTime;

        public void StartPanting()
        {
            if (_pantingCoroutine != null) return;
            
            _pantingCoroutine = StartCoroutine(CheckForPanting());
            MyPlayer.EffectsManager.PlayLoopedSound("Pant");
            _startTime = Time.time;
        }

        public void StopPanting()
        {
            if(_pantingCoroutine==null) return;
            StopCoroutine(_pantingCoroutine);
            StartCoroutine(StopSound());
        }

        private IEnumerator CheckForPanting()
        {
            var audioSource = MyPlayer.EffectsManager.playerSfxSource;
            audioSource.volume = 0;
            
            while (true)
            {
                //if player is running with sprint with more than threshold seconds increase the sound volume of panting
                float input= Input.GetAxis(MyPlayer.UseHorizontal? "Horizontal": "Vertical");

                bool isSprinting = Mathf.Abs(input) > 0.5;

                if (isSprinting && Time.time - _startTime > pantingThreshold)
                {
                    audioSource.volume = Mathf.Lerp(audioSource.volume, maximumVolume, Time.deltaTime * volumeTransitionSpeed);
                }
                else
                {
                    audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * volumeTransitionSpeed);
                    if(!isSprinting) _startTime = Time.time;
                    
                }
                
                yield return null;
            }
            
        }

        private IEnumerator StopSound()
        {
            var audioSource = MyPlayer.EffectsManager.playerSfxSource;
            while (audioSource.volume > 0)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * volumeTransitionSpeed);
                if(Mathf.Approximately(audioSource.volume, 0)) break;
                yield return null;
            }
            
            MyPlayer.EffectsManager.StopLoopedSound();
        }
        
    }
}
