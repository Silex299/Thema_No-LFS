using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Interactions.Animation
{
    public class AdvancedAnimationTrigger : MonoBehaviour
    {
        [BoxGroup("Animation")] public string animationName;
        [BoxGroup("Animation")] public float animationTime = 1;

        [BoxGroup("Movement")] public float transitionTime;

        [OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
        public float animationHeight;

        [OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
        public float animationDistance;

        [OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
        public AnimationCurve heightCurve;

        [OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
        public AnimationCurve distanceCurve;


        [BoxGroup("State")] public bool changeState;

        [BoxGroup("State"), ShowIf(nameof(changeState))]
        public int stateIndex;

        [BoxGroup("State"), ShowIf(nameof(changeState)), Range(0, 1)]
        public float overrideTime;

        [BoxGroup("State"), ShowIf(nameof(changeState))]
        public bool overrideAnimation;

        [BoxGroup("State"), ShowIf(nameof(overrideAnimation))]
        public string overrideAnimationName;

        [BoxGroup("Event")] public UnityEvent onActionStart;
        [BoxGroup("Event")] public UnityEvent onActionEnd;


        private Coroutine _triggerActionCoroutine;

        #region Editor

        [Range(0, 1), OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
        public float normalisedTime;

        public void SetNormalisedTime()
        {
            Animator animator = FindObjectOfType<Player>().GetComponent<Animator>();

            animator.Play(animationName, 1, normalisedTime);
            animator.Update(0);

            Vector3 repos = transform.position +
                            transform.forward * distanceCurve.Evaluate(normalisedTime) * animationDistance +
                            transform.up * heightCurve.Evaluate(normalisedTime) * animationHeight;

            animator.transform.position = repos;
            animator.transform.rotation = transform.rotation;
        }

        #endregion


        public void Trigger()
        {
            _triggerActionCoroutine ??= StartCoroutine(TriggerAnimation());
        }

        private IEnumerator TriggerAnimation()
        {
            onActionStart.Invoke();

            Player player = PlayerMovementController.Instance.player;

            #region Player Movement

            player.DisabledPlayerMovement = true;
            player.CController.enabled = false;

            #endregion


            player.AnimationController.CrossFade(animationName, transitionTime, 1);


            Vector3 initialPlayerPos = player.transform.position;
            Quaternion initialPlayerRot = player.transform.rotation;

            float timeElapsed = 0;
            while (timeElapsed < animationTime)
            {
                timeElapsed += Time.deltaTime;


                #region GetNormalized Time

                var currentClip = player.AnimationController.GetCurrentAnimatorStateInfo(1);
                var nextClip = player.AnimationController.GetNextAnimatorStateInfo(1);

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
                

                if (timeElapsed < transitionTime)
                {
                    var repos = transform.position +
                                transform.forward * (distanceCurve.Evaluate(0.2f) * animationDistance) +
                                transform.up * (heightCurve.Evaluate(0.2f) * animationHeight);

                    player.transform.position = Vector3.Lerp(initialPlayerPos, repos, timeElapsed / transitionTime);
                }
                else
                {

                    if (normalizedTime != 0)
                    {
                        var repos = transform.position +
                                 transform.forward *
                                 (distanceCurve.Evaluate(normalizedTime) *
                                  animationDistance) +
                                 transform.up * (heightCurve.Evaluate(normalizedTime) *
                                                 animationHeight);

                        player.transform.position = repos;
                    }
                    
                }


                player.transform.rotation = Quaternion.Lerp(initialPlayerRot, transform.rotation,
                    Mathf.Clamp01(timeElapsed / transitionTime));

                yield return null;
            }

            if (changeState)
            {
                if (timeElapsed / animationTime > overrideTime)
                {
                    player.MovementController.ResetAnimator();
                    player.MovementController.ChangeState(stateIndex);

                    if (overrideAnimation)
                    {
                        player.AnimationController.Play(overrideAnimationName);
                    }
                }
            }

            #region Player Movement

            player.MovementController.ResetAnimator();
            player.DisabledPlayerMovement = false;
            player.CController.enabled = true;

            #endregion

            StopAllCoroutines();
            _triggerActionCoroutine = null;
            onActionEnd.Invoke();
            
        }
    }
}