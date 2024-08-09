using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Player_Interactable
{
    public class InteractableBox : InteractableBase
    {
        
        
        [SerializeField, BoxGroup("Box Properties")] private bool restrictX;
        [SerializeField, BoxGroup("Box Properties"), HideIf(nameof(restrictX))] private Vector2 xBound;
        [SerializeField, BoxGroup("Box Properties")] private bool restrictZ;
        [SerializeField, BoxGroup("Box Properties"), HideIf(nameof(restrictZ))] private Vector2 zBound;


        private Vector3 _dragOffset;
        private static readonly int Push = Animator.StringToHash("Push");


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            
            if (!restrictZ)
            {
                var pos1 = new Vector3(0, 0, zBound.x);
                pos1 = transform.parent.TransformPoint(pos1);
                var pos2 = new Vector3(0, 0, zBound.y);
                pos2 = transform.parent.TransformPoint(pos2);
                
                Gizmos.DrawLine(pos1, pos2);
            }
            
            if (!restrictX)
            {

                var pos1 = new Vector3(xBound.x, 0, 0);
                pos1 = transform.parent.TransformPoint(pos1);
                var pos2 = new Vector3(xBound.y, 0, 0);
                pos2 = transform.parent.TransformPoint(pos2);
                
                Gizmos.DrawLine(pos1, pos2);
            }

        }

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
                if (!player.IsInteracting)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    _dragOffset = playerTransform.position - transform.position;
                    player.IsInteracting = true;
                }
                
                // ReSharper disable once PossibleNullReferenceException
                var desiredPosition = playerTransform.position - _dragOffset;
                var desiredLocalPosition = transform.parent.InverseTransformPoint(desiredPosition);
                
                //Constraint the box to the x and z axis
                if (restrictX)
                {
                    desiredLocalPosition.x = 0;
                }
                else
                {
                    desiredLocalPosition.x = Mathf.Clamp(desiredLocalPosition.x, xBound.x, xBound.y);
                }

                if (restrictZ)
                {
                    desiredLocalPosition.z = 0;
                }
                else
                {
                    desiredLocalPosition.z = Mathf.Clamp(desiredLocalPosition.z, zBound.x, zBound.y);
                }

                transform.localPosition = desiredLocalPosition;
                
                player.animator.SetBool(Push, true);
                
            }
            else
            {
                player.IsInteracting = false;
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
            player.IsInteracting = false;
        }
    }
}
