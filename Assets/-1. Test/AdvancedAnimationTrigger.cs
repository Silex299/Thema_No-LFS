using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using Triggers;
using UnityEngine;
using UnityEngine.Events;

public class AdvancedAnimationTrigger : MonoBehaviour
{
    //TODO: REMOVE
    public Animator animator;

    [BoxGroup("Input")] public string inputString;
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

    [BoxGroup("State"), ShowIf(nameof(changeState))]
    public float overrideTime;


    [BoxGroup("Misc")] public TriggerCondition[] conditions;

    [BoxGroup("Event")] public UnityEvent onTriggerEnter;
    [BoxGroup("Event")] public UnityEvent onTriggerExit;
    [BoxGroup("Event")] public UnityEvent onActionStart;
    [BoxGroup("Event")] public UnityEvent onActionEnd;


    private Coroutine _playerInTriggerCoroutine;

    #region Editor

    [Range(0, 1), OnValueChanged(nameof(SetNormalisedTime)), BoxGroup("Movement")]
    public float normalisedTime;

    public void SetNormalisedTime()
    {
        animator.Play(animationName, 1, normalisedTime);
        animator.Update(0);

        Vector3 repos = transform.position +
                        transform.forward * distanceCurve.Evaluate(normalisedTime) * animationDistance +
                        transform.up * heightCurve.Evaluate(normalisedTime) * animationHeight;

        animator.transform.position = repos;
    }

    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            onTriggerEnter.Invoke();
            _playerInTriggerCoroutine ??= StartCoroutine(PlayerInTrigger(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            onTriggerExit.Invoke();

            if (_playerInTriggerCoroutine != null)
            {
                StopCoroutine(_playerInTriggerCoroutine);
                _playerInTriggerCoroutine = null;
            }
        }
    }


    private IEnumerator PlayerInTrigger(Collider other)
    {
        bool result = false;
        foreach (var condition in conditions)
        {
            result = condition.Condition(other);

            if (!result)
            {
                break;
            }

            yield return null;
        }


        if (result)
        {
            Trigger();
        }
    }


    public void Trigger()
    {
        StartCoroutine(TriggerAnimation());
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
        float timeElapsed = 0;
        while (timeElapsed < animationTime)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < transitionTime)
            {
                var repos = transform.position +
                            transform.forward * (distanceCurve.Evaluate(0.2f) * animationDistance) +
                            transform.up * (heightCurve.Evaluate(0.2f) * animationHeight);

                player.transform.position = Vector3.Lerp(initialPlayerPos, repos, timeElapsed / transitionTime);
            }
            else
            {
                var repos = transform.position +
                            transform.forward * (distanceCurve.Evaluate(Mathf.Clamp01(timeElapsed / animationTime)) *
                                                 animationDistance) +
                            transform.up * (heightCurve.Evaluate(Mathf.Clamp01(timeElapsed / animationTime)) *
                                            animationHeight);
                player.transform.position = repos;
            }


            if (changeState)
            {
                if (timeElapsed / animationTime > overrideTime)
                {
                    player.MovementController.ResetAnimator();
                    player.MovementController.ChangeState(stateIndex);
                }
            }
            yield return null;
        }


        #region Player Movement

        player.MovementController.ResetAnimator();
        player.DisabledPlayerMovement = false;
        player.CController.enabled = true;

        #endregion

        StopAllCoroutines();
        _playerInTriggerCoroutine = null;
        onActionEnd.Invoke();
    }
}