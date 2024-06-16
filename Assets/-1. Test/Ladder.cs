using System.Collections;
using Player_Scripts;
using UnityEngine;

public class Ladder : MonoBehaviour
{


    public bool canJumpOfTheLadder;
    public float transitionTime;
    
    public Transform startLadder;
    public Transform endLadder;
    
    public Transform startDisengagedTransform;  
    public Transform endDisengagedTransform;

    private bool _engaged;
    
    public void Engage(bool atStart)
    {
        if(_engaged) return;
        StartCoroutine(EngageLadder(atStart));
    }

    private IEnumerator EngageLadder(bool atStart)
    {

        _engaged = true;
        PlayerMovementController.Instance.ChangeState(1);
        
        var requiredTransform = atStart ? startLadder : endLadder;

        //Move player to required position and rotate if needed using PlayerMover 
        yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime, true, true, false);
        
        PlayerMovementController.Instance.player.ladderMovementState.connectedLadder = this;
        
    }


    public void DisEngage(bool atStart)
    {
        if(!_engaged) return;
        StartCoroutine(DisEngageLadder(atStart));
    }

    private IEnumerator DisEngageLadder(bool atStart)
    {
        PlayerMovementController.Instance.RollBack();
        
        var requiredTransform = atStart ? startDisengagedTransform : endDisengagedTransform;

        //Move player to required position and rotate if needed using PlayerMover 
        yield return PlayerMover.MoveCoroutine(requiredTransform, transitionTime, true, true, false);
        
        _engaged = false;
    }
    
}
