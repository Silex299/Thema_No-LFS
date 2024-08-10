using System.Collections.Specialized;
using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableBase : MonoBehaviour
    {
        public virtual void MovePlayer(float input, Transform playerTransform)
        {
            
        }

        public virtual void EngageClimbable(PlayerV1 player)
        {
        }


        public virtual void ExitClimbable()
        {
        }


        public virtual Vector3 GetMovementVector(Transform playerTransform, float speed)
        {
            throw new System.NotImplementedException();
        }

        public virtual Vector3 GetInitialConnectPoint(Transform playerTransform)
        {
            throw new System.NotImplementedException();
        }
        
    }
}
