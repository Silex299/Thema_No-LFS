using Health;
using Misc;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.NPC_Weapon;

// ReSharper disable once CheckNamespace
public class GuardNpc : MonoBehaviour
{
    [BoxGroup("References")] public Animator animator;
    [BoxGroup("References")] public NpcPathFinder pathFinder;
    [BoxGroup("References")] public HealthBaseClass health;
    [BoxGroup("References")] public SightDetection sight;
    [BoxGroup("References")] public WeaponBase weapon;

    [BoxGroup("Properties")] public float rotationSpeed;

    [FoldoutGroup("States")] public GuardNpcStateType stateEnum;
    [FoldoutGroup("States")] public GuardNpcSurveillanceState surveillanceState = new GuardNpcSurveillanceState();
    [FoldoutGroup("States")] public GuardNpcChaseState chaseState = new GuardNpcChaseState();

    [FoldoutGroup("States")]
    public GuardNpcAfterPlayerDeathState afterPlayerDeathState = new GuardNpcAfterPlayerDeathState();

    private GuardNpcState _currentState;

    public NpcPathFinder PathFinder
    {
        get => pathFinder;
        set => pathFinder = value;
    }

    private void Start()
    {
        ChangeState(stateEnum);
        PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;

        if (sight)
        {
            health.onDeath += sight.DisableSightDetection;
        }

        if (weapon)
        {
            health.onDeath += weapon.ResetWeapon;
        }
    }

    private void OnDisable()
    {
        PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;

        if (sight)
        {
            health.onDeath -= sight.DisableSightDetection;
        }

        if (weapon)
        {
            health.onDeath -= weapon.ResetWeapon;
        }
    }


    private void Update()
    {
        _currentState.Update(this);
    }

    public void ChangeState(int index)
    {
        ChangeState((GuardNpcStateType)index);
    }

    public void ChangeState(GuardNpcStateType newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        print("Changing state to " + newState);

        stateEnum = newState;

        switch (stateEnum)
        {
            case GuardNpcStateType.Chase:
                _currentState = chaseState;
                break;
            case GuardNpcStateType.Surveillance:
                _currentState = surveillanceState;
                break;
            case GuardNpcStateType.AfterDeath:
                _currentState = afterPlayerDeathState;
                break;
        }


        // ReSharper disable once PossibleNullReferenceException
        _currentState.Enter(this);


        print("changed state " + _currentState);
    }

    private void OnPlayerDeath()
    {
        ChangeState(GuardNpcStateType.AfterDeath);
    }

    public void StopChasing(bool status)
    {
        if (stateEnum == GuardNpcStateType.Chase)
        {
            chaseState.StopChasing(status, this);
        }
    }

    public void ResetNpc(int stateIndex = 0)
    {
        health.ResetHealth();
        if (sight) sight.EnableSightDetection();
        if (weapon) weapon.ResetWeapon();
        ChangeState(stateIndex);
    }

    [System.Serializable]
    public enum GuardNpcStateType
    {
        Surveillance,
        Chase,
        AfterDeath
    }
}