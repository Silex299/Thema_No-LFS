using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.PlayerInteractions
{
    public class InteractableBox : InteractableBase
    {
        [SerializeField, BoxGroup("Box Properties")] private bool restrictX;
        [SerializeField, BoxGroup("Box Properties")] private bool restrictZ;


        private Vector3 _dragOffset;
        private static readonly int Push = Animator.StringToHash("Push");


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            
            var player = other.GetComponent<PlayerV1>();
            player.Interactable = this;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player_Main")) return;
            
            var player = other.GetComponent<PlayerV1>();
            ExitInteraction(player);
        }


        public override void InteractionFixedUpdate(PlayerV1 player)
        {
            Interact(player);
        }
        
        public override void Interact(PlayerV1 player)
        {
            if(player.InWater && !player.AtSurface) return;
            
            var playerTransform = player.transform;
            
            if (Input.GetButton(interactionKey))
            {
                if (!isInteracting)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    _dragOffset = playerTransform.position - transform.position;
                    isInteracting = true;
                }
                
                // ReSharper disable once PossibleNullReferenceException
                var desiredPosition = playerTransform.position - _dragOffset;
                var desiredLocalPosition = transform.parent.InverseTransformPoint(desiredPosition);
                
                if(restrictX)
                    desiredLocalPosition.x = 0;
                if (restrictZ)
                    desiredLocalPosition.z = 0;
                
                transform.localPosition = desiredLocalPosition;
                player.animator.SetBool(Push, true);
                
            }
            else
            {
                isInteracting = false;
                player.animator.SetBool(Push, false);
            }
            
        }


        public override void ExitInteraction(PlayerV1 player)
        {
            if (player.Interactable == this)
            {
                player.Interactable = null;
                player.animator.SetBool(Push, false);
            }
            isInteracting = false;
        }
    }
}
