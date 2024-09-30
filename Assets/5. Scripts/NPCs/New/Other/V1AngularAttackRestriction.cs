using NPCs.New.V1;
using NPCs.New.V1.States;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NPCs.New.Other
{
    public class V1AngularAttackRestriction : MonoBehaviour
    {
        [Required] public V1NpcChaseState chaseState;
        [MinMaxSlider(-180, 180, true)]
        public Vector2 angleRange = new Vector2(-90, 90);


        public float angle;
        
        public UnityEvent<bool> onAngleChange;
        
        private void Update()
        {
            CheckAngle();
        }
        private void CheckAngle()
        {
            Vector3 direction = transform.position - transform.position;
            
            angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            onAngleChange.Invoke(angle>=angleRange.x && angle<=angleRange.y);
        }
    }
}
