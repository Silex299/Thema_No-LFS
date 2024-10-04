using System.Collections;
using System.Collections.Generic;
using NPCs.New.V1;
using UnityEngine;

namespace Triggers
{
    public class NpcUpdatePathTrigger : MonoBehaviour
    {


        private Coroutine _triggerCoroutine;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("NPC"))
            {

                if (_triggerCoroutine == null)
                {
                    if (other.TryGetComponent(out V1Npc npc))
                    {
                        
                    }
                }
                
            }
        }
        
        private IEnumerator ResetTriggerCoroutine(Collider other)
        {
            yield return new WaitForSeconds(0.5f);
        }



    }
}
