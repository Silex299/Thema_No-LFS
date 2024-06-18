// ReSharper disable once CheckNamespace

using UnityEngine;

public abstract class GuardNpcState
{
    public abstract void Enter(GuardNpc npc);
    public abstract void Update(GuardNpc npc);
    public abstract void Exit(GuardNpc npc);

    public void Rotate(Transform target, Vector3 destination, float rotationSpeed)
    {

        Debug.DrawLine(target.position, target.position + (destination - target.position).normalized, Color.blue);

        Vector3 forward = destination - target.position;
        forward = forward.normalized;
        forward.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(forward, target.up);
        target.rotation = Quaternion.Slerp(target.rotation, newRotation, Time.deltaTime * rotationSpeed);

    }
    
}