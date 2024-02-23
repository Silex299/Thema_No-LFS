using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class AutomaticTrigger : MonoBehaviour
    {

        [Space(10)]
        [SerializeField, BoxGroup("Trigger Properties")] private string triggerTag = "Player";
        [SerializeField, BoxGroup("Trigger Properties")] private bool oneTime;
        [SerializeField, BoxGroup("Trigger Properties")] private bool isActive = true;



        [SerializeField, BoxGroup("Events"), Space(10)] private UnityEvent playAction;


        private bool _actionPerformed;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            if (oneTime && _actionPerformed) return;

            if (other.CompareTag(triggerTag))
            {
                if (!PlayerController.Instance.Player.PlayerController.enabled) return;

                playAction.Invoke();

                if (oneTime) _actionPerformed = true;
            }
        }

        public void SetTrigger(bool value)
        {
            isActive = value;
        }

    }
}
