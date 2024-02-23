using UnityEngine;


namespace Misc.Items
{

    public class ReactiveInteractable : MonoBehaviour
    {
        
        public virtual void Interact(bool status)
        {
            print("Interact ::: " + gameObject.name);
        }


    }
}
