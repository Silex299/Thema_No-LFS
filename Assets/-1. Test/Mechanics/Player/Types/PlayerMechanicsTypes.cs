using System;
using System.Collections;
using Mechanics.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Types
{
    [Serializable]
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


        public IEnumerator PlayAnim(Transform sourceTransform, PlayerV1 player, float actionWidth = 0.01f,
            TimedAction timedAction = null)
        {
            player.animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = player.transform.position;
            Quaternion initialPlayerRot = player.transform.rotation;

            float timeElapsed = 0;

            while (timeElapsed < animationTime)
            {
                timeElapsed += Time.deltaTime;


                #region GetNormalized Time

                var currentClip = player.animator.GetCurrentAnimatorStateInfo(1);
                var nextClip = player.animator.GetNextAnimatorStateInfo(1);

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
                
                #region GetclosedPointToLine

                //Get the closest position to the source
                Vector3 desiredTargetPos = GameVector.GetClosestPointToLine(
                    sourceTransform.position - (sourceTransform.right * actionWidth),
                    to: sourceTransform.position + (sourceTransform.right * actionWidth),
                    point: player.transform.position);

                #endregion

                #region Set position and Rotation

                
                if (normalizedTime < transitionTime)
                {
                    var repos = desiredTargetPos +
                                sourceTransform.forward *
                                (distanceCurve.Evaluate(transitionTime) * animationDistance) +
                                sourceTransform.up * (heightCurve.Evaluate(transitionTime) * animationHeight);

                    player.transform.position = Vector3.Lerp(initialPlayerPos, repos, normalizedTime / transitionTime);
                }
                else
                {
                    if (normalizedTime != 0)
                    {
                        var repos = desiredTargetPos +
                                    sourceTransform.forward *
                                    (distanceCurve.Evaluate(normalizedTime) *
                                     animationDistance) +
                                    sourceTransform.up * (heightCurve.Evaluate(normalizedTime) *
                                                          animationHeight);

                        player.transform.position = repos;
                    }
                }
                
                player.transform.rotation = Quaternion.Lerp(initialPlayerRot, sourceTransform.rotation,
                    normalizedTime / transitionTime);
                
                

                #endregion

                #region TimedAction

                if (timedAction != null && timeElapsed >= timedAction.time)
                {
                    timedAction.action.Invoke();
                    timedAction = null;
                }

                #endregion
                
                yield return new WaitForEndOfFrame();
            }
            
            if (timedAction != null)
            {
                timedAction.action.Invoke();
            }
            
        }
    }
}
