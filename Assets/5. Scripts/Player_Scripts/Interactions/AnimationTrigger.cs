using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR

using UnityEditor;

#endif


// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions
{

    public class AnimationTrigger : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")] private string animationName;

        [SerializeField, BoxGroup("Animation")] private float animationDelay = 1.5f;


        [SerializeField, BoxGroup("Movement")] private float initialDelay = 0.5f;
        [SerializeField, BoxGroup("Movement")] private float finalDelay = 0.3f;
        [SerializeField, BoxGroup("Movement")] private Transform initialPointOfAction;
        [SerializeField, BoxGroup("Movement")] private Transform finalPointOfAction;


        [SerializeField, BoxGroup("State")] private bool changeState;
        [SerializeField, BoxGroup("State"), ShowIf("changeState")] private int stateIndex;
        [SerializeField, BoxGroup("State")] private bool overrideAnimation;
        [SerializeField, BoxGroup("State"), ShowIf("overrideAnimation")] private string overrideAnimationName;

        private bool _isExecuting;
        private bool _initialMove;
        private bool _finalMove;

        private Transform _target;
        private float _timeElapsed;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;



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
                player.player.AnimationController.Play(animationName, 1, 0);
                _preview = true;


                var tran = player.transform;

                _initialPlayerPos = tran.position;
                _initialPlayerRot = tran.eulerAngles;

                tran.position = initialPointOfAction.position;
                tran.rotation = initialPointOfAction.rotation;

                Invoke("Reset", animationDelay);
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


        private void LateUpdate()
        {
            if (!_isExecuting) return;

            if (_initialMove)
            {
                InitialMove();
            }

            if (_finalMove)
            {
                FinalMove();
            }

        }


        private void InitialMove()
        {
            if (!_initialMove)
            {
                _initialMove = true;

                _timeElapsed = 0;
                _initialPosition = _target.position;
                _initialRotation = _target.rotation;
            }
            else
            {
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / initialDelay;

                _target.position = Vector3.Lerp(_initialPosition, initialPointOfAction.position, fraction);
                _target.rotation = Quaternion.Slerp(_initialRotation, initialPointOfAction.rotation, fraction);


                if (fraction >= 1)
                {
                    _initialMove = false;
                }

            }
        }


        private void FinalMove()
        {

            if (!_finalMove)
            {
                _finalMove = true;

                _timeElapsed = 0;
                _initialPosition = _target.position;
                _initialRotation = _target.rotation;
            }
            else
            {
                if (!finalPointOfAction)
                {
                    _finalMove = false;
                    return;
                }
                
                _timeElapsed += Time.deltaTime;

                float fraction = _timeElapsed / finalDelay;

                _target.position = Vector3.Lerp(_initialPosition, finalPointOfAction.position, fraction);
                _target.rotation = Quaternion.Slerp(_initialRotation, finalPointOfAction.rotation, fraction);


                if (fraction >= 1)
                {
                    _finalMove = false;
                }

            }
        }


        public void Execute()
        {
            if (_isExecuting) return;

            StartCoroutine(ExecuteAnimation());
        }

        private IEnumerator ExecuteAnimation()
        {

            _isExecuting = true;
            PlayerMovementController player = PlayerMovementController.Instance;
            _target = player.transform;
            player.DisablePlayerMovement(true);
            player.player.CController.enabled = false;

            InitialMove();

            yield return new WaitUntil(() => !_initialMove);



            //ANIMATION AND STATE
            if (changeState)
            {
                if (overrideAnimation)
                {
                    player.ChangeState(stateIndex, overrideAnimationName);
                }
                else
                {
                    player.ChangeState(stateIndex);
                }
                
            }

            player.PlayAnimation(animationName, 0.2f, 1);

            yield return new WaitForSeconds(animationDelay);


            FinalMove();


            yield return new WaitUntil(() => !_finalMove);


            //RESET
            _isExecuting = false;
            _target = null;
            player.ResetAnimator();
            player.DisablePlayerMovement(false);
            player.player.CController.enabled = true;


        }

    }
}
