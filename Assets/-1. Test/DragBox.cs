
using Player_Scripts;
using Player_Scripts.Interactables;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragBox : Interactable
{
    
    public Rigidbody rigidbody;
    public float pushStrength;
    
    public override PlayerInteractionType Interact()
    {
        if (!enabled) return PlayerInteractionType.NONE;

        if (!GetKey())
        {
            _isInteracting = false;
            return PlayerInteractionType.NONE;
        }
        
        Vector3 pushDirection = PlayerMovementController.Instance.transform.forward;
        rigidbody.AddForce(pushDirection * (pushStrength * Time.deltaTime), ForceMode.Force);
        
        return interactionType;
    }
}
