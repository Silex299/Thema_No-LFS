using System;
using UnityEngine;

public class CustomMovementNpc : MonoBehaviour
{



    public Animator animator;
    public float rotationSpeed;
    public Transform[] waypoints;
    public float waypointThreshold;

    public int _currentWaypoint;


    private void Update()
    {
        
    }



    private void Move()
    {
        
    }
    
    private void ChangeWaypoint()
    {
        Vector3 nextPos = waypoints[_currentWaypoint].position;
        Vector3 characterPos = transform.position;
        nextPos.y = 0;
        characterPos.y = 0;
        
        if (Vector3.Distance(nextPos, characterPos) < waypointThreshold)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= waypoints.Length)
            {
                _currentWaypoint = 0;
            }
        }
    }

    private void Rotate()
    {
        Vector3 nextPos = waypoints[_currentWaypoint].position;
        Vector3 direction = nextPos - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


}