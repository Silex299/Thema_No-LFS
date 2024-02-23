using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable_Items.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class PressurePlatform : MonoBehaviour
    {
        [SerializeField] private int maximumCapacity;
        [SerializeField] private int minimumThreshold;

        [SerializeField] private UnityEvent<bool> Trigger;

        private int _objectCount;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                _objectCount++;
            }


            Trigger?.Invoke(_objectCount >= minimumThreshold && _objectCount <= maximumCapacity);

        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                _objectCount--;
            }


            Trigger?.Invoke(_objectCount >= minimumThreshold && _objectCount <= maximumCapacity);

        }
    }
}
