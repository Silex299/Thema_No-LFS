using System;
using System.Collections;
using System.Collections.Generic;
using Health;
using NPCs.New.Other;
using NPCs.New.Path_Finder;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPCs.New.V1
{
    public class V1Npc : SerializedMonoBehaviour
    {

        #region Variables


        #region Editor Exposed Variables
        
        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public V1NpcAimRigController aimRigController;  //TODO: MOVE AIM RIG CONTROLLER TO ONLY NEEDED BASE AND CREATE A RESET AND ON DEATH METHOD FOR EACH STATE
        [FoldoutGroup("References")] public HealthBaseClass health;
        
        [FoldoutGroup("Npc Movement Properties")] public float stopDistance = 1f; //TODO: REMOVE THIS COMPLETELY
        [FoldoutGroup("Npc Movement Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Movement Properties")] public float accelerationTime = 1;


        [FoldoutGroup("Path Finder")] public PathFinderBase pathFinder;
        [FoldoutGroup("Path Finder")] public float targetOffset;
        [FoldoutGroup("Path Finder")] public float npcEyeHeight = 1.5f;
        [FoldoutGroup("Path Finder")] public float pathFindingInterval = 0.5f;
        
        [FoldoutGroup("States")] public int initState;
        [FoldoutGroup("States")] public float afterPlayerDeathDelay;
        [FoldoutGroup("States")] public int afterPlayerDeathState;
        [FoldoutGroup("States")] public V1NpcBaseState[] states;
        [FoldoutGroup("States")] public Dictionary<int, V1NpcBaseState[]> subStates;
        
        #endregion

        #region Non Exposed Variables
        
        private int _currentStateIndex = -1;
        public Transform Target => PlayerMovementController.Instance.transform; //Was target before, If there is any problem change it back to target and set it in start method;

        public bool CanAttack { get; set; } = true;
        public Action<int> onStateChange;
        public Action onNpcDeath;
        
        
        
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int PathBlocked = Animator.StringToHash("PathBlocked");
        private static readonly int StateIndex = Animator.StringToHash("StateIndex");

        #endregion
        

        #endregion

        #region Built in methods
        
        
        private void Start()
        {
            ChangeState(initState);
        }
        private void OnEnable()
        {
            PlayerMovementController.Instance.player.Health.onDeath += OnPlayerDeath;
            if(health) health.onDeath += OnNpcDeath;
        }
        private void OnDisable()
        {
            PlayerMovementController.Instance.player.Health.onDeath -= OnPlayerDeath;
            if(health) health.onDeath -= OnNpcDeath;
        }
        private void Update()
        {
            if (health )
            {
                if(health.IsDead) return;
            }

            if (_currentStateIndex != -1)
            {
                states[_currentStateIndex].UpdateState(this);
                
                if (subStates.TryGetValue(_currentStateIndex, out var currentSubStates))
                {
                    if (currentSubStates.Length > 0)
                    {
                        foreach (var subState in currentSubStates)
                        {
                            subState.UpdateState(this);
                        }
                    }
                }
            }
            
        }

        private void LateUpdate()
        {
            if (health )
            {
                if(health.IsDead) return;
            }

            if (_currentStateIndex != -1)
            {
                states[_currentStateIndex].LateUpdateState(this);
                
                if (subStates.TryGetValue(_currentStateIndex, out var currentSubStates))
                {
                    if (currentSubStates.Length > 0)
                    {
                        foreach (var subState in currentSubStates)
                        {
                            subState.LateUpdateState(this);
                        }
                    }
                }
            }
        }

        #endregion
        
        #region Custom Methods
        
        public void Rotate(Vector3 desiredPos, float speed)
        {
            
            Vector3 forward = desiredPos - transform.position;
            forward.y = 0;
            Quaternion desiredRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, speed);
        }
        public void UnRestrictedRotate(Vector3 desiredPos, float speed)
        {
            Vector3 forward = desiredPos - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(forward.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, speed);
        }
        public void UnRestrictedRotate(Vector3 desiredPos, float speed, Vector3 forcedUp)
        {
            Vector3 forward = desiredPos - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(forward.normalized, forcedUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, speed);
        }

        public void ChangeState(int stateIndex)
        {
            ChangeState(stateIndex, false);
        }
        
        public void ChangeState(int stateIndex, bool overrideHealthCheck = false)
        {
            
            if(stateIndex == _currentStateIndex) return;
            if (!overrideHealthCheck && health)
            {
                if(health.IsDead) return;
            }
            
            if(_currentStateIndex!=-1) states[_currentStateIndex].Exit(this);
            
            onStateChange?.Invoke(stateIndex);
            print("Changing Npc state::" + stateIndex);
            
            if (stateIndex == -1)
            {
                states[_currentStateIndex].Exit(this);
                enabled = false;
                return;
            }
            _currentStateIndex = stateIndex;
            
            states[_currentStateIndex].Enter(this);

        }
      
        
        private void OnPlayerDeath()
        {
            print("FUCK THE PLAYERS DEAD");
            StartCoroutine(OnPlayerDeath(afterPlayerDeathDelay));
        }

        private IEnumerator OnPlayerDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeState(afterPlayerDeathState);
        }

        private void OnNpcDeath()
        {
            if (_currentStateIndex != -1)
            {
                states[_currentStateIndex].Exit(this);
            }
            _currentStateIndex = -1;
            onNpcDeath?.Invoke();
        }  
        
        public void Reset()
        {
            ChangeState(initState, true);
            //Reset Animator
            animator.SetBool(Attack, false);
            animator.SetFloat(Speed, 0);
            animator.SetBool(PathBlocked, false);
            animator.SetInteger(StateIndex, 0);
            
        }
        
        #endregion
       
        
    }
}
