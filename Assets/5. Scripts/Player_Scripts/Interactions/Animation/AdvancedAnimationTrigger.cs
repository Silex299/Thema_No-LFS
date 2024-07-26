using System.Collections;
using Sirenix.OdinInspector;
using Thema_Type;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Interactions.Animation
{
    public class AdvancedAnimationTrigger : MonoBehaviour
    {
        
        [FoldoutGroup("Animation")] public AdvancedCurvedAnimation animationInfo;
        [FoldoutGroup("Animation")] public float animtionWidth;
        

        [FoldoutGroup("State")] public bool changeState;

        [FoldoutGroup("State"), ShowIf(nameof(changeState))]
        public int stateIndex;

        [FoldoutGroup("State"), ShowIf(nameof(changeState)), Range(0, 1)]
        public float overrideTime;

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
            Gizmos.DrawLine(transform.position - transform.right * animtionWidth,
                transform.position + transform.right * animtionWidth);
        }

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

            #endregion

            yield return animationInfo.PlayAnim(player.AnimationController, player.transform, transform, animtionWidth);

            if (changeState)
            {
                player.MovementController.ResetAnimator();
                player.MovementController.ChangeState(stateIndex);

                if (overrideAnimation)
                {
                    player.AnimationController.Play(overrideAnimationName);
                }
            }

            #region Player Movement

            player.MovementController.ResetAnimator();
            player.DisabledPlayerMovement = false;
            player.CController.enabled = true;

            #endregion

            StopAllCoroutines();
            _triggerActionCoroutine = null;
            onActionEnd.Invoke();
            
        }
    }
}