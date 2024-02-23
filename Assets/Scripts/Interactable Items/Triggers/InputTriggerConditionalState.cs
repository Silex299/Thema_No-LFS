using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactable_Items.Triggers
{
    public class InputTriggerConditionalState : InputTrigger
    {

        [SerializeField, BoxGroup("Properties")] private PlayerController.PlayerStates activeState;
        protected override void Update()
        {
            if (!isActive) return;

            if (!playerIsInTrigger || oneTime && actionPerformed) return;

            if (Input.GetButton(inputActionName))
            {
                if (PlayerController.Instance.initState != activeState) return;

                if (!PlayerController.Instance.Player.PlayerController.enabled) return;

                isActive = false;
                StartCoroutine(Action());
                if (oneTime) actionPerformed = true;
            }
        }
    }
}
