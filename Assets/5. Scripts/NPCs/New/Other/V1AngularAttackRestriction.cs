using NPCs.New.V1;
using NPCs.New.V1.States;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NPCs.New.Other
{
    public class V1AngularAttackRestriction : MonoBehaviour
    {
        [MinMaxSlider(0, 180, true)] public Vector2 angleRange = new Vector2(-90, 90);
        public float angleOffset;


        public float angle;

        public UnityEvent<bool> onAngleChange;

        private void Update()
        {
            CheckAngle();
        }

        
        private void CheckAngle()
        {
            Vector3 direction = PlayerMovementController.Instance.transform.position - transform.position;
            direction.y = transform.forward.y;

            angle = Vector3.Angle(transform.forward, direction.normalized);
            angle = Mathf.Abs(angle) + angleOffset;
            
            onAngleChange.Invoke(angle >= angleRange.x && angle <= angleRange.y);
        }
    }
}