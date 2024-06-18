using Player_Scripts;
using UnityEngine;

[System.Serializable]
// ReSharper disable once CheckNamespace
public class GuardNpcChaseState : GuardNpcState
{

    public float stopDistance;
    public float attackDistance;

    public NpcPathFinder pathFinder;
    public NPCs.Weapons.FireArm weapon;
    
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    private bool isAttacking;

    public override void Enter(GuardNpc npc)
    {
        npc.animator.SetBool(Chase, true);
    }

    public override void Update(GuardNpc npc)
    {
        
        Vector3 guardPos = npc.transform.position + Vector3.up * 1.3f;
        Vector3 playerPos = PlayerMovementController.Instance.transform.position + Vector3.up * 1.3f;
        Vector3 destination = pathFinder.GetNextPoint(guardPos, playerPos);
        
        Rotate(npc.transform, destination, npc.rotationSpeed*10);
        
        
                
        //distance between npc and player
        float distance = Vector3.Distance(npc.transform.position, PlayerMovementController.Instance.transform.position);

        if (distance < stopDistance)
        {
            npc.animator.SetFloat(Speed, 0, 0.03f, Time.deltaTime);
        }
        else
        {
            npc.animator.SetFloat(Speed, 1, 0.03f, Time.deltaTime);
        }

        if (distance < attackDistance)
        {
            npc.animator.SetBool(Attack1, true);
            Attack();
        }
        else
        {
            npc.animator.SetBool(Attack1, false);
            isAttacking = false;
        }



    }

    public override void Exit(GuardNpc npc)
    {
        npc.animator.SetBool(Chase, false);
        isAttacking = false;
    }


    private float startAttackTime;
    protected void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            startAttackTime = Time.time;
        }
        
        if(Time.time < startAttackTime + 0.5f) return;
        
        if (weapon)
        {
            weapon.FireBullet();
        }
    }
    
}