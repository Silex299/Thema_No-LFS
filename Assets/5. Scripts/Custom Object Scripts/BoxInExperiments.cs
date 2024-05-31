using System.Collections;
using Misc.Items;
using Player_Scripts;
using Sirenix.OdinInspector;
using Triggers;
using UnityEngine;
using UnityEngine.Events;

public class BoxInExperiments : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private float initialDelay;
    [SerializeField] private Rigidbody connectedLoad;
    [SerializeField] private Rope[] connectedRopes;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            StartCoroutine(StartAction());
        }
    }


    private IEnumerator StartAction()
    {

        if (!PlayerMovementController.Instance.VerifyState(PlayerMovementState.BasicMovement))
        {
            yield break;
        }

        yield return new WaitForSeconds(initialDelay);

        connectedRopes[0].BreakRope();
        connectedLoad.isKinematic = false;

        yield return new WaitForSeconds(2f);

        connectedRopes[1].BreakRope();
        yield return new WaitForSeconds(0.1f);

        animator.enabled = true;
        animator.Play("BoxFall");

    }


}
