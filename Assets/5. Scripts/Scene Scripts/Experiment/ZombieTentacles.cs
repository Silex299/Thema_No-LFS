using UnityEngine;

namespace Scene_Scripts.Experiment
{
    public class ZombieTentacles : MonoBehaviour
    {

        public Animator animator;
        
        private bool _isScreaming = false;

        public void Scream()
        {
            if (_isScreaming) return;
            
            animator.CrossFade("Scream", 0.1f);
            _isScreaming = true;
        }
        
        public void StopScream()
        {
            if(!_isScreaming) return;
            
            animator.CrossFade("StopScream", 0.1f);
            _isScreaming = false;
        }


    }
}
