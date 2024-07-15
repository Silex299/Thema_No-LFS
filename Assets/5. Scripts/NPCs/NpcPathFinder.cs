using Sirenix.OdinInspector;
using UnityEngine;

public class NpcPathFinder : MonoBehaviour
{
    [InfoBox("raycast has to hit nothing in order to get direct player position")]
    public LayerMask layerMask;

    public virtual Vector3 GetNextPoint( GuardNpc npc, Vector3 destination)
    {
        return Vector3.zero;
    }
    
    protected bool InSight(Vector3 obj1, Vector3 obj2)
    {
        return !Physics.Linecast(obj1, obj2, layerMask);
    }
}