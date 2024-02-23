using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public class LevelPlatform : MonoBehaviour
    {
        [SerializeField, InfoBox("Place only two positions")] private List<Vector3> platfromPositions;
        [SerializeField] private float movementSpeed;


        private int _currentPlatformIndex;
        private bool _movePlatform;

        private void Update()
        {
            if (_movePlatform)
            {

                var pos = transform.position;

                transform.position = Vector3.MoveTowards(pos, platfromPositions[_currentPlatformIndex], Time.deltaTime * movementSpeed);
               
                if(Vector3.Distance(pos, platfromPositions[_currentPlatformIndex]) < 0.01f)
                {
                    _movePlatform = false;
                }

            }
        }

        [SerializeField, Button("Get Position", ButtonSizes.Large)]
        public void GetPosition()
        {
            platfromPositions.Add(transform.position);
        }
        public void MovePlatform(bool open)
        {
            _currentPlatformIndex = open ? 1 : 0;
            _movePlatform = true;
        } 
    }
}