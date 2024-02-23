using System;
using Interactable_Items;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class DraggableCart : DraggableBox
{
    [SerializeField, BoxGroup("Wheels")] private bool invertedWheelMovement;
    [SerializeField, BoxGroup("Wheels")] private float wheelRadius;
    [SerializeField, BoxGroup("Wheels")] private axis wheelAxis;


    [SerializeField, BoxGroup("Wheels")] private Transform[] wheels;

    [SerializeField, BoxGroup("Effects")] private VisualEffect[] sparks;
    [SerializeField, BoxGroup("Effects")] private float sparkThresoldAngle;


    private float _lastValue;


    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        PlaySparks(false);
    }

    private void FixedUpdate()
    {
        if(!_playerIsInTrigger) return;


        if (_drag)
        {

            float diff = 0;
            float angle = 0;
            switch (wheelAxis)
            {
                case axis.x:
                    diff = _lastValue - transform.position.z;
                    _lastValue = transform.position.z;
                    break;
                case axis.z:
                    diff = _lastValue - transform.position.x;
                    _lastValue = transform.position.x;
                    break;
                default:
                    diff = 0;
                    break;
            }


            angle = Mathf.Rad2Deg * (diff / wheelRadius);


            if (Mathf.Abs(angle) > sparkThresoldAngle)
            {
                PlaySparks(true);
            }
            else
            {
                PlaySparks(false);
            }


            foreach (var wheel in wheels)
            {
                if (wheelAxis == axis.x)
                {
                    wheel.Rotate(new Vector3(invertedWheelMovement ? -1 : 1, 0, 0), angle);
                }
                else if (wheelAxis == axis.z)
                {
                    wheel.Rotate(new Vector3(0, 0, invertedWheelMovement ? -1 : 1), angle);
                }
                else if (wheelAxis == axis.y)
                {
                    wheel.Rotate(new Vector3(0, invertedWheelMovement ? -1 : 1, 0), angle);
                }
            }
        }
        else
        {
            PlaySparks(false);
        }

        
    }

    public void PlaySparks(bool play)
    {
        foreach (var spark in sparks)
        {
            if (play)
            {
                spark.Play();
            }
            else
            {
                spark.Stop();
            }
        }
    }
}