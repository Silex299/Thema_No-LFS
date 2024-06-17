using System.Collections;
using Misc;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR

using UnityEditor;

#endif


// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions.Animation
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

        
        [Button("Reset", ButtonSizes.Large), GUIColor(0.6f, 0.1f, 0f)]
        
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



        public void Execute()
        {
            if (_isExecuting) return;

            StartCoroutine(ExecuteAnimation());
        }

        private IEnumerator ExecuteAnimation()
        {

            _isExecuting = true;
            PlayerMovementController.Instance.PlayAnimation(animationName, 0.2f, 1);
            
            yield return PlayerMover.MoveCoroutine(initialPointOfAction, initialDelay, true, false, false);
            
            


            yield return new WaitForSeconds(animationDelay);

            //ANIMATION AND STATE
            if (changeState)
            {
                if (overrideAnimation)
                {
                    PlayerMovementController.Instance.ChangeState(stateIndex, overrideAnimationName);
                }
                else
                {
                    PlayerMovementController.Instance.ChangeState(stateIndex);
                }
                
            }

            if (finalPointOfAction)
            {
                //MOVE TO FINAL POINT using PlayerMover
                yield return PlayerMover.MoveCoroutine(finalPointOfAction, finalDelay, true, true, true);
            }

            
            PlayerMovementController.Instance.DisablePlayerMovement(false);
            PlayerMovementController.Instance.player.CController.enabled = true;
            PlayerMovementController.Instance.ResetAnimator();
            
            //RESET
            _isExecuting = false;

        }

    }
}
