using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thema_Type;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Player_Scripts
{
    public class PlayerEffectsManager : SerializedMonoBehaviour
    {


        [SerializeField, TabGroup("References", "Player")] private Player player;

        [SerializeField, TabGroup("References", "Audio Source")] private AudioSource interactionSource;
        [SerializeField, TabGroup("References", "Audio Source", Order = 0)] private AudioSource playerSoundSource;
        [SerializeField, TabGroup("References", "Audio Source")] private AudioSource otherSoundSource;

        [SerializeField, TabGroup("Effects", "Step Sockets"), Space(5f)] private Transform leftFootSocket;
        [SerializeField, TabGroup("Effects", "Step Sockets")] private Transform rightFootSocket;
        [SerializeField, TabGroup("Effects", "Step Sockets")] private LayerMask raycastMask;
        [SerializeField, TabGroup("Effects", "Step Effects"), Space(5f)] private Dictionary<string, GameObject> stepEffects;
        [SerializeField, TabGroup("Effects", "Step Audio Clips"), Space(5f)] private Dictionary<string, List<AudioClip>> stepSounds;


        [SerializeField, TabGroup("Effects", "General Sounds"), Space(5f), InfoBox("This dictionary contains, Ground Layer as key= { Sound Name as key = {List of sounds(that will be chosed at random)} }")]
        private Dictionary<string, Dictionary<string, List<AudioClip>>> generalSounds;

        [SerializeField, TabGroup("Effects", "Other Sounds")]
        private Dictionary<string, AudioClip> otherSounds;





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


            player.Health.OnTakingDamage += PlayHeartBeat;
        }

        private void OnDisable()
        {
            player.Health.OnTakingDamage -= PlayHeartBeat;
        }



        private void PlayHeartBeat(float fraction)
        {
            if (otherSoundSource)
            {

                if (fraction > 0.5f)
                {
                    otherSoundSource.Stop();
                }
                else
                {
                    if (!otherSoundSource.isPlaying)
                    {
                        otherSoundSource.Play(); AudioClip clip;

                        if (otherSounds.TryGetValue("heartbeat", out clip))
                        {
                            otherSoundSource.clip = clip;
                        }
                        else
                        {
                            Debug.LogWarning("Couldn't find heartbeat track");
                        }
                    }

                    //TODO : Check for clip match(if sound source is playing other clips already)

                    otherSoundSource.volume = 1 - fraction;

                    otherSoundSource.pitch = Mathf.Clamp(fraction + 0.5f, 0, 1.2f);

                }

            }
        }
        
        public void PlaySteps(Object footInfo)
        {
            var step = footInfo as Step;

            if (!player.IsGrounded || player.DisabledPlayerMovement) return;

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
                //print("Step sound not found for : " + _currentEffectVolume);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leftFootSocket.position, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rightFootSocket.position, 0.1f);
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

                            //Not sure its acting weird, so added 100f to raycast
                            
                            Ray ray1 = new Ray(leftFootSocket.position + Vector3.up * 100f, Vector3.down);
                            Debug.DrawRay(ray1.origin, ray1.direction * 200f, Color.red, 1f);
                            if (Physics.Raycast(ray1, out RaycastHit hit1, 200f, raycastMask))
                            {
                                Instantiate(pref, hit1.point, Quaternion.LookRotation(transform.forward));
                            }

                            break;
                        case WhichStep.RIGHT:
                            
                            Ray ray2 = new Ray(leftFootSocket.position + Vector3.up * 100f, Vector3.down);
                            Debug.DrawRay(ray2.origin, ray2.direction * 200f, Color.yellow, 1f);
                            if (Physics.Raycast(ray2, out RaycastHit hit2, 200f, raycastMask))
                            {
                                Instantiate(pref, hit2.point, Quaternion.LookRotation(transform.forward));
                            }
                            break;
                        default:
                            print("fuck");
                            break;
                    }

                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }


        }

        //For playing Steps (volume devpendent only for sound volume)
        public void PlayStepsIndependent(string soundKey)
        {

            if (player.DisabledPlayerMovement) return;

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
            if (player.DisabledPlayerMovement) return;
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

            try
            {
                if (generalSounds.TryGetValue("Default", out var sounds))
                {
                    if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                    {
                        var randomIndex = Random.Range(0, clips.Count);
                        playerSoundSource.PlayOneShot(clips[randomIndex], _volumeMultiplier);
                    }
                }

            }
            catch
            {
            }
        }

        public void PlayGeneralSoundDefaultRandom(string soundKey, float volume)
        {
            if (player.DisabledPlayerMovement) return;

            try
            {
                if (generalSounds.TryGetValue("Default", out var sounds))
                {
                    if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                    {
                        var randomIndex = Random.Range(0, clips.Count);
                        playerSoundSource.PlayOneShot(clips[randomIndex], volume);
                    }
                }

            }
            catch
            {
                Debug.LogWarning("Some Error occured");
            }
        }

        public void SetInteractionSound(float value)
        {
            interactionSource.volume = value;
        }

        public void PlayLoopInteraction(string soundKey, bool play)
        {
            if (!play)
            {
                interactionSource.loop = false;
                interactionSource.Stop();
            }
            else
            {
                try
                {
                    if (generalSounds.TryGetValue("Default", out var sounds))
                    {
                        if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                        {
                            var randomIndex = Random.Range(0, clips.Count);

                            if (!interactionSource.isPlaying)
                            {
                                interactionSource.loop = true;
                                interactionSource.clip = clips[randomIndex];
                                interactionSource.Play();
                            }
                        }
                    }

                }
                catch
                {
                    Debug.LogWarning("Some Error occured");
                }
            }


        }


        public GameObject SpawnEffect(string key, Vector3 overridePosition)
        {
            return Instantiate(stepEffects[key], transform.position + overridePosition, Quaternion.identity, this.transform);
        }


    }
}
