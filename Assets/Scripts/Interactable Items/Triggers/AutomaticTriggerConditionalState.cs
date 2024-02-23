using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactable_Items.Triggers
{
    public class AutomaticTriggerConditionalState : AutomaticTrigger
    {
        [SerializeField, BoxGroup("Trigger Properties")]
        private PlayerController.PlayerStates activationState;
        protected override void OnTriggerEnter(Collider other)
        {
            if (PlayerController.Instance.initState == activationState)
            {
                base.OnTriggerEnter(other);
            }
        }
    }
}
