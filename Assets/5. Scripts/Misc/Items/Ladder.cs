using System.Collections;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Misc.Items
{
    public class Ladder : MonoBehaviour
    {
        public bool canJumpOfTheLadder;
        public float transitionTime;

        public Transform startLadder;
        public Transform endLadder;

        public Transform startDisengagedTransform;
        public Transform endDisengagedTransform;


        public float movementSpeed = 0.1f;


        private float _playerAt;
        private Vector3 _playerPosition;
        [HideInInspector] public bool engaged;

        public void MoveLadder(float input)
        {
            _playerAt += input * Time.deltaTime * movementSpeed;
            _playerAt = Mathf.Clamp01(_playerAt);

            Vector3 direction = (endLadder.position - startLadder.position);
            _playerPosition = startLadder.position + _playerAt * direction;

            PlayerMovementController.Instance.transform.position = _playerPosition;


            if (_playerAt == 0 && input < 0)
            {
                DisEngage(true);
            }

            if (_playerAt == 1 && input > 0)
            {
                DisEngage(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_playerPosition, 0.2f);
        }

        public void Engage(bool atStart)
        {
            if (engaged) return;

            print("Get Ready to fuck me daddy:" + Time.time);
            _playerAt = atStart ? 0 : 1;
            StartCoroutine(EngageLadder(atStart));
        }

        private IEnumerator EngageLadder(bool atStart)
        {
            engaged = true;
            print("Fuck me daddy ::" + Time.time);

            PlayerMovementController.Instance.player.ladderMovementState.connectedLadder = this;
            PlayerMovementController.Instance.ChangeState(1);

            var requiredTransform = atStart ? startLadder : endLadder;

            //Move player to required position and rotate if needed using PlayerMover 
            yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime, true, true, false);

        }


        public void DisEngage(bool atStart)
        {
            if (!engaged) return;

            StartCoroutine(DisEngageLadder(atStart));
        }

        private IEnumerator DisEngageLadder(bool atStart)
        {
            PlayerMovementController.Instance.RollBack();
            
            var requiredTransform = atStart ? startDisengagedTransform : endDisengagedTransform;
            
            //Move player to required position and rotate if needed using PlayerMover 
            yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime);

            
            engaged = false;
        }
    }
}