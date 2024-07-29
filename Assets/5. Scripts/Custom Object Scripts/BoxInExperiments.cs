using System.Collections;
using Misc;
using Misc.Items;
using Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Custom_Object_Scripts
{
    public class BoxInExperiments : MonoBehaviour
    {

        [SerializeField] private Animator animator;
        [SerializeField] private float initialDelay;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Rope[] connectedRopes;

        [SerializeField] private UnityEvent onBoxFall;

        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if(_triggered) return;
        
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
        
            _triggered = true;
            yield return new WaitForSeconds(initialDelay);

            connectedRopes[0].BreakRope();

            yield return new WaitForSeconds(1.5f);


            connectedRopes[1].BreakRope();

            rb.isKinematic = true;
            animator.enabled = true;
            CutsceneManager.Instance.PlayClip(2);

        }


        public void EnableRopeTrigger()
        {
        
            onBoxFall.Invoke();
        }

    }
}
