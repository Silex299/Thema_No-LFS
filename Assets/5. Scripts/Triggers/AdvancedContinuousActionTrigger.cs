using System.Collections;
using Managers;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class AdvancedContinuousActionTrigger : MonoBehaviour
    {
        [BoxGroup("Input")] public string engageInput;
        [BoxGroup("Input")] public string actionInput;
        [BoxGroup("Input")] public float actionTriggerTime;

        [BoxGroup("Animation")] public bool simpleEngage;
        [BoxGroup("Animation")] public AdvancedCurvedAnimation engageAnim;
        [BoxGroup("Animation")] public string actionAnimName; //MAY BE CHANGE LATER TO MORE DYNAMIC


        [BoxGroup("Events")] public float actionDelay;
        [BoxGroup("Events")] public UnityEvent actionEvent;


        private Coroutine _engagedCoroutine;
        private bool _playerInTrigger;

        private void OnTriggerStay(Collider other)
        {
            if(!enabled) return;
            if (!other.CompareTag("Player_Main")) return;

            if (_playerInTrigger) return;
            _playerInTrigger = true;

            if (_engagedCoroutine != null) StopCoroutine(ResetTrigger());
            StartCoroutine(ResetTrigger());
        }


        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.5f);
            _playerInTrigger = false;
        }

        private void Update()
        {
            if (!_playerInTrigger) return;

            if (Input.GetButtonDown(engageInput))
            {
                _engagedCoroutine ??= StartCoroutine(Engage());
            }
        }


        private IEnumerator Engage()
        {
            var player = PlayerMovementController.Instance.player;

            player.CanRotate = false;
            player.CController.enabled = false;
            player.DisabledPlayerMovement = true;

            if (simpleEngage)
            {
                yield return engageAnim.SimpleAnim(player.AnimationController, player.transform, transform);
            }
            else
            {
                yield return engageAnim.PlayAnim(player.AnimationController, player.transform, transform);
            }


            float timeElapsed = 0;

            var uiManager = UIManager.Instance;
            uiManager.UpdateActionFillPos(transform.position, new Vector3(0, 2f, 0f));

            //Action
            while (!Input.GetButtonUp(engageInput))
            {
                if (Input.GetButton(actionInput))
                {
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    timeElapsed = 0;
                }

                uiManager.UpdateActionFill(Mathf.Clamp01(timeElapsed / actionTriggerTime), actionInput);
                
                if (timeElapsed >= actionTriggerTime)
                {
                    player.AnimationController.CrossFade(actionAnimName, 0.2f, 1);
                    yield return new WaitForSeconds(actionDelay);
                    actionEvent.Invoke();
                    break;
                }

                yield return null;
            }


            player.AnimationController.CrossFade("Default", 0.1f, 1);
            player.DisabledPlayerMovement = false;
            player.CController.enabled = true;
            player.CanRotate = true;
            _engagedCoroutine = null;
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
        /// <summary>
        /// Plays animation and follows the curves
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="target"></param>
        /// <param name="triggerTransform"></param>
        /// <returns></returns>
        public IEnumerator PlayAnim(Animator animator, Transform target, Transform triggerTransform)
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


                if (timeElapsed < transitionTime)
                {
                    var repos = triggerTransform.position +
                                triggerTransform.forward * (distanceCurve.Evaluate(0.2f) * animationDistance) +
                                triggerTransform.up * (heightCurve.Evaluate(0.2f) * animationHeight);

                    target.position = Vector3.Lerp(initialPlayerPos, repos, timeElapsed / transitionTime);
                }
                else
                {
                    if (normalizedTime != 0)
                    {
                        var repos = triggerTransform.position +
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
        public IEnumerator SimpleAnim(Animator animator, Transform target, Transform triggerTransform)
        {
            animator.CrossFade(animationName, transitionTime, 1);

            Vector3 initialPlayerPos = target.position;
            Quaternion initialPlayerRot = target.rotation;

            float timeElapsed = 0;

            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;

                target.position = Vector3.Lerp(initialPlayerPos, triggerTransform.position,
                    timeElapsed / transitionTime);
                target.rotation = Quaternion.Lerp(initialPlayerRot, triggerTransform.rotation,
                    timeElapsed / transitionTime);

                yield return null;
            }
        }

    }
}