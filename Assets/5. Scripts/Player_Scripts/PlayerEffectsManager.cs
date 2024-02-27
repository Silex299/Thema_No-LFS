using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thema_Type;

namespace Player_Scripts
{
    public class PlayerEffectsManager : SerializedMonoBehaviour
    {


        [SerializeField, TabGroup("References", "Player")] private Player player;

        [SerializeField, TabGroup("References", "Audio Source")] private AudioSource interactionSource;
        [SerializeField, TabGroup("References", "Audio Source", Order = 0)] private AudioSource playerSoundSource;

        [SerializeField, TabGroup("Effects", "Step Sockets"), Space(5f)] private Transform leftFootSocket;
        [SerializeField, TabGroup("Effects", "Step Sockets")] private Transform rightFootSocket;
        [SerializeField, TabGroup("Effects", "Step Sockets")] private LayerMask raycastMask;
        [SerializeField, TabGroup("Effects", "Step Effects"), Space(5f)] private Dictionary<string, GameObject> stepEffects;
        [SerializeField, TabGroup("Effects", "Step Audio Clips"), Space(5f)] private Dictionary<string, List<AudioClip>> stepSounds;


        [SerializeField, TabGroup("Effects", "General Sounds"), Space(5f), InfoBox("This dictionary contains, Ground Layer as key= { Sound Name as key = {List of sounds(that will be chosed at random)} }")]
        private Dictionary<string, Dictionary<string, List<AudioClip>>> generalSounds;





        private string _currentEffectVolume = default;
        public string CurrentEffectVolume
        {
            set
            {
                _currentEffectVolume = value;
            }
        }
        private float _volumeMultiplier = 1;
        public float VolumeMultiplier
        {
            set
            {
                _volumeMultiplier = value;
            }
        }


        private static PlayerEffectsManager _instance;

        public static PlayerEffectsManager Instance
        {
            get => _instance;
        }


        private void Start()
        {
            if (PlayerEffectsManager.Instance != null)
            {
                Destroy(this);
            }
            else
            {
                PlayerEffectsManager._instance = this;
            }
        }


      

        public void PlaySteps(Object footInfo)
        {
            var step = footInfo as Step;

            if (!player.IsGrounded || player.DisablePlayerMovement) return;

            //Call for Spawn Steps
            StartCoroutine(SpawnSteps(step.whichStep));

            float rawInput = Input.GetAxis(player.UseHorizontal ? "Horizontal" : "Vertical");
            float volume = Mathf.Abs(rawInput) * step.volume * _volumeMultiplier;

            try
            {
                if (stepSounds.TryGetValue(_currentEffectVolume, out var clips))
                {
                    interactionSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volume);
                }
            }
            catch
            {
                print("Step sound not found for : " + _currentEffectVolume);
            }
        }

        private IEnumerator SpawnSteps(WhichStep step)
        {

            yield return new WaitForSeconds(0.3f);
            //Left Foot

            try
            {
                if (stepEffects.TryGetValue(_currentEffectVolume, out GameObject pref))
                {

                    switch (step)
                    {
                        case WhichStep.LEFT:

                            if (Physics.Raycast(leftFootSocket.position + Vector3.up, Vector3.down, out RaycastHit hit1, 1.5f, raycastMask))
                            {
                                Debug.DrawLine(leftFootSocket.position + Vector3.up, hit1.point, Color.yellow, 5f);

                                Instantiate(pref, hit1.point, Quaternion.LookRotation(transform.forward));
                            }

                            break;
                        case WhichStep.RIGHT:

                            if (Physics.Raycast(rightFootSocket.position + Vector3.up, Vector3.down, out RaycastHit hit2, 1.5f, raycastMask))
                            {
                                Debug.DrawLine(rightFootSocket.position + Vector3.up, hit2.point, Color.yellow, 5f);
                                Instantiate(pref, hit2.point, Quaternion.LookRotation(transform.forward));
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            catch
            {
                Debug.LogWarning("No step effect found for :" + _currentEffectVolume);
            }


        }

        //For playing Steps (volume devpendent only for sound volume)
        public void PlayStepsIndependent(string soundKey)
        {

            if (player.DisablePlayerMovement) return;

            try
            {
                if (stepSounds.TryGetValue(soundKey, out var clips))
                {
                    interactionSource.PlayOneShot(clips[Random.Range(0, clips.Count)], _volumeMultiplier);
                }
            }
            catch
            {
                Debug.LogWarning("Sound Error with loading Independent step sounds");
            }
        }


        /// <summary>
        /// For playing Gneral Sound 
        /// Current Volume Depedent
        /// e.g. Land Jump
        /// </summary>
        /// <param name="soundKey"> action name </param>
        public void PlayGeneralSoundRandom(string soundKey)
        {
            if (player.DisablePlayerMovement) return;
            try
            {
                if (generalSounds.TryGetValue(_currentEffectVolume, out var sounds))
                {
                    if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                    {
                        var randomIndex = Random.Range(0, clips.Count);
                        interactionSource.PlayOneShot(clips[randomIndex], 0.7f);
                    }
                }

            }
            catch
            {
                Debug.LogWarning("Some Error occured");
            }

        }

        /// <summary>
        /// For playing general Default Sounds
        /// Not current volume dependent
        /// e.g, moan, shout, hurt
        /// </summary>
        /// <param name="soundKey">action name</param>
        public void PlayGeneralSoundDefaultRandom(string soundKey)
        {
            if (player.DisablePlayerMovement) return;

            print(soundKey);
            try
            {
                if (generalSounds.TryGetValue("Default", out var sounds))
                {
                    if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                    {
                        var randomIndex = Random.Range(0, clips.Count);
                        print(clips[randomIndex]);
                        playerSoundSource.PlayOneShot(clips[randomIndex], 0.7f);
                    }
                }

            }
            catch
            {
                Debug.LogWarning("Some Error occured");
            }
        }




    }
}
