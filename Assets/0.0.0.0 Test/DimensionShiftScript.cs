using System.Collections;
using System.Net.NetworkInformation;
using Misc;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class DimensionShiftScript : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem shiftParticleEffect;


    private bool _isInRealDimension = true;
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

    public void ChangeDimension()
    {
        if (shiftParticleEffect)
        {
            shiftParticleEffect.Play();
        }

        _isInRealDimension = !_isInRealDimension;

        if (_isInRealDimension)
        {
            PlayerSceneAnimatonManager.Instance?.PlayPlayerSceneAnimation(0);
        }
        else
        {
            PlayerSceneAnimatonManager.Instance?.PlayPlayerSceneAnimation(1);
        }
    }

    public void DimensionShiftEnd()
    {
        _isTransitioning = false;
        player.MovementController.DisablePlayerMovement(false);
    }
    
    
    
}