using System.Collections;
using System.ComponentModel.Design;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;

namespace Misc.Items
{
    public class RotatingDoor : MonoBehaviour
    {
        
        [FoldoutGroup("Door Movement")] public Quaternion closedRotation;
        [FoldoutGroup("Door Movement")] public Quaternion openRotation;
        
        [FoldoutGroup("Door Movement")] public float transitionTime = 1f;
        
        [FoldoutGroup("Door Movement")] public bool followCurve;
        [FoldoutGroup("Door Movement")] [ShowIf(nameof(followCurve))]public AnimationCurve openCurve;
        [FoldoutGroup("Door Movement")] [ShowIf(nameof(followCurve))]public AnimationCurve closeCurve;



        [FoldoutGroup("Sound")] public AudioSource source;
        [FoldoutGroup("Sound")] public SoundClip openSound;
        [FoldoutGroup("Sound")] public SoundClip closeSound;


        [FoldoutGroup("Misc")] [field: SerializeField] public bool isOpen { get; set; }
        private Coroutine _openDoorCoroutine;

#if UNITY_EDITOR
        
        [Button("Get Closed Rotation")]
        public void GetClosedRotation()
        {
            closedRotation = transform.rotation;
        }
        
        [Button("Get Open Rotation")]
        public void GetOpenRotation()
        {
            openRotation = transform.rotation;
        }

        /// <summary>
        /// Don't use this in game, this is just for testing purposes
        /// </summary>
        [Button("Toggle Door")]
        public void ToggleDoor()
        {
            OpenDoor(!isOpen);
        }
        
#endif

        public void OpenDoor(bool open)
        {
            isOpen = open;

            if (_openDoorCoroutine != null)
            {
                StopCoroutine(_openDoorCoroutine);
            }
            _openDoorCoroutine = StartCoroutine(OpenDoorCoroutine(open));
            PlaySound(open);
        }
        
        private IEnumerator OpenDoorCoroutine(bool open)
        {

            Quaternion initialRot = transform.rotation;
            
            float t = 0;
            
            while (t < transitionTime)
            {
                t += Time.deltaTime;

                if (!followCurve)
                {
                    transform.rotation = Quaternion.Slerp(initialRot, open ? openRotation : closedRotation, t / transitionTime);
                }
                else
                {
                    float normalisedTime =
                        open ? openCurve.Evaluate(t / transitionTime) : closeCurve.Evaluate(t / transitionTime);
                    
                    transform.rotation = Quaternion.SlerpUnclamped(initialRot, open ? openRotation : closedRotation, normalisedTime);
                }
                
                yield return null;
            }
            
        }

        private void PlaySound(bool open)
        {
            if(!source) return;

            if (open && openSound.clip)
            {
                source.PlayOneShot(openSound.clip, openSound.volume);
            }
            else if(closeSound.clip)
            {
                source.PlayOneShot(closeSound.clip, closeSound.volume);
            }
        }

    }
}
