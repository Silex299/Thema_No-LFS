using Player_Scripts;
using UnityEngine;
using Weapons.NPC_Weapon;

[System.Serializable]
// ReSharper disable once CheckNamespace
public class GuardNpcChaseState : GuardNpcState
{
    public float stopDistance;
    public float attackDistance;

    [Space(10)] public bool randomAttack;
    public int attackCount;

    public FireArm weapon;

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
        
        Vector3 guardPos = npc.transform.position + Vector3.up * 1.3f;
        Vector3 playerPos = PlayerMovementController.Instance.transform.position + Vector3.up * 1.3f;
        Vector3 destination = npc.PathFinder.GetNextPoint(guardPos, playerPos, npc.GetInstanceID());

        Rotate(npc.transform, destination, npc.rotationSpeed * 10);


        //distance between npc and player
        float distance = Vector3.Distance(npc.transform.position, PlayerMovementController.Instance.transform.position);

        //npc.animator.SetFloat(Speed, distance < stopDistance ? 0 : 1, 0.01f, Time.deltaTime);

        if (distance < attackDistance)
        {
            npc.animator.SetBool(Attack1, true);
            Attack();
        }
        else
        {
            npc.animator.SetBool(Attack1, false);
            _isAttacking = false;
        }
    }

    public override void Exit(GuardNpc npc)
    {
        npc.animator.SetBool(Chase, false);
        _isAttacking = false;
    }


    private float _startAttackTime;

    protected void Attack()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            _startAttackTime = Time.time;
        }

        if (Time.time < _startAttackTime + 0.5f) return;

        if (weapon)
        {
            weapon.Fire();
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