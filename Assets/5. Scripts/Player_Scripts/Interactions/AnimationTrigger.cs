using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR

using UnityEditor;

#endif


namespace Player_Scripts.Interactions
{

    public class AnimationTrigger : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")] private string animationName;

        [SerializeField, BoxGroup("Animation")] private float animationDelay = 1.5f;

        /// <summary>
        /// Time needed to move the player to initial point of action;
        /// </summary>
        [SerializeField, BoxGroup("Movement")] private float initialDelay = 0.5f;
        [SerializeField, BoxGroup("Movement")] private float finalDelay = 0.3f;
        [SerializeField, BoxGroup("Movement")] private Transform initialPointOfAction;
        [SerializeField, BoxGroup("Movement")] private Transform finalPointOfAction;


        [SerializeField, BoxGroup("State")] private bool changeState;
        [SerializeField, BoxGroup("State"), ShowIf("changeState")] private PlayerMovementState ChangeState;

        private bool _isExecuting;
        private bool _initialMove;
        private bool _finalMove;

        private Transform _target;
        private Vector3 _initialPositon;
        private Vector3 _initialRotaion;
        private float _movementTimeElapsed;


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
                _initialPositon = _target.position;
                _initialRotaion = _target.eulerAngles;
                _movementTimeElapsed = 0;
                _initialMove = true;
            }
            else
            {

                _movementTimeElapsed += Time.deltaTime;

                float fraction = _movementTimeElapsed / initialDelay;

                _target.position = Vector3.Lerp(_initialPositon, initialPointOfAction.position, fraction);
                _target.eulerAngles = Vector3.Lerp(_initialRotaion, initialPointOfAction.eulerAngles, fraction);

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
                _initialPositon = _target.position;
                _movementTimeElapsed = 0;
                _finalMove = true;
            }
            else
            {
                _movementTimeElapsed += Time.deltaTime;
                float fraction = _movementTimeElapsed / finalDelay;

                _target.position = Vector3.Lerp(_initialPositon, finalPointOfAction.position, fraction);

                if (fraction >= 1)
                {
                    _finalMove = false;
                }
            }
        }


        private IEnumerator ExecuteAnimation()
        {

            if (_isExecuting) yield return null;

            PlayerMovementController player = PlayerMovementController.Instance;
            player.DiablePlayerMovement(true);
            player.player.CController.enabled = false;
            _isExecuting = true;
            _initialMove = true;

            _target = player.transform;
            InitialMove();
            //Move player

            yield return new WaitUntil(() => !_initialMove);


            //Play first aniamtion
            player.PlayAnimation(animationName, 0.4f, 1);

            yield return new WaitForSeconds(animationDelay);

            FinalMove();
            //Moveplayer
            yield return new WaitUntil(() => !_finalMove);

            //Reset everything
            _isExecuting = false;
            player.DiablePlayerMovement(false);
            player.player.CController.enabled = true;
        }

    }
}
