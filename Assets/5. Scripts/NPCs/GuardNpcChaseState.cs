using Player_Scripts;
using UnityEngine;
using Weapons.NPC_Weapon;

[System.Serializable]
// ReSharper disable once CheckNamespace
public class GuardNpcChaseState : GuardNpcState
{
    public float stopDistance;
    public float attackDistance;

    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    private bool _isAttacking;
    private bool _canChase = true;

    public override void Enter(GuardNpc npc)
    {
        npc.animator.SetBool(Chase, true);
        _canChase = true;
    }

    public override void Update(GuardNpc npc)
    {
        if(!_canChase) return;
        
        Vector3 playerPos = PlayerMovementController.Instance.transform.position;
        Vector3 npcPos = npc.transform.position;
        
        Vector3 destination = npc.PathFinder.GetNextPoint(npc, playerPos);
        Rotate(npc.transform, destination + Vector3.up * 1.3f, npc.rotationSpeed * 10);
        
        float destinationDistance = Vector3.Distance(npc.transform.position, destination);
        float playerOriginalDistance =  Vector3.Distance(npc.transform.position, playerPos);
        float planerDistance = Vector3.Distance(playerPos, new Vector3(npcPos.x, playerPos.y, npcPos.z));
        

        npc.animator.SetFloat(Speed, destinationDistance < stopDistance ? 0 : 1, 0.01f, Time.deltaTime);
        
        
        if (playerOriginalDistance < attackDistance)
        {
            npc.animator.SetBool(Attack1, true);
            Attack(npc);
        }
        else
        {
            npc.animator.SetBool(Attack1, false);
            _isAttacking = false;
        }

        if (planerDistance < stopDistance && playerOriginalDistance>attackDistance)
        {
            npc.animator.SetBool(Unreachable, true);
        }
        else
        {
            npc.animator.SetBool(Unreachable, false);
        }
    }

    public override void Exit(GuardNpc npc)
    {
        npc.animator.SetBool(Chase, false);
        _isAttacking = false;
    }


    private float _startAttackTime;
    private static readonly int Unreachable = Animator.StringToHash("Unreachable");

    protected void Attack(GuardNpc npc)
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            _startAttackTime = Time.time;
        }

        if (Time.time < _startAttackTime + 0.5f) return;

        if (npc.weapon)
        {
            npc.weapon.Fire();
        }
    }

    public void StopChasing(bool status, GuardNpc npc)
    {
        _canChase = !status;
        npc.animator.SetBool(Chase, !status);

        if (!_canChase)
        {
            npc.animator.SetFloat(Speed, 0);
        }
    }
}