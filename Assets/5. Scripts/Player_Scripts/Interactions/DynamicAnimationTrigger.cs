using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Interactions
{
    public class DynamicAnimationTrigger : MonoBehaviour
    {
        [SerializeField, BoxGroup] private string animationName;
        [SerializeField, BoxGroup] private float movementTime;
        [SerializeField, BoxGroup] private bool disablePlayerMovement;

        private Transform _target;
        private bool _movePlayer;

        private IEnumerator PlayAnimation()
        {
            if (disablePlayerMovement)
            {
                PlayerMovementController.Instance.DisablePlayerMovement(true);
                PlayerMovementController.Instance.player.CController.enabled = false;
            }
            
            PlayerMovementController.Instance.PlayAnimation(animationName, 0.1f, 1);

            float timeElapsed = 0;

            Transform playerTransform = PlayerMovementController.Instance.transform;
            Vector3 initialPos = playerTransform.position;
            Quaternion initialRot = playerTransform.rotation;
            
            while (timeElapsed < movementTime)
            {
                timeElapsed += Time.deltaTime;

                float fraction = timeElapsed / movementTime;
                playerTransform.position = Vector3.Lerp(initialPos, transform.position, fraction);
                playerTransform.rotation = Quaternion.Slerp(initialRot, transform.rotation, fraction);
                
                yield return null;
            }

            yield return new WaitForSeconds(2f);
            
            PlayerMovementController.Instance.DisablePlayerMovement(false);
            PlayerMovementController.Instance.player.CController.enabled = true;
            
        }
        
        public void TriggerAnimation(bool moveWithAnimation)
        {
            if (moveWithAnimation)
            {
                StartCoroutine(PlayAnimation());
            }
            else
            {
                PlayerMovementController.Instance.PlayAnimation(animationName, 0.1f, 1);
            }
        }

       
    }
}
