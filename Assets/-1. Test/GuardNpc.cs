using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class GuardNpc : MonoBehaviour
{

    public Animator animator;

    public float rotationSpeed;
    public GuardNpcStateType stateEnum;
    
    public GuardNpcSurveillanceState surveillanceState = new GuardNpcSurveillanceState();
    public GuardNpcChaseState chaseState = new GuardNpcChaseState();
    public GuardNpcAfterPlayerDeathState afterPlayerDeathState = new GuardNpcAfterPlayerDeathState();
    private GuardNpcState _currentState;


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
    
    [System.Serializable]
    public enum  GuardNpcStateType
    {
        Surveillance,
        Chase,
        AfterDeath
    }
    
}