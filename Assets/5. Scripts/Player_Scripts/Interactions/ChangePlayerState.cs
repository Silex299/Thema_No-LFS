using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Interactions
{
    public class ChangePlayerState : MonoBehaviour
    {
        
        [BoxGroup, SerializeField] private int stateIndex;


        [BoxGroup, SerializeField] private bool overrideAnimation;
        [BoxGroup, SerializeField] private string overrideAnimationName;


        public void ChangeState(float smoothTime = 0.1f)
        {
            PlayerMovementController movementController = PlayerMovementController.Instance;
            
            movementController.ChangeState(stateIndex);

            if (overrideAnimation)
            { 
                movementController.PlayAnimation(overrideAnimationName, smoothTime, 0);
            }
            
        }

    }
}
