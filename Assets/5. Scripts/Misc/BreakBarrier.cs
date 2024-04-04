using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Misc
{

    public class BreakBarrier : MonoBehaviour
    {

        [SerializeField] public Transform forceTransform;
        [SerializeField] public float breakForce;

        [SerializeField] private Barrier[] barriers;


        private int currentIndex;
        public void Break()
        {
            print("calling babe");
            Barrier barrier = barriers[currentIndex];

            barrier.BreakBarrier(breakForce, forceTransform);

            Invoke(nameof(BreakableAction), barrier.actionDelay);
        }


        private void BreakableAction()
        {
            barriers[currentIndex].action?.Invoke();
            currentIndex++;
        }
    }


    [System.Serializable]
    public class Barrier
    {
        public List<Rigidbody> rigidBody;
        public float actionDelay;
        public UnityEvent action;


        public void BreakBarrier(float breakForce, Transform forceTransform)
        {
            Debug.Log("Fuck me dady");
            foreach (var rb in rigidBody)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.AddForceAtPosition(breakForce * forceTransform.forward, forceTransform.position);
            }
        }
    }

}