using Player_Scripts;
using UnityEngine;

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

    public GuardNpcState currentState;
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
        currentState.Update(this);
    }
 

    public void ChangeState(int index)
    {
        ChangeState((GuardNpcStateType) index);
    }
    public void ChangeState(GuardNpcStateType newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        print("Changing state to " + newState);
        
        stateEnum = newState;

        switch (stateEnum)
        {
            case GuardNpcStateType.Chase:
                currentState = chaseState;
                break;
            case GuardNpcStateType.Surveillance:
                currentState = surveillanceState;
                break;
            case GuardNpcStateType.AfterDeath:
                currentState = afterPlayerDeathState;
                break;
        }
        

        // ReSharper disable once PossibleNullReferenceException
        currentState.Enter(this);
        
        
        print("changed state " + currentState);
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