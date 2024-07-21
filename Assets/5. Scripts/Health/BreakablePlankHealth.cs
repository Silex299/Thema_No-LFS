using Sirenix.OdinInspector;
using UnityEngine;

namespace Health
{
    public class BreakablePlankHealth : HealthBaseClass
    {
    
        [SerializeField, BoxGroup("Prefab")] private GameObject brokenPlankPrefab;
    
        [Button("Break", ButtonSizes.Gigantic)]
        protected override void Death(string message = "")
        {
            GameObject obj = Instantiate(brokenPlankPrefab, transform.position, transform.rotation, transform.parent);
            obj.transform.localScale = this.transform.localScale;

            gameObject.SetActive(false);
            deathAction?.Invoke();
        }
    }
}
