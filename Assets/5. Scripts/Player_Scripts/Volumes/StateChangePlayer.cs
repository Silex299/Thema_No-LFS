using UnityEngine;

namespace Player_Scripts.Volumes
{

    public class StateChangePlayer : MonoBehaviour
    {
        
        [SerializeField] private int stateIndex;
        
        [SerializeField] private bool enableDirection;
        [SerializeField] private bool oneWayRotation;


        //Create a function to state Player state in PlayerMovementController.cs
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                PlayerMovementController.Instance.ChangeState(stateIndex);
                
                PlayerMovementController.Instance.player.enabledDirectionInput = enableDirection;
                PlayerMovementController.Instance.player.oneWayRotation = oneWayRotation;

            }
        }

    }

}