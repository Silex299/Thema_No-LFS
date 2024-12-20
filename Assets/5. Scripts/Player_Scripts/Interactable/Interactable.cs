using UnityEngine;
using Thema;
using Sirenix.OdinInspector;
using System.Collections;

namespace Player_Scripts.Interactables
{

    public class Interactable : MonoBehaviour
    {

        [SerializeField, BoxGroup("Base Class")] protected bool canInteract;
        [SerializeField, BoxGroup("Base Class")] protected bool continuousInteraction;
        [SerializeField, BoxGroup("Base Class")] protected KeyInputType keyInputType;
        [SerializeField, BoxGroup("Base Class")] protected PlayerInteractionType interactionType;
        [SerializeField, BoxGroup("Base Class")] protected string keyString;

        protected bool playerIsInTrigger;
        protected bool isInteracting;



        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!canInteract) return;

            if (other.CompareTag("Player_Main"))
            {
                playerIsInTrigger = true;
                PlayerMovementController.Instance.SetIntractable(this);
            }
        }

        private Coroutine _resetTrigger;

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!continuousInteraction) return;

            if (other.CompareTag("Player_Main"))
            {
                if (_resetTrigger != null)
                {
                    StopCoroutine(_resetTrigger);
                }
                _resetTrigger = StartCoroutine(ResetTrigger());
            }

        }


        protected virtual IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            playerIsInTrigger = false;

            if(PlayerMovementController.Instance.player.interactable == this)
            {
                PlayerMovementController.Instance.player.interactable = null;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                playerIsInTrigger = false;
                isInteracting = false;
                PlayerMovementController.Instance.SetIntractable(null);
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