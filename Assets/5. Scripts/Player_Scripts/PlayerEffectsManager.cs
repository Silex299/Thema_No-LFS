using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Thema_Type;
using UnityEditor.Rendering;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Player_Scripts
{
    public class PlayerEffectsManager : SerializedMonoBehaviour
    {
        [SerializeField, TabGroup("References", "Player")]
        private Player player;

        [SerializeField, TabGroup("References", "Audio Source")]
        private AudioSource interactionSource;

        [SerializeField, TabGroup("References", "Audio Source", Order = 0)]
        private AudioSource playerSoundSource;

        [SerializeField, TabGroup("References", "Audio Source")]
        private AudioSource otherSoundSource;

        [SerializeField, TabGroup("Steps", "Step Sockets"), Space(5f)]
        private Transform leftFootSocket;

        [SerializeField, TabGroup("Steps", "Step Sockets")]
        private Transform rightFootSocket;

        [SerializeField, TabGroup("Steps", "Step Sockets")]
        private LayerMask raycastMask;

        [SerializeField, TabGroup("Steps", "Step Effects"), Space(5f)]
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, GameObject> stepEffects;

        [SerializeField, TabGroup("Steps", "Step Audio Clips"), Space(5f)]
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, List<AudioClip>> stepSounds;


        [SerializeField, TabGroup("Other Sounds", "Interaction Sounds"), Space(5f),
         InfoBox(
             "This dictionary contains, Ground Layer as key= { Sound Name as key = {List of sounds(that will be chose at random)} }")]
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, Dictionary<string, List<AudioClip>>> interactionSounds;


        [SerializeField, TabGroup("Other Sounds", "Player Sounds")]
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, List<AudioClip>> playerSounds;

        [SerializeField, TabGroup("Other Sounds", "Other Sounds")]
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, AudioClip> otherSounds;


        public string currentEffectVolume = default;

        public string CurrentEffectVolume
        {
            set => currentEffectVolume = value;
        }

        private float _volumeMultiplier = 1;

        public float VolumeMultiplier
        {
            set => _volumeMultiplier = value;
        }


        private static PlayerEffectsManager _instance;

        public static PlayerEffectsManager Instance => _instance;

        private Coroutine _playerMovementCoroutine;

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


            player.Health.onTakingDamage += PlayHeartBeat;
        }

        private void OnDisable()
        {
            player.Health.onTakingDamage -= PlayHeartBeat;
        }

        /// <summary>
        /// Plays on taking damage
        /// </summary>
        /// <param name="fraction">higher the damage more the volume</param>
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
                        otherSoundSource.Play();
                        AudioClip clip;

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

        #region Interactions

        #region For Steps

        /// <summary>
        /// Plays step sound and spawns Step effect if there's any
        /// </summary>
        /// <param name="footInfo"></param>
        public void PlaySteps(Object footInfo)
        {
            var step = footInfo as Step;

            if (!player.IsGrounded || player.DisabledPlayerMovement) return;

            StartCoroutine(SpawnSteps(step));
        }

        private IEnumerator SpawnSteps(Step stepInfo)
        {
            yield return new WaitForSeconds(0.3f);

            var step = stepInfo.whichStep;
            
            
            switch (step)
            {
                case WhichStep.LEFT:

                    //Not sure its acting weird, so added 100f to raycast

                    Ray ray1 = new Ray(transform.position + Vector3.up, (leftFootSocket.position - transform.position -  Vector3.up).normalized);
                    
                    Debug.DrawRay(ray1.origin, ray1.direction * 10f, Color.red, 1f);
                    
                    if (Physics.Raycast(ray1, out RaycastHit hit1, 100f, raycastMask))
                    {
                        var hitSurface = hit1.collider.tag;
                        //Spawn Effects
                        if (stepEffects.ContainsKey(hitSurface))
                        {
                            if (stepEffects.TryGetValue(hitSurface, out var pref))
                            {
                                Instantiate(pref, hit1.point, Quaternion.LookRotation(transform.forward));
                            }
                            
                        }
                        
                        PlayStepSound(hitSurface);
                        
                    }

                    break;
                case WhichStep.RIGHT:

                    Ray ray2 = new Ray(transform.position + Vector3.up, (rightFootSocket.position - transform.position - Vector3.up).normalized);
                    
                    Debug.DrawRay(ray2.origin, ray2.direction * 10f, Color.yellow, 1f);
                    
                    if (Physics.Raycast(ray2, out RaycastHit hit2, 100f, raycastMask))
                    {
                        var hitSurface = hit2.collider.tag;
                        
                        //Spawn Effects
                        if (stepEffects.ContainsKey(hitSurface))
                        {
                            if (stepEffects.TryGetValue(hitSurface, out var pref))
                            {
                                Instantiate(pref, hit2.point, Quaternion.LookRotation(transform.forward));
                            }
                        }
                        
                        PlayStepSound(hitSurface);
                        
                    }

                    break;
                default:
                    print("fuck");
                    break;
            }

            yield break;


            void PlayStepSound(string hitSurface)
            {
                //Play Sounds
                if (!stepSounds.ContainsKey(hitSurface)) return;
                if (!stepSounds.TryGetValue(hitSurface, out List<AudioClip> clips)) return;
                
                float rawInput = Input.GetAxis(player.UseHorizontal ? "Horizontal" : "Vertical");
                float volume = Mathf.Abs(rawInput) * stepInfo.volume * _volumeMultiplier;
                interactionSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volume);
            }
            
        }

        #endregion

        /// <summary>
        /// For playing Interaction Sound 
        /// Current Volume Dependent
        /// e.g. Land Jump
        /// </summary>
        /// <param name="soundKey"> interaction name, e.g. Land, Jump </param>
        public void PlayInteractionSound(string soundKey)
        {
            Ray ray2 = new Ray(transform.position + 2*Vector3.up, -Vector3.up);
                    
            Debug.DrawRay(ray2.origin, ray2.direction * 10f, Color.blue, 1f);

            if (Physics.Raycast(ray2, out RaycastHit hit, 100f, raycastMask))
            {
                string effectSurface = hit.collider.tag; 
                
                if (!interactionSounds.ContainsKey(effectSurface)) return;
                if (interactionSounds.TryGetValue(effectSurface, out var sounds))
                {
                    if (!sounds.ContainsKey(soundKey)) return;

                    if (sounds.TryGetValue(soundKey, out List<AudioClip> clips))
                    {
                        var randomIndex = Random.Range(0, clips.Count);
                        interactionSource.PlayOneShot(clips[randomIndex], _volumeMultiplier);
                    }
                }
            }
            
        }

        #endregion

        #region Player Sounds

        /// <summary>
        /// For playing Player Sounds
        /// e.g, moan, shout, hurt
        /// </summary>
        /// <param name="soundKey">action name</param>
        public void PlayPlayerSound(string soundKey)
        {
            if (!playerSounds.ContainsKey(soundKey))
            {
                return;
            }
            
            if (playerSounds.TryGetValue(soundKey, out List<AudioClip> clips))
            {
                var randomIndex = Random.Range(0, clips.Count);
                playerSoundSource.PlayOneShot(clips[randomIndex], _volumeMultiplier);
            }
        }

        public void PlayPlayerMovementSound(string soundKey)
        {
            if(!playerSounds.ContainsKey(soundKey)) return;
            
            if (playerSounds.TryGetValue(soundKey, out List<AudioClip> clips))
            {
                var randomIndex = Random.Range(0, clips.Count);
                playerSoundSource.PlayOneShot(clips[randomIndex]);
                float clipLength = clips[randomIndex].length;
                _playerMovementCoroutine ??= StartCoroutine(SetVelocityEffectedVolume(clipLength));
            }
        }

        private IEnumerator SetVelocityEffectedVolume(float time)
        {
            float timeElapsed = 0;

            while (timeElapsed < time)
            {
                timeElapsed += Time.deltaTime;
                float newVolume = Mathf.Clamp(PlayerVelocityCalculator.Instance.velocity.magnitude / 1, 0,
                    _volumeMultiplier);
                playerSoundSource.volume = newVolume;
                yield return null;
            }

            _playerMovementCoroutine = null;
        }

        #endregion

        public void PlayOtherSounds(string soundKey, float volume = 0.5f)
        {
            if(!otherSounds.ContainsKey(soundKey)) return;
            
            if (otherSounds.TryGetValue(soundKey, out AudioClip clip))
            {
                otherSoundSource.PlayOneShot(clip, volume);
            }
        }

        public GameObject SpawnEffects(string key, Vector3 overridePos, Transform parent)
        {
            if (!stepEffects.ContainsKey(key)) return null;

            if (stepEffects.TryGetValue(key, out GameObject pref))
            {
                return Instantiate(pref, overridePos, Quaternion.LookRotation(transform.forward), parent);
            }

            return null;
        }
        
        #region Obosolette

        //Remove this
        public void SetInteractionSound(float value)
        {
            interactionSource.volume = value;
        }

        //REMOVE THIS
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
                    if (interactionSounds.TryGetValue("Default", out var sounds))
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
        

        #endregion
    }
}