using System.Collections;
using System.Runtime.InteropServices;
using Player_Scripts;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class DimensionShiftScript : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem shiftParticleEffect;
    [SerializeField] private float shiftDelay = 1f;

    private bool _isInRealDimension = true;
    private Coroutine _shiftDimensionCoroutine;

    private void Update()
    {
        if (Input.GetButtonDown("f"))
        {
            ShiftDimension();
        }
    }

    private void ShiftDimension()
    {
        if (_shiftDimensionCoroutine != null)
        {
            return;
        }
        _isInRealDimension = !_isInRealDimension;

        _shiftDimensionCoroutine = StartCoroutine(ShiftDimensionCoroutine());
    }


    private IEnumerator ShiftDimensionCoroutine()
    {
        //Play meditate animation

        //Play transition animation

        //change camera clip mask

        //Bring player to movable state

        player.AnimationController.CrossFade("ShiftDimension", 0.5f, 1);
        player.MovementController.DisablePlayerMovement(true);
 
        yield return new WaitForSeconds(1);


        if (shiftParticleEffect)
        {
            shiftParticleEffect.Play();
        }

        yield return new WaitForSeconds(shiftDelay - 1);
        

        player.AnimationController.CrossFade("Default", 0.2f, 1);
        yield return new WaitForSeconds(0.5f);
        player.MovementController.DisablePlayerMovement(false);

        _shiftDimensionCoroutine = null;
    }
}