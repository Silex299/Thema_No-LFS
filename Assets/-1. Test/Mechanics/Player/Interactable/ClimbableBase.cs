using System.Collections.Specialized;
using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableBase : MonoBehaviour
    {
        
        
        public virtual void UpdateClimbable(PlayerV1 player)
        {
            
        }

        
        public virtual Vector3 GetMovementVector(Transform playerTransform, float speed)
        {
            throw new System.NotImplementedException();
        }
        
        public virtual Quaternion GetMovementRotation(Transform playerTransform)
        {
            throw new System.NotImplementedException();
        }

        public virtual Vector3 GetInitialConnectPoint(Transform playerTransform)
        {
            throw new System.NotImplementedException();
        }
        
        public virtual Quaternion GetInitialRotation(Transform playerTransform)
        {
            throw new System.NotImplementedException();
        }
        
    }
}
