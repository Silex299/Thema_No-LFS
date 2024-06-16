using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions.Animation
{
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

    
    
        #region Editor

#if UNITY_EDITOR


        //TODO: REMOVE ALL

        private bool _preview;
        private PlayerMovementController player;
        private Vector3 _initialPlayerPos;
        private Vector3 _initialPlayerRot;

        [Button("Preview", ButtonSizes.Large), GUIColor(0.1f, 0.6f, 0f)]
        private void Preview()
        {
            if (!_preview)
            {
                EditorApplication.update += Preview;
                player = FindObjectOfType<PlayerMovementController>();
                player.player.AnimationController.Play(engageAnimation, 1, 0);
                _preview = true;


                var tran = player.transform;

                _initialPlayerPos = tran.position;
                _initialPlayerRot = tran.eulerAngles;

                tran.position = engagedTransform.position;
                tran.rotation = engagedTransform.rotation;

                Invoke("Reset", 3);
            }
            else
            {
                player.player.AnimationController.Update(Time.deltaTime);
            }
        }

        private void Reset()
        {
            _preview = false;
            Transform trans = player.transform;
            trans.position = _initialPlayerPos;
            trans.eulerAngles = _initialPlayerRot;

            player.PlayAnimation("Default", 1);
            player.player.AnimationController.Update(0);

            EditorApplication.update -= Preview;
        }


#endif

        #endregion
    
    
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
                    if (Input.GetButton(actionInput))
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
            //Play engage animation on layer 1
            PlayerMovementController.Instance.PlayAnimation(engageAnimation, 0.2f, 1);
            print(Time.time);
            
            yield return PlayerMover.MoveCoroutine(engagedTransform, transitionTime, true, false, false);
            
            print(Time.time);
            _engagePlayerCoroutine = null;

        }

        private IEnumerator DisEngagePlayer()
        {
            print("Disengaging");
        
            //PLay default animation on layer 1
            PlayerMovementController.Instance.PlayAnimation("Default", 0.5f, 1);

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
}