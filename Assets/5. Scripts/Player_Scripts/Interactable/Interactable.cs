using UnityEngine;
using Thema_Type;
using Sirenix.OdinInspector;

namespace Player_Scripts.Interactables
{

    public class Interactable : MonoBehaviour
    {

        [SerializeField, BoxGroup("Base Class")] protected bool canInteract;
        [SerializeField, BoxGroup("Base Class")] protected KeyInputType keyInputType;
        [SerializeField, BoxGroup("Base Class")] protected PlayerInteractionType interactionType;
        [SerializeField, BoxGroup("Base Class")] protected string keyString;

        protected bool _playerIsInTrigger;
        protected bool _isInteracting;

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!canInteract) return;

            if (other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _playerIsInTrigger = true;
                PlayerMovementController.Instance.SetInteractable(this);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _playerIsInTrigger = false;
                _isInteracting = false;
                PlayerMovementController.Instance.SetInteractable(null);
            }
        }


        public virtual PlayerInteractionType Interact()
        {
            return PlayerInteractionType.NONE;
        }

        public bool GetKey()
        {
            switch (keyInputType)
            {
                case KeyInputType.Key_Press:
                    return Input.GetButtonDown(keyString);
                case KeyInputType.Key_Hold:
                    return Input.GetButton(keyString);
                case KeyInputType.Key_Release:
                    return Input.GetButtonUp(keyString);
            }

            return false;
        }
    }

}