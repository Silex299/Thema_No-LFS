using UnityEngine;

namespace NPC
{

    public class AIVision_2 : AIVision
    {

        protected override void Update()
        {
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(targetTag) || isDisabled) return;

            _playerInRange = false;
            targetFound = false;

        }
    }

}
