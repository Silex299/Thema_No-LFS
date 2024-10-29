using UnityEngine;

namespace Path_Scripts
{
    public class TwoWayOverridePath : OverridePath
    {

        
        public float distance = 5f;
        
        public override Vector3 GetNextPosition(float input, float otherInput)
        {

            Vector3 forward = transform.forward * input;
            Vector3 right = transform.right * otherInput;
            
            Vector3 direction = (forward + right).normalized;
            
            print(transform.position + (direction * distance));
            return transform.position + (direction * distance);
        }
    }
}
