using Health;
using NPCs.New.Other;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New.V1
{
    public class V1Npc : MonoBehaviour
    {

        #region Variables


        #region Editor Exposed Variables
        
        [FoldoutGroup("References")] public Animator animator;
        [FoldoutGroup("References")] public ProximityDetection proximityDetection;
        [FoldoutGroup("References")] public V1NpcAimRigController aimRigController;
        [FoldoutGroup("References")] public HealthBaseClass health;
        
        [FoldoutGroup("Npc Movement Properties")] public float stopDistance = 1f;
        [FoldoutGroup("Npc Movement Properties")] public float rotationSpeed = 10;
        [FoldoutGroup("Npc Movement Properties")] public float accelerationTime = 1;


        [FoldoutGroup("Path Finder")] public PathFinderBase pathFinder;
        [FoldoutGroup("Path Finder")] public float targetOffset;
        [FoldoutGroup("Path Finder")] public float npcEyeHeight = 1.5f;
        [FoldoutGroup("Path Finder")] public float pathFindingInterval = 0.5f;
        
        [FoldoutGroup("States")] public int initState;
        [FoldoutGroup("States")] public int afterPlayerDeathState;
        [FoldoutGroup("States")] public V1NpcBaseState[] states;
        
        #endregion

        #region Non Exposed Variables
        
        private int _currentStateIndex = -1;
        [HideInInspector] public Transform target;

        public bool CanAttack { get; set; } = true;
        
        
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
            target = PlayerMovementController.Instance.transform;
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
            
            if(_currentStateIndex!=-1) states[_currentStateIndex].UpdateState(this);
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
        public void ChangeState(int stateIndex)
        {
            print("Changing Npc state");
            if(stateIndex == _currentStateIndex) return;
            
            if (health)
            {
                if(health.IsDead) return;
            }
            
            if(_currentStateIndex!=-1) states[_currentStateIndex].Exit(this);
            
            //TODO: TEST MAY NOT WORK
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
            ChangeState(afterPlayerDeathState);
        }

        private void OnNpcDeath()
        {
            states[_currentStateIndex].Exit(this);
            _currentStateIndex = -1;
        }  
        
        public void Reset()
        {
            ChangeState(initState);
            
            //Reset Animator
            animator.SetBool(Attack, false);
            animator.SetFloat(Speed, 0);
            animator.SetBool(PathBlocked, false);
            animator.SetInteger(StateIndex, 0);
            
        }
        
        #endregion
       
        
    }
}
