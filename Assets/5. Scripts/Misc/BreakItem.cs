using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class BreakItem : MonoBehaviour
    {
       
        [AssetsOnly] public GameObject brokenPrefab;

        public void Break()
        {
            this.gameObject.SetActive(false);
            //Instantiate broken prefab at the same position and rotation
            Instantiate(brokenPrefab, transform.position, transform.rotation, transform.parent);
        }

        public void ResetItem()
        {
            this.gameObject.SetActive(true);
        }
    }
}
