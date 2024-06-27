using Player_Scripts;
using UnityEngine;

[System.Serializable]
public class GuardNpcAfterPlayerDeathState : GuardNpcState
{
    public bool guardAfterChase = true;
    public float distanceThreshold = 0.5f;


    private bool _moving;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int AfterDeath = Animator.StringToHash("AfterDeath");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public override void Enter(GuardNpc npc)
    {
        npc.animator.SetBool(Attack, false);
        npc.animator.SetBool(Chase, false);
        
        if (guardAfterChase)
        {
            npc.ChangeState(GuardNpc.GuardNpcStateType.Surveillance);
            return;
        }

        npc.animator.SetBool(AfterDeath, true);
    }

    public override void Update(GuardNpc npc)
    {
        //distance between npc and player
        float distance = Vector3.Distance(npc.transform.position, PlayerMovementController.Instance.transform.position);

        Rotate(npc.transform, PlayerMovementController.Instance.transform.position, npc.rotationSpeed * 15);

        if (distance > distanceThreshold)
        {
            npc.animator.SetFloat(Speed, 1, 0.05f, Time.deltaTime);
        }
        else
        {
            npc.animator.SetFloat(Speed, 0, 0.05f, Time.deltaTime);
        }
        
    }

    public override void Exit(GuardNpc npc)
    {
    }
}