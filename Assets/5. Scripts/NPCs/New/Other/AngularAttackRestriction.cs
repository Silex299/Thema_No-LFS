
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.New.Other
{
    public class AngularAttackRestriction : MonoBehaviour
    {
        public Npc npc;
        [MinMaxSlider(-180, 180, true)]
        public Vector2 angleRange = new Vector2(-90, 90);


        public bool condition;
        public float angle;
        
        private void Update()
        {
            CheckAngle();
        }
     
        private void CheckAngle()
        {
            Vector3 direction = npc.target.position - transform.position;
            
            angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            condition = angle>=angleRange.x && angle<=angleRange.y;
            npc.CanAttack = condition;
        }
    }
}
