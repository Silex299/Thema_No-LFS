using Health;
using Sirenix.OdinInspector;
using UnityEngine;

public class BreakablePlankHealth : HealthBaseClass
{
    
    [SerializeField, BoxGroup("Prefab")] private GameObject brokenPlankPrefab;
    
    protected override void Death(string message = "")
    {
        Instantiate(brokenPlankPrefab, transform.position, transform.rotation);
        deathAction?.Invoke();
        Destroy(this.gameObject, 0.1f);
    }
}
