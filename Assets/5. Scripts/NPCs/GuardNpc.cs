using Player_Scripts;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
public class GuardNpc : MonoBehaviour
{

    public Animator animator;

    public float rotationSpeed;
    public GuardNpcStateType stateEnum;
    [SerializeField] private NpcPathFinder pathFinder;
   
    
    public GuardNpcSurveillanceState surveillanceState = new GuardNpcSurveillanceState();
    public GuardNpcChaseState chaseState = new GuardNpcChaseState();
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
        PlayerMovementController.Instance.player.Health.OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerMovementController.Instance.player.Health.OnDeath -= OnPlayerDeath;
    }


    private void Update()
    {
        _currentState.Update(this);
    }
 

    public void ChangeState(int index)
    {
        ChangeState((GuardNpcStateType) index);
    }
    public void ChangeState(GuardNpcStateType newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        stateEnum = newState;
        
        _currentState = newState switch
        {
            GuardNpcStateType.Surveillance => surveillanceState,
            GuardNpcStateType.Chase => chaseState,
            GuardNpcStateType.AfterDeath => afterPlayerDeathState,
            _ => _currentState
        };

        // ReSharper disable once PossibleNullReferenceException
        _currentState.Enter(this);
    }
    
    private void OnPlayerDeath()
    {
        ChangeState(GuardNpcStateType.AfterDeath);
    }


    public void StopChasing(bool status)
    {
        if(stateEnum == GuardNpcStateType.Chase)
        {
            chaseState.StopChasing(status, this);
        }
    }
    
    [System.Serializable]
    public enum  GuardNpcStateType
    {
        Surveillance,
        Chase,
        AfterDeath
    }
    
}