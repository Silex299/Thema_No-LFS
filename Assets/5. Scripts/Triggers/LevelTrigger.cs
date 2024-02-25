using UnityEngine;
using Player_Scripts;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LevelTrigger : MonoBehaviour
{

    [SerializeField, BoxGroup("Trigger Params")] private bool triggerPulled;
    [SerializeField, BoxGroup("Trigger Params")] private bool oneTime;
    [SerializeField, BoxGroup("Trigger Params")] private Animator leverAnimator;
    [SerializeField, BoxGroup("Trigger Params")] private float actionDelay = 0;
    [SerializeField, BoxGroup("Trigger Params")] private float leverPullDelay = 1;

    [SerializeField, BoxGroup("Player Movement")] private Transform forwardPosition;
    [SerializeField, BoxGroup("Player Movement")] private Transform backwardPosition;
    [SerializeField, BoxGroup("Player Movement")] private float movementSpeed;
    [SerializeField, BoxGroup("Player Movement")] private float rotationSpeed;


    [SerializeField, BoxGroup("Evenets")] private UnityEvent triggerPullAction;
    [SerializeField, BoxGroup("Evenets")] private UnityEvent triggerPushAction;

    [SerializeField, BoxGroup("Sounds")] private AudioSource soundSource;
    [SerializeField, BoxGroup("Sounds")] private AudioClip triggerSound;


    private bool _playerIsInTrigger;
    private bool _triggerEngaged;
    private float _lastTriggerTime;

    /// <summary>
    /// If player is facing forward direction
    /// </summary>

    private bool _forwardDirection;
    private Coroutine _trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (oneTime && triggerPulled) return;
        if (other.CompareTag("Player_Main"))
        {
            _playerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _playerIsInTrigger = false;
        }
    }


    private void Start()
    {
        //Set Initial State
        if (triggerPulled)
        {
            leverAnimator.Play("Trigger");
        }
        else
        {
            leverAnimator.Play("Trigger Inverse");
        }
    }

    private void Update()
    {
        if (!_playerIsInTrigger) return;


        if (Input.GetButtonUp("e"))
        {
            if (_lastTriggerTime + 0.8f > Time.time)
            {
                Invoke("Reset", _lastTriggerTime + 0.8f - Time.time);
            }
            else
            {
                Reset();
            }
        }

        if (Input.GetButton("e"))
        {
            if (oneTime && triggerPulled) return;

            var player = PlayerMovementController.Instance;

            if (!_triggerEngaged)
            {
                _triggerEngaged = true;
                _lastTriggerTime = Time.time;
                player.DiablePlayerMovement(true);

                _forwardDirection = CheckPlayerDirection();
                if (_forwardDirection)
                {
                    if (!triggerPulled)
                    {
                        player.PlayAnimation("Trigger_Pull_Enter", 0.2f, 1);
                    }
                    else
                    {
                        player.PlayAnimation("Trigger_Push_Enter", 0.2f, 1);
                    }

                }
                else
                {
                    if (!triggerPulled)
                    {
                        player.PlayAnimation("Trigger_Pull_Enter_Inv", 0.2f, 1);
                    }
                    else
                    {

                        player.PlayAnimation("Trigger_Push_Enter_Inv", 0.2f, 1);
                    }
                }

            }
            else
            {
                if (_lastTriggerTime + 0.5f > Time.time) return;

                var input = Input.GetAxis("Horizontal");

                if (input < 0 && !triggerPulled)
                {
                    _lastTriggerTime = Time.time;
                    triggerPulled = true;

                    player.SetAnimationTrigger("Trigger");
                    leverAnimator.Play("Trigger");

                    if (_trigger == null)
                    {
                        _trigger = StartCoroutine(Trigger());
                    }
                }

                if (input > 0 && triggerPulled)
                {
                    _lastTriggerTime = Time.time;
                    triggerPulled = false;
                    player.SetAnimationTrigger("Trigger");
                    leverAnimator.Play("Trigger Inverse");

                    if (_trigger == null)
                    {
                        _trigger = StartCoroutine(Trigger());
                    }
                }


            }


        }
    }

    private void LateUpdate()
    {
        if (oneTime && triggerPulled) return;

        if (_triggerEngaged)
        {
            var player = PlayerMovementController.Instance;
            if (_forwardDirection)
            {
                //Move to position 1
                var playerTransform = PlayerMovementController.Instance.transform;

                player.transform.position = Vector3.Lerp(playerTransform.position, forwardPosition.position, Time.deltaTime * movementSpeed);
                player.transform.rotation = Quaternion.Lerp(playerTransform.rotation, forwardPosition.rotation, Time.deltaTime * rotationSpeed);
            }
            else
            {

                var playerTransform = player.transform;

                playerTransform.position = Vector3.Lerp(playerTransform.position, backwardPosition.position, Time.deltaTime * movementSpeed);
                playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, backwardPosition.rotation, Time.deltaTime * rotationSpeed);
                //move to position 2
            }
        }
    }

    private IEnumerator Trigger()
    {
        yield return new WaitForSeconds(leverPullDelay);

        if (soundSource)
        {
            soundSource.PlayOneShot(triggerSound);
        }

        yield return new WaitForSeconds(actionDelay);

        if (triggerPulled)
        {
            triggerPullAction?.Invoke();
        }
        else
        {
            triggerPushAction?.Invoke();
        }

        _trigger = null;
    }

    private void Reset()
    {
        _triggerEngaged = false;
        PlayerMovementController.Instance.PlayAnimation("Default", 0.5f, 1);
        PlayerMovementController.Instance.DiablePlayerMovement(false);
    }

    private bool CheckPlayerDirection()
    {
        Vector3 v1 = transform.forward;
        Vector3 v2 = PlayerMovementController.Instance.transform.forward;

        v1.y = 0;
        v2.y = 0;

        var angle = Vector3.Angle(v1, v2);



        return angle < 90;
    }


    public void ChangeState(bool pulled)
    {
        if (triggerPulled == pulled) return;

        triggerPulled = pulled;
        if (pulled)
        {
            leverAnimator.Play("Trigger");
        }
        else
        {
            leverAnimator.Play("Trigger Inverse");
        }
    }

}
