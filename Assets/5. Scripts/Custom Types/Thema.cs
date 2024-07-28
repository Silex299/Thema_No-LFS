using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Thema_Type
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
        Key_Release
    }

    [System.Serializable]
    public enum WhichStep
    {
        LEFT,
        RIGHT
    }

    #endregion


    #region Classes

    public static class ThemaVector
    {
        public static Vector3 GetClosestPointToLine(Vector3 from, Vector3 to, Vector3 point)
        {
            Vector3 line = to - from;
            Vector3 pointDirection = point - from;

            float  t = Vector3.Dot(pointDirection, line) / Vector3.Dot(line, line);

            t = float.IsNaN(t) ? 0 : Mathf.Clamp01(t);
            
            return from + line * t;
        }

        public static Vector3 GetClosestPointToLine(Vector3 line, Vector3 point)
        {
            Vector3 lineDirection = line.normalized;
            float t = Vector3.Dot(point, lineDirection);
            return t * lineDirection;
        }
    }


    [System.Serializable]
    public class AdvancedCurvedAnimation
    {
        public string animationName;
        public float animationTime;
        public float transitionTime;
        [OnValueChanged(nameof(Preview))] public float animationHeight;
        [OnValueChanged(nameof(Preview))] public float animationDistance;
        [OnValueChanged(nameof(Preview))] public AnimationCurve heightCurve;
        [OnValueChanged(nameof(Preview))] public AnimationCurve distanceCurve;


        #region EDITOR

#if UNITY_EDITOR

        //REMOVED: EVERYTHING IMPORTANT

        public Animator previewAAnimator;
        public Transform transform;

        [Range(0, 1), OnValueChanged(nameof(Preview))]
        public float normalisedTime;

        public void Preview()
        {
            previewAAnimator.Play(animationName, 1, normalisedTime);
            previewAAnimator.Update(0);

            Vector3 repos = transform.position +
                            transform.forward * distanceCurve.Evaluate(normalisedTime) * animationDistance +
                            transform.up * heightCurve.Evaluate(normalisedTime) * animationHeight;

            previewAAnimator.transform.position = repos;
            previewAAnimator.transform.rotation = transform.rotation;
        }

#endif

        #endregion

        /// <summary>
        /// Plays animation and follows the curves
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="target"></param>
        /// <param name="triggerTransform"></param>
        /// <param name="actionWidth"></param>
        /// <returns></returns>
        public IEnumerator PlayAnim(Animator animator, Transform target, Transform triggerTransform,
            float actionWidth = 0.01f)
        {
            animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = target.position;
            Quaternion initialPlayerRot = target.rotation;

            float timeElapsed = 0;

            while (timeElapsed < animationTime)
            {
                timeElapsed += Time.deltaTime;


                #region GetNormalized Time

                var currentClip = animator.GetCurrentAnimatorStateInfo(1);
                var nextClip = animator.GetNextAnimatorStateInfo(1);

                float normalizedTime = 0;

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
                                triggerTransform.forward * (distanceCurve.Evaluate(0.2f) * animationDistance) +
                                triggerTransform.up * (heightCurve.Evaluate(0.2f) * animationHeight);

                    target.position = Vector3.Lerp(initialPlayerPos, repos, normalizedTime / transitionTime);
                }
                else
                {
                    if (normalizedTime != 0)
                    {
                        var repos = triggerPos +
                                    triggerTransform.forward *
                                    (distanceCurve.Evaluate(normalizedTime) *
                                     animationDistance) +
                                    triggerTransform.up * (heightCurve.Evaluate(normalizedTime) *
                                                           animationHeight);

                        target.position = repos;
                    }
                }

                target.rotation = Quaternion.Lerp(initialPlayerRot, triggerTransform.rotation,
                    timeElapsed / transitionTime);

                yield return null;
            }

            target.rotation = triggerTransform.rotation;
        }

        /// <summary>
        /// Plays animation and directly moves to trigger position
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="target"></param>
        /// <param name="triggerTransform"></param>
        /// <returns></returns>
        public IEnumerator SimpleAnim(Animator animator, Transform target, Transform triggerTransform, float animationWidth = 0)
        {
            animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = target.position;
            Quaternion initialPlayerRot = target.rotation;

            Vector3 finalPos = ThemaVector.GetClosestPointToLine(from: triggerTransform.position - triggerTransform.right * animationWidth,
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

    #endregion
}