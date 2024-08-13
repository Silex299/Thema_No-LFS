using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Controllers
{
    public class Controller : MonoBehaviour
    {

        [BoxGroup("Base Properties")] public int stateIndex;
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        public virtual void ControllerEnter(PlayerV1 player)
        {
            player.animator.SetInteger(StateIndex, stateIndex);
        }

        public virtual void ControllerExit(PlayerV1 player)
        {
            
        }
        
        public virtual void ControllerUpdate(PlayerV1 player)
        {
            
        }

        public virtual void ControllerFixedUpdate(PlayerV1 player)
        {
            
        }

        public virtual void ControllerLateUpdate(PlayerV1 player)
        {
            
        }
    }
}
