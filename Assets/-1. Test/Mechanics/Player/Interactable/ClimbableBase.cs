using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableBase : MonoBehaviour
    {
        public bool Engaged { get; set; }

        public virtual void MovePlayer(float input, Transform playerTransform)
        {
            
        }

        public virtual void EngageClimbable(PlayerV1 player)
        {
            Engaged = true;
        }


        public virtual void ExitClimbable()
        {
            Engaged = false;
        }
        
    }
}
