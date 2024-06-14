using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
public class PlayerActionTrigger : MonoBehaviour
{
    
    
    [InfoBox("set player hip tag as Player_Main")]
    [InfoBox("disable the component by default")]
    
    [BoxGroup("Input")] public string engageInput;
    [BoxGroup("Input")] public string actionInput;
    [BoxGroup("Input")] public float actionTime;


    [BoxGroup("Movement")] public float transitionTime = 0.5f;
    [BoxGroup("Movement")] public Transform engagedTransform;


    [BoxGroup("Animation")] public string engageAnimation;
    [BoxGroup("Animation")] public string actionAnimation;

    [BoxGroup("Misc")] public float completeActionDelay;
    [BoxGroup("Misc")] public int maximumActionCount = 1;
    [BoxGroup("Misc")] public bool disEngageAfterAction = true;

    [BoxGroup("Misc"), ShowIf(nameof(disEngageAfterAction))]
    public float disEngageAfterActionDelay = 3f;


    [BoxGroup("Events")] public UnityEvent immediateAction;
    [BoxGroup("Events")] public UnityEvent completeAction;


    private bool _playerEngaged;
    private bool _actionCalled;
    private int _actionCount;
    private float _actionTime;

    private Coroutine _engagePlayerCoroutine;
    private Coroutine _disengagePlayerCoroutine;
    private Coroutine _playActionCoroutine;
    private Coroutine _triggerCoroutine;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            if(_triggerCoroutine!=null) StopCoroutine(_triggerCoroutine);
            _triggerCoroutine = StartCoroutine(TriggerCoroutine());
        }
    }


    private void Update()
    {
        
        if (!_playerEngaged)
        {
            //If input action is pressed down call 
            if (Input.GetButtonDown(engageInput) && _actionCount < maximumActionCount)
            {
                if (_disengagePlayerCoroutine == null && _playActionCoroutine == null)
                {
                    _engagePlayerCoroutine ??= StartCoroutine(EngagePlayer());
                }
            }
        }
        else
        {
            if (_actionCalled) return;
            
            if (_actionCount < maximumActionCount)
            {
                //If action input is down
                if (Input.GetButtonDown(actionInput))
                {
                    if (_engagePlayerCoroutine == null && _disengagePlayerCoroutine == null)
                    {
                        _playActionCoroutine ??= StartCoroutine(PlayAction());
                    }
                    
                }
            }
            
            //If input action is pressed up call disengage player
            if (Input.GetButtonUp(engageInput))
            {
                if (_engagePlayerCoroutine == null && _playActionCoroutine == null)
                {
                    _disengagePlayerCoroutine ??= StartCoroutine(DisEngagePlayer());
                }
            }

            
        }
    }

    
    
    private IEnumerator TriggerCoroutine()
    {
        enabled = true;
        
        yield return new WaitForSeconds(0.5f);
        
        enabled = false;
    }

    
    private IEnumerator EngagePlayer()
    {
        _playerEngaged = true;
        float timeElapsed = 0;
        Vector3 initialPosition = PlayerMovementController.Instance.transform.position;
        Quaternion initialRotation = PlayerMovementController.Instance.transform.rotation;


        //Disable Player movement
        PlayerMovementController.Instance.DisablePlayerMovement(true);
        //Disable player character controller
        PlayerMovementController.Instance.player.CController.enabled = false;

        //Play engage animation on layer 1
        PlayerMovementController.Instance.PlayAnimation(engageAnimation, 0.2f, 1);


        PlayerMovementController.Instance.transform.position = engagedTransform.position;

        while (timeElapsed < transitionTime)
        {
            timeElapsed += Time.deltaTime;

            // Move player to the engaged transform
            PlayerMovementController.Instance.transform.position = Vector3.Lerp(initialPosition,
                engagedTransform.position, timeElapsed / transitionTime);
            PlayerMovementController.Instance.transform.rotation = Quaternion.Lerp(initialRotation,
                engagedTransform.rotation, timeElapsed / transitionTime);

            yield return null;
        }

        _engagePlayerCoroutine = null;
    }

    private IEnumerator DisEngagePlayer()
    {
        print("Disengaging");
        
        //PLay default animation on layer 1
        PlayerMovementController.Instance.PlayAnimation("Default", 0.2f, 1);

        //play a exit delay
        yield return new WaitForSeconds(0.5f);

        //Reset player engaged and enable player movement
        _playerEngaged = false;
        enabled = false;
        
        PlayerMovementController.Instance.DisablePlayerMovement(false);
        //Enable player character controller
        PlayerMovementController.Instance.player.CController.enabled = true;
        
        _disengagePlayerCoroutine = null;
    }

    private IEnumerator PlayAction()
    {
        _actionCalled = true;
        float timeElapsed = 0;

        while (timeElapsed < actionTime)
        {
            timeElapsed += Time.deltaTime;

            if (Input.GetButtonUp(actionInput))
            {
                _actionCalled = false;
                _playActionCoroutine = null;
                yield break;
            }

            yield return null;
        }


        _actionCount++;
        immediateAction.Invoke();

        //Play action animation on layer 1
        PlayerMovementController.Instance.PlayAnimation(actionAnimation, 0.2f, 1);

        yield return new WaitForSeconds(completeActionDelay);

        completeAction.Invoke();
        _actionCalled = false;
        _playActionCoroutine = null;


        if (disEngageAfterAction)
        {
            yield return new WaitForSeconds(disEngageAfterActionDelay);
            
            //disengage
            _disengagePlayerCoroutine ??= StartCoroutine(DisEngagePlayer());
        }
    }
}