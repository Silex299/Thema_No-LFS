using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class FractionalMovingPlatform : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Movement"), InfoBox("Only two positions")] private List<Vector3> level;

        [SerializeField, FoldoutGroup("Movement"), Button("Set Position", ButtonSizes.Large), GUIColor(0.1f, 0.7f, 1)]
        public void SetPosition()
        {
            level.Add(transform.position);
        }


        private float _distance;
        private Vector3 _direction;
        
        private void Start()
        {
            _distance = Mathf.Abs((level[1] - level[0]).magnitude);
            _direction = (level[1] - level[0]).normalized;
        }

        public void UpdatePosition(float fraction)
        {
            transform.position = level[0] + _direction * (_distance * fraction);
        }
    }
}
