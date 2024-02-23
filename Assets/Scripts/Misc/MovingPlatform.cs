using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class MovingPlatform : MonoBehaviour
    {

        [SerializeField] private float movementSmoothness;
        [SerializeField] private List<Vector3> positions;

        [SerializeField] private UnityEvent completeMovement;

        [Button("GetPosition", ButtonSizes.Large)]
        public void GetPosition()
        {
            positions.Add(this.transform.position);
        }


        private int _currentTargetIndex;
        private bool _movePlatform;
        

        public void MovePlatform()
        {
            _movePlatform = true;
            _currentTargetIndex = (_currentTargetIndex + 1) % positions.Count;
        }

        private void FixedUpdate()
        {
            if(!_movePlatform) return;
            
            transform.position = Vector3.MoveTowards(transform.position, positions[_currentTargetIndex],
                Time.fixedDeltaTime * movementSmoothness);
            if (Mathf.Abs((this.transform.position - positions[_currentTargetIndex]).magnitude) < 0.05f)
            {
                _movePlatform = false;
                completeMovement?.Invoke();
            }
            
        }
        
    }
}
