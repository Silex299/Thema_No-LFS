using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Events;

public class SlamingDoor : MonoBehaviour
{
    
	[SerializeField] private List<Vector3> positions;

    [Button("Get Positon", ButtonSizes.Large)] 
    public void GetCurrentPosition()
    {
        positions.Add(transform.position);
    }


    [SerializeField] private float normalizedSpeed;
    [SerializeField] private AnimationCurve interpolatePositionCurve;

    [SerializeField] private UnityEvent onSlam;

    public bool slam;

    private bool _reset;
    private float _lerp;
    private Vector3 _direction;
    private float _distance;


    private void Start()
    {
        _direction = (positions[0] - positions[1]).normalized;
        _distance = Vector3.Distance(positions[0], positions[1]);
    }


    private void Update()
    {

        if (!slam)
        {
            if (_reset) return;

            transform.position = Vector3.MoveTowards(transform.position, positions[0], Time.deltaTime * 3f);
            if(Vector3.Distance(transform.position, positions[0])< 0.01f)
            {
                _reset = true;
            }

            return;
        }


        if (_lerp >= 1)
        {
            _lerp = 0;
        }

        _lerp += Time.deltaTime * normalizedSpeed;


        transform.position = positions[1] + _direction * _distance * interpolatePositionCurve.Evaluate(_lerp);
      

    }

    public void Reset()
    {
        slam = false;
    }


}
