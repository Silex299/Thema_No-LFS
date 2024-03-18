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


        [SerializeField, BoxGroup("Movement")] private float initalDelay = 0.5f;
        [SerializeField, BoxGroup("Movement")] private float finalDelay = 0.3f;
        [SerializeField, BoxGroup("Movement")] private Transform initialPointOfAction;
        [SerializeField, BoxGroup("Movement")] private Transform finalPointOfAction;

        [SerializeField, BoxGroup("Movement")] private float movementSpeed;


        [SerializeField, BoxGroup("State")] private bool changeState;
        [SerializeField, BoxGroup("State"), ShowIf("changeState")] private PlayerMovementState exitState;
        [SerializeField, BoxGroup("State"), ShowIf("changeState")] private int stateIndex;

        private bool _isExecuting;
        private bool _initialMove;
        private bool _finalMove;

        private Transform _target;
        private float _fraction;



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
            }
            else
            {
                _fraction += Time.deltaTime * movementSpeed;

                _target.position = Vector3.Lerp(_target.position, initialPointOfAction.position, _fraction);
                _target.rotation = Quaternion.Slerp(_target.rotation, initialPointOfAction.rotation, _fraction);

            }
        }


        private void FinalMove()
        {

            if (!_finalMove)
            {
                if (!finalPointOfAction)
                {
                    return;
                }

                _finalMove = true;
            }
            else
            {
                _fraction += Time.deltaTime * movementSpeed;

                _target.position = Vector3.Lerp(_target.position, finalPointOfAction.position, _fraction);
                _target.rotation = Quaternion.Slerp(_target.rotation, finalPointOfAction.rotation, _fraction);

            }
        }


        public void Execute()
        {
            if (_isExecuting) return;

            StartCoroutine(ExecuteAnimation());
        }

        private IEnumerator ExecuteAnimation()
        {

            PlayerMovementController player = PlayerMovementController.Instance;

            player.ChangeState(-1);

            player.player.CController.enabled = false;
            player.DiablePlayerMovement(true);
            player.transform.position = initialPointOfAction.position;
            player.transform.rotation = initialPointOfAction.rotation;

            player.PlayAnimation(animationName, 0.2f, 1);
            yield return new WaitForSeconds(animationDelay);





            player.DiablePlayerMovement(false);
            player.player.CController.enabled = true;

        }

    }
}
