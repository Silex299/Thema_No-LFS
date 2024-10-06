using NPCs.New.V1.States;
using UnityEngine;

namespace Triggers
{
    public class NpcUpdatePathTrigger : MonoBehaviour
    {


        private Coroutine _triggerCoroutine;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("NPC")) return;
            
            var chaseState = other.GetComponentInChildren<V1NpcChaseState>();
            if (chaseState != null)
            {
                chaseState.UpdatePath = false;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("NPC")) return;
            
            var chaseState = other.GetComponentInChildren<V1NpcChaseState>();
            if (chaseState != null)
            {
                chaseState.UpdatePath = true;
            }
        }
    }
}
