
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class ToggleAnimationIndex : MonoBehaviour
    {
        [SerializeField] private int[] indecies;

        [SerializeField] private UnityEvent<int> action; 

        private int _currentIndex;

        public void PlayAction()
        {
            action?.Invoke(_currentIndex);
            _currentIndex = (_currentIndex + 1) % indecies.Length;
        }

        public void SetCurrentIndex(int index)
        {
            _currentIndex = index;
        }

    }
}
