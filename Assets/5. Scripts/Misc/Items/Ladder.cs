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

        public bool engaged;

        public void Engage(bool atStart)
        {
            if (engaged) return;
            StartCoroutine(EngageLadder(atStart));
        }

        private IEnumerator EngageLadder(bool atStart)
        {
        
            engaged = true;
            PlayerMovementController.Instance.ChangeState(1);

            var requiredTransform = atStart ? startLadder : endLadder;

            //Move player to required position and rotate if needed using PlayerMover 
            yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime, true, true, false);

            PlayerMovementController.Instance.player.ladderMovementState.connectedLadder = this;
            PlayerMovementController.Instance.ResetAnimator();
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
            yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime, true, true, true);

            engaged = false;
            PlayerMovementController.Instance.ResetAnimator();
        }

    }
}