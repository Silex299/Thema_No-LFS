using System;
using System.Collections;
using Misc;
using Player_Scripts;
using UnityEngine;

public class AdvancedAnimationTrigger : MonoBehaviour
{

    //TODO: REMOVE
    public Animator animator;

    public string animationName;
    public Transform finalPos;

    public float transitionTime;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            StartCoroutine(TriggerAnimation());
        }
    }

    private IEnumerator TriggerAnimation()
    {
        PlayerMovementController.Instance.DisablePlayerMovement(true);
        PlayerMovementController.Instance.player.CController.enabled = false;
        PlayerMovementController.Instance.player.CanJump = false;
        
        yield return PlayerMover.MoveCoroutine(this.transform, transitionTime);

        if (finalPos)
        {
            yield return PlayerMover.MoveCoroutine(finalPos, transitionTime);
        }
        
        PlayerMovementController.Instance.DisablePlayerMovement(false);
        PlayerMovementController.Instance.player.CController.enabled = true;
        PlayerMovementController.Instance.player.CanJump = true;

    }


}
