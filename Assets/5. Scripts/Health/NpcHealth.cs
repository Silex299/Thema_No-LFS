using Health;
using UnityEngine;

public class npcHealth : HealthBaseClass
{
    
    public CharacterController characterController;
    public Animator animator;

    private bool _isDead;
    
    public override void Kill(string message)
    {
        if(_isDead) return;

        _isDead = true;
        
        if (message == "RAY")
        {
            AnimationDeath("Float");
        }
        else
        {
            Death(message);
        }
    }


    protected override void Death(string message = "")
    {
        base.Death(message);
        _isDead = true;
        RagdollDeath();
    }

    private void RagdollDeath()
    {
        animator.enabled = false;
        characterController.enabled = false;
    }

    private void AnimationDeath(string animationName)
    {
        animator.CrossFade(animationName, 0.2f, 1);
    }

    private void Reset()
    {
        animator.enabled = true;
        characterController.enabled = true;
        _isDead = false;
    }
}
