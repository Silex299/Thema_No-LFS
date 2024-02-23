using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class InputTrigger : MonoBehaviour
    {



        [SerializeField, BoxGroup("Properties")] private string triggerTag = "Player";
        [SerializeField, BoxGroup("Properties")] protected string inputActionName = "Interaction_1";
        [SerializeField] float activationTimeDelay = 2f;
        [SerializeField, BoxGroup("Properties")] protected bool oneTime;
        [SerializeField, Space(10)] private UnityEvent triggerEvent; 


        [SerializeField] protected bool isActive = true;

        protected bool actionPerformed;
        protected bool playerIsInTrigger;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (oneTime && actionPerformed) return;

            if (other.CompareTag(triggerTag))
            {
                playerIsInTrigger = true;
            }
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            if (oneTime && actionPerformed) return;

            if (other.CompareTag(triggerTag))
            {
                playerIsInTrigger = false;
            }
        }

        protected virtual void Update()
        {
            if (!isActive) return;
            if (!playerIsInTrigger || oneTime && actionPerformed) return;

            if (Input.GetButtonDown(inputActionName))
            {
                if (!PlayerController.Instance.Player.PlayerController.enabled) return;

                isActive = false;
                StartCoroutine(Action());
                if (oneTime) actionPerformed = true;
            }

        }

        protected virtual IEnumerator Action()
        {
            triggerEvent.Invoke();
            yield return new WaitForSeconds(activationTimeDelay);
            isActive = true;
        }

        public void Active(bool status)
        {
            isActive = status;
        }


    }
}
