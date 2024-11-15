using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

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


        [Space(10)] public bool overrideEndExit;
        [ShowIf(nameof(overrideEndExit))] public UnityEvent endExitEvent;
        [Space(10)] public bool overrideStartExit;
        [ShowIf(nameof(overrideStartExit))] public UnityEvent startExitEvent;

        private float _playerAt;
        private Vector3 _playerPosition;
        public bool engaged;


        public bool Engaged
        {
            set => engaged = value;
        }
        

        public void MoveLadder(float input)
        {
            _playerAt += input * Time.deltaTime * movementSpeed;
            _playerAt = Mathf.Clamp01(_playerAt);

            Vector3 direction = (endLadder.position - startLadder.position);
            _playerPosition = startLadder.position + _playerAt * direction;

            PlayerMovementController.Instance.transform.position = _playerPosition;


            switch (_playerAt)
            {
                case 0 when input < 0:
                    DisEngage(true);
                    break;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                case 1 when input > 0:
                    DisEngage(false);
                    break;
            }
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_playerPosition, 0.2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startLadder.position, endLadder.position);
        }

        public void Engage(bool atStart)
        {
            if (engaged) return;

            _playerAt = atStart ? 0 : 1;
            StartCoroutine(EngageLadder(atStart));
        }

        private IEnumerator EngageLadder(bool atStart)
        {
            engaged = true;

            PlayerMovementController.Instance.player.ladderMovementState.connectedLadder = this;
            PlayerMovementController.Instance.ChangeState(1);

            var requiredTransform = atStart ? startLadder : endLadder;

            //Move player to required position and rotate if needed using PlayerMover 
            yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime);
        }


        public void DisEngage(bool atStart)
        {
            if (!engaged) return;


            if (atStart)
            {
                if (overrideStartExit || !startDisengagedTransform)
                {
                    startExitEvent.Invoke();
                    return;
                }
            }
            else
            {
                if(overrideEndExit || !endDisengagedTransform)
                {
                    endExitEvent.Invoke();
                    return;
                }
            }
            
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