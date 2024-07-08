using Sirenix.OdinInspector;
using UnityEngine;

namespace Player_Scripts.Interactables
{
    public class DraggableCart : DragBox
    {

        [SerializeField, BoxGroup("Cart Property")] private float wheelSpeed;
        [SerializeField, BoxGroup("Cart Property")] private Transform[] wheelTransforms;

        [SerializeField, BoxGroup("Cart Property")] private Health.CartHealth cart;
        

        public void Reset()
        {
            _isInteracting = false;
            _playerIsInTrigger = false;

            if (!cart.gameObject.activeInHierarchy)
            {
                cart.gameObject.SetActive(true);
                cart.Reset();
            }
        }

        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }


    }


}