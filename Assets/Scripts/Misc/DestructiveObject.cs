using Sirenix.OdinInspector;
using UnityEngine;

public class DestructiveObject : MonoBehaviour
{
    
    [SerializeField, SceneObjectsOnly] private GameObject nonDestructedObject;
    [SerializeField, SceneObjectsOnly] private GameObject destructedObject;


    [SerializeField] private Vector3 impulse;


    public bool destroy;
    private bool _destroyed;


    private void Update()
    {
        if (destroy && !_destroyed)
        {
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        if (_destroyed) return;

        Destroy(nonDestructedObject);
        destructedObject.SetActive(true);
        var rb = destructedObject.GetComponentsInChildren<Rigidbody>();
        foreach(var r in rb)
        {
            r.AddForce(impulse, ForceMode.Impulse);
        }
        _destroyed = true;
    }


}
