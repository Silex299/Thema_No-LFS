using Mechanics.Player.Custom;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableLadder : ClimbableBase
    {
        #region Variables

        #region Climbable Properties

        [BoxGroup("Climbable Param")] public float startLadder;
        [BoxGroup("Climbable Param")] public float endLadder;
        [BoxGroup("Climbable Param")] public Vector3 offset;

        #endregion
        
        private float _playerAt;

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var playerPosition = (transform.position + transform.up * startLadder) +
                                 transform.up * _playerAt * (endLadder - startLadder) + offset;
            Gizmos.DrawWireSphere(playerPosition, 0.2f);

            Gizmos.color = Color.yellow;
            var starPos = transform.position + transform.up * startLadder;
            var endPos = transform.position + transform.up * endLadder;
            Gizmos.DrawLine(starPos, endPos);
        }

        #endregion

        #region Overriden Methods
        
        public override Vector3 GetInitialConnectPoint(Transform playerTransform)
        {
            var startPos = transform.position + transform.up * startLadder;
            var endPos = transform.position + transform.up * endLadder;
            var closestPoint = ThemaVector.GetClosestPointToLine(startPos, endPos, playerTransform.position);
            
            _playerAt = (closestPoint - startPos).magnitude / (endPos - startPos).magnitude;

            return closestPoint;
        }   
        public override Quaternion GetInitialRotation(Transform playerTransform)
        {
            return Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        public override Quaternion GetMovementRotation(Transform playerTransform)
        {
            return Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        public override Vector3 GetMovementVector(Transform playerTransform, float speed)
        {
            _playerAt = Mathf.Clamp01(_playerAt + Input.GetAxis("Vertical") * speed);
            var startPos = transform.position + transform.up * startLadder;
            var targetPos = startPos + transform.up * (_playerAt * (endLadder - startLadder)) + offset;
            return targetPos - playerTransform.position;
        }
        
        
        
        #endregion

    }
}