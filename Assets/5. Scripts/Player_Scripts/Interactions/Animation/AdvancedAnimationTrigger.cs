using System.Collections;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player_Scripts.Interactions.Animation
{
    public class AdvancedAnimationTrigger : MonoBehaviour
    {
        
        [FoldoutGroup("Animation")] public AdvancedCurvedAnimation animationInfo;
        [FoldoutGroup("Animation")] public float animationWidth;
        [FoldoutGroup("Animation")] public float resetJumpDelay = 1;
        

        [FoldoutGroup("State")] public bool changeState;

        [FoldoutGroup("State"), ShowIf(nameof(changeState))]
        public int stateIndex;

        [FoldoutGroup("State"), ShowIf(nameof(changeState)), Range(0, 1)]
        public float overrideTime = 0.8f;

        [FoldoutGroup("State"), ShowIf(nameof(changeState))]
        public bool overrideAnimation;

        [FoldoutGroup("State"), ShowIf(nameof(overrideAnimation))]
        public string overrideAnimationName;

        [FoldoutGroup("Event")] public UnityEvent onActionStart;
        [FoldoutGroup("Event")] public UnityEvent onActionEnd;


        private Coroutine _triggerActionCoroutine;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position - transform.right * animationWidth,
                transform.position + transform.right * animationWidth);
        }

        [ContextMenu("Play")]
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
            player.MovementController.ResetAnimator();
            
            
            
            #endregion

            TimedAction timedAction = null; 
            if (changeState)
            {
                void ChangeState()
                {
                    player.MovementController.ResetAnimator();
                    player.MovementController.ChangeState(stateIndex);

                    if (overrideAnimation)
                    {
                        player.AnimationController.Play(overrideAnimationName);
                    }
                }
                
                timedAction = new TimedAction(overrideTime, ChangeState);
            }

            yield return animationInfo.PlayAnim(player.AnimationController, player.transform, transform, animationWidth,
                timedAction);

            #region Player Movement
            
            player.MovementController.ResetAnimator();
            player.DisabledPlayerMovement = false;
            player.CController.enabled = true;

            #endregion

            onActionEnd.Invoke();
            _triggerActionCoroutine = null;
            StopAllCoroutines();



            yield return new WaitForSeconds(resetJumpDelay);
            


        }
    }
}