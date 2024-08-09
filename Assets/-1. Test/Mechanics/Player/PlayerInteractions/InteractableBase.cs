using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.PlayerInteractions
{
    public class InteractableBase : MonoBehaviour
    {

        [BoxGroup("Base")] public string interactionKey;
        [BoxGroup("Base")]  public bool isInteracting;

        public virtual void InteractionUpdate(PlayerV1 player)
        {
        }
        public virtual void InteractionFixedUpdate(PlayerV1 player)
        {
        }
        public virtual void InteractionLateUpdate(PlayerV1 player)
        {
        }
        
        public virtual void Interact(PlayerV1 player)
        {
        }

        public virtual void ExitInteraction(PlayerV1 player)
        {
            
        }
        
    }
}
