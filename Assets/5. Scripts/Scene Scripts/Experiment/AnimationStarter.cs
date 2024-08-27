using System;
using UnityEngine;

namespace Scene_Scripts.Experiment
{
    public class AnimationStarter : MonoBehaviour
    {

        public Animator animator;
        public string starterAnimation;

        private void Start()
        {
            animator.Play(starterAnimation);
        }
    }
}
