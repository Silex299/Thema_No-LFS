using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Thema
{
    #region Types

    /// <summary>
    /// Indicated how player should interact with the key, If held, release, on press key
    /// </summary>
    [System.Serializable]
    public enum KeyInputType
    {
        Key_Press,
        Key_Hold,
        Key_Release,
        Key_Axis,
        Axis_Press
    }

    [System.Serializable]
    public enum WhichStep
    {
        LEFT,
        RIGHT
    }
    
    [System.Serializable]
    public struct SoundClip
    {
        public AudioClip clip;
        public float volume;
    }
    
    [System.Serializable]
    public struct SoundClipArray
    {
        public AudioClip[] clips;
        public float volume;
    }

    
    [System.Serializable]
    public struct SoundSource
    {
        public AudioSource source;
        public float maximumVolume;
    }
    
    #endregion


    #region Classes

    public static class ThemaVector
    {
        public static Vector3 GetClosestPointToLine(Vector3 from, Vector3 to, Vector3 point)
        {
            Vector3 line = to - from;
            Vector3 pointDirection = point - from;

            float t = Vector3.Dot(pointDirection, line) / Vector3.Dot(line, line);

            t = float.IsNaN(t) ? 0 : Mathf.Clamp01(t);

            return from + line * t;
        }

        public static Vector3 GetClosestPointToLine(Vector3 line, Vector3 point)
        {
            Vector3 lineDirection = line.normalized;
            float t = Vector3.Dot(point, lineDirection);
            return t * lineDirection;
        }

        public static float PlannerDistance(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }


        public static int ClosestPoint(IEnumerable<Vector3> points, Vector3 point, out Vector3 closestPoint)
        {
            float distance = float.MaxValue;
            int closestPointIndex = 0;
            closestPoint = Vector3.zero;
            
            int i = 0;
            
            foreach (var pt in points)
            {
                float currentIndexDistance = PlannerDistance(point, pt);
                if (distance > currentIndexDistance)
                {
                    closestPointIndex = i;
                    distance = currentIndexDistance;
                    closestPoint = pt;
                }

                i++;
            }
            
            return closestPointIndex;

        }
        
    }
    
    [System.Serializable]
    public class AdvancedCurvedAnimation
    {
        public string animationName;
        public float animationTime;
        public float transitionTime;
        [Sirenix.OdinInspector.OnValueChanged("Preview")] public float animationHeight;
        [Sirenix.OdinInspector.OnValueChanged("Preview")] public float animationDistance;
        [Sirenix.OdinInspector.OnValueChanged("Preview")] public AnimationCurve heightCurve;
        [Sirenix.OdinInspector.OnValueChanged("Preview")] public AnimationCurve distanceCurve;
        public bool followRotationCurve;

        [Sirenix.OdinInspector.OnValueChanged("Preview"), Sirenix.OdinInspector.ShowIf(nameof(followRotationCurve))]
        public AnimationCurve rotationCurve;


        #region EDITOR

#if UNITY_EDITOR

        //REMOVED: EVERYTHING IMPORTANT

        public Animator previewAAnimator;

        [Range(0, 1), Sirenix.OdinInspector.OnValueChanged(nameof(Preview))]
        public float normalisedTime;

        public void Preview()
        {
            previewAAnimator.Play(animationName, 1, normalisedTime);
            previewAAnimator.Update(0);

            Vector3 repos = transform.position +
                            transform.forward * distanceCurve.Evaluate(normalisedTime) * animationDistance +
                            transform.up * heightCurve.Evaluate(normalisedTime) * animationHeight;

            previewAAnimator.transform.position = repos;

            previewAAnimator.transform.rotation = followRotationCurve ? Quaternion.Euler(transform.eulerAngles.x, transform.rotation.eulerAngles.y + rotationCurve.Evaluate(normalisedTime) * 180, transform.eulerAngles.z) : transform.rotation;
        }

#endif

        public Transform transform;
        
        #endregion

        /// <summary>
        /// Plays animation and follows the curves
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="target"></param>
        /// <param name="triggerTransform"></param>
        /// <param name="actionWidth"></param>
        /// <param name="timedAction"></param>
        /// <returns></returns>
        public IEnumerator PlayAnim(Animator animator, Transform target, Transform triggerTransform,
            float actionWidth = 0.01f, TimedAction timedAction = null)
        {
            animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = target.position;
            Quaternion initialPlayerRot = target.rotation;

            float timeElapsed = 0;
            float normalizedTime = 0;

            while (timeElapsed < animationTime)
            {
                timeElapsed += Time.deltaTime;


                #region GetNormalized Time

                var currentClip = animator.GetCurrentAnimatorStateInfo(1);
                var nextClip = animator.GetNextAnimatorStateInfo(1);


                if (currentClip.IsName(animationName))
                {
                    normalizedTime = currentClip.normalizedTime;
                }
                else if (nextClip.IsName(animationName))
                {
                    normalizedTime = nextClip.normalizedTime;
                }

                #endregion


                Vector3 triggerPos = ThemaVector.GetClosestPointToLine(
                    from: triggerTransform.position - (triggerTransform.right * actionWidth),
                    to: triggerTransform.position + (triggerTransform.right * actionWidth),
                    point: target.position);


                if (normalizedTime < transitionTime)
                {
                    var repos = triggerPos +
                                triggerTransform.forward *
                                (distanceCurve.Evaluate(transitionTime) * animationDistance) +
                                triggerTransform.up * (heightCurve.Evaluate(transitionTime) * animationHeight);

                    if (followRotationCurve)
                    {
                        var rot = Quaternion.Euler(initialPlayerRot.eulerAngles.x, transform.rotation.eulerAngles.y + rotationCurve.Evaluate(normalizedTime) * 180, initialPlayerRot.eulerAngles.z);
                        target.rotation = Quaternion.Lerp(initialPlayerRot, rot, normalizedTime / transitionTime);
                    }

                    target.position = Vector3.Lerp(initialPlayerPos, repos, normalizedTime / transitionTime);
                }
                else
                {
                    var repos = triggerPos +
                                triggerTransform.forward *
                                (distanceCurve.Evaluate(normalizedTime) *
                                 animationDistance) +
                                triggerTransform.up * (heightCurve.Evaluate(normalizedTime) *
                                                       animationHeight);

                    target.position = repos;


                    if (followRotationCurve)
                    {
                        var rot = Quaternion.Euler(initialPlayerRot.eulerAngles.x, transform.rotation.eulerAngles.y + rotationCurve.Evaluate(normalizedTime) * 180, initialPlayerRot.eulerAngles.z);
                        target.rotation = Quaternion.Lerp(initialPlayerRot, rot, normalizedTime / transitionTime);
                    }
                }

                if (!followRotationCurve && normalizedTime < transitionTime) //MAY NEED CHANGE INITIALLY IT WAS ONLY !followRotationCurve
                {
                    target.rotation = Quaternion.Lerp(initialPlayerRot, triggerTransform.rotation,
                        timeElapsed / transitionTime);
                }

                if (timedAction != null)
                {
                    if (timeElapsed / animationTime >= timedAction.time)
                    {
                        timedAction.action?.Invoke();
                        timedAction = null;
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Plays animation and directly moves to trigger position
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="target"></param>
        /// <param name="triggerTransform"></param>
        /// <returns></returns>
        public IEnumerator SimpleAnim(Animator animator, Transform target, Transform triggerTransform,
            float animationWidth = 0)
        {
            animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = target.position;
            Quaternion initialPlayerRot = target.rotation;

            Vector3 finalPos = ThemaVector.GetClosestPointToLine(
                from: triggerTransform.position - triggerTransform.right * animationWidth,
                to: triggerTransform.position + triggerTransform.right * animationWidth,
                point: target.position);

            float timeElapsed = 0;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;

                target.position = Vector3.Lerp(initialPlayerPos, finalPos,
                    timeElapsed / transitionTime);
                target.rotation = Quaternion.Lerp(initialPlayerRot, triggerTransform.rotation,
                    timeElapsed / transitionTime);

                yield return null;
            }
        }
    }


    [Serializable, CanBeNull]
    public class TimedAction
    {
        public float time;
        public Action action;

        public TimedAction(float time, Action action)
        {
            this.time = time;
            this.action = action;
        }
    }

    #endregion

    #region Data Stuctures
    
    public class PriorityQueue<T>
    {
        
        private readonly SortedList<float, T> _list = new SortedList<float, T>(Comparer<float>.Create((x, y) => x.CompareTo(y))); // ascending order

        /// <summary>
        /// Enqueues the item with the given priority.
        /// </summary>
        /// <param name="item"> The item to enqueue </param>
        /// <param name="priority">Priority of the Item</param>
        public void Enqueue(float priority, T item)
        {
            if (_list.ContainsKey(priority))
            {
                Debug.Log(item);
            }
            _list.Add(priority, item); // Add a new item with the given priority
        }

        public KeyValuePair<float, T> Dequeue()
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("The sorted queue is empty.");
            }
            
            var firstPair = _list.First();
            _list.Remove(firstPair.Key);
            
            return firstPair;
        }

        public bool IsEmpty => _list.Count == 0;
        
        public int Count => _list.Count;
    }

    [System.Serializable]
    public class ThemaInput
    {
        public KeyInputType inputType;

        [ShowIf("@inputType == KeyInputType.Key_Axis || inputType == KeyInputType.Axis_Press")]
        public float threshold;
        
        [ShowIf("@inputType == KeyInputType.Key_Axis || inputType == KeyInputType.Axis_Press")]
        public bool invertedAxis;

        
        
        public bool GetInput(string inputString)
        {
            switch (inputType)
            {
                case KeyInputType.Key_Press:
                    return Input.GetButtonDown(inputString);
                case KeyInputType.Key_Hold:
                    return Input.GetButton(inputString);
                case KeyInputType.Key_Release:
                    return Input.GetButtonUp(inputString);
                case KeyInputType.Key_Axis:
                    return !invertedAxis ? Input.GetAxis(inputString) > threshold : Input.GetAxis(inputString) < threshold;
                case KeyInputType.Axis_Press:
                    return !invertedAxis ? Mathf.Abs(Input.GetAxis(inputString)) > threshold : Mathf.Abs(Input.GetAxis(inputString)) < threshold;
                default:
                    return false;
            }
        }
    }

    #endregion
}