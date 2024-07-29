using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.Items
{
    public class RotatingDoor : MonoBehaviour
    {
        
        public Quaternion closedRotation;
        public Quaternion openRotation;
        
        public float transitionTime = 1f;
        
        public bool followCurve;
        [ShowIf(nameof(followCurve))]public AnimationCurve openCurve;
        [ShowIf(nameof(followCurve))]public AnimationCurve closeCurve;
        
        [Space(10)]
        public bool isOpen;

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
        

    }
}
