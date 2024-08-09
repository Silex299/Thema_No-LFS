using System.Collections;
using Mechanics.Player.Custom;
using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableLadder : ClimbableBase
    {
        

        public float startLadder;
        public float endLadder;
        public Vector3 offset;
        public float movementSpeed = 0.1f;
        public float transitionTime = 0.2f;

        private float _playerAt;
        private Coroutine _engageCoroutine;
        
        
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

        public override void EngageClimbable(PlayerV1 player)
        {
            if (Engaged) return;
            _engageCoroutine ??= StartCoroutine(EngageCoroutine(player));
        }

        public override void MovePlayer(float input, Transform playerTransform)
        {
            //Movement
            _playerAt += Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;
            _playerAt = Mathf.Clamp01(_playerAt);
            var startPos = transform.position + transform.up * startLadder;
            Vector3 direction = transform.up;
            
            playerTransform.position = startPos + direction * (_playerAt * (endLadder - startLadder)) + offset;
            playerTransform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        }

        private IEnumerator EngageCoroutine(PlayerV1 player)
        {
            var startPos = transform.position + transform.up * startLadder;
            var endPos =transform.position + transform.up *  endLadder;

            var closestPoint = ThemaVector.GetClosestPointToLine(startPos, endPos, player.transform.position);
            _playerAt = (closestPoint - startPos).magnitude / (endPos - startPos).magnitude;


            float timeElapsed = 0;
            Vector3 initPlayerPos = player.transform.position;
            Quaternion initPlayerRot = player.transform.rotation;
            
            while (timeElapsed <= transitionTime)
            {
                timeElapsed += Time.deltaTime;
                
                player.transform.position = Vector3.Lerp(initPlayerPos, closestPoint + offset, timeElapsed / transitionTime);
                player.transform.rotation = Quaternion.Slerp(initPlayerRot, transform.rotation, timeElapsed / transitionTime);

                yield return new WaitForEndOfFrame();
            }

            Engaged = true;
            _engageCoroutine = null;

        }
       
    }
}
