using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    public class VolumeTrigger : MonoBehaviour
    {

        [SerializeField] private string triggerTag = "Player";


        [SerializeField] private bool isActive = true;
        [SerializeField] private UnityEvent enterTrigger;
        [SerializeField] private UnityEvent exitTrigger;



        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(triggerTag) || !isActive) return;

            enterTrigger?.Invoke();

        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(triggerTag) || !isActive) return;

            exitTrigger?.Invoke();
        }

        public void SetTrigger(bool value)
        {
            isActive = value;
        }

    }
}
