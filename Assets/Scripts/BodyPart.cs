using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class BodyPart : MonoBehaviour
{

    [SerializeField] private VisualEffect vfx;
    
    private bool _spawned;
    [Button("Get VFX Graph")]
    public void GetVFX()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(_spawned) return;
        if (vfx)
        { 
            if (!other.collider.CompareTag("Player"))
            {
                vfx.Play();
                vfx.transform.parent = GameManager.instance.transform;
                _spawned = true;
            }
        }
        
    }
}
