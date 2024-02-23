using UnityEngine;

namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class ChangeParent : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            //ADD more tags if needed
            if (other.CompareTag("Player_Main"))
            {
                other.transform.parent = transform;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                other.transform.parent = MasterManager.Instance.transform;
            }
        }
    }

}