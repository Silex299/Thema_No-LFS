using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player_Scripts
{
    public class AnimationRigController : MonoBehaviour
    {
        [SerializeField] private Rig[] rigs;
        [SerializeField] private float transitionSmoothness;


        private int _currenIndex;
        private float _currentWeight;
        private bool _changeWeight;


        private void Update()
        {
            if (_changeWeight)
            {
                rigs[_currenIndex].weight = Mathf.MoveTowards(rigs[_currenIndex].weight, _currentWeight, transitionSmoothness);
                if (Mathf.Abs(rigs[_currenIndex].weight - _currentWeight) < 0.01)
                {
                    _changeWeight = false;
                }
            }
        }



        public void SetRigWeight(int index)
        {
            _changeWeight = true;
            _currenIndex = index;
            _currentWeight = 1;
        }

        public void ResetRigWeight(int index)
        {
            _changeWeight = true;
            _currenIndex = index;
            _currentWeight = 0;
        }
    }

}

