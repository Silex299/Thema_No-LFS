using Misc;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Player_Scripts.Interactions
{
    public class DimensionShiftScript : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private ParticleSystem shiftParticleEffect;


        private bool _isTransitioning = false;

        private void Update()
        {
            if (Input.GetButtonDown("f"))
            {
                ShiftDimension();
            }
        }

        private void ShiftDimension()
        {
            if(_isTransitioning) return;
            DimensionShiftStart();
        }


        private void DimensionShiftStart()
        {
            _isTransitioning = true;
            player.AnimationController.CrossFade("ShiftDimension", 0.5f, 1);
            player.MovementController.DisablePlayerMovement(true);
        }
        public void DimensionShiftEnd()
        {
            _isTransitioning = false;
            player.MovementController.DisablePlayerMovement(false);
        }
    
    
    
    }
}