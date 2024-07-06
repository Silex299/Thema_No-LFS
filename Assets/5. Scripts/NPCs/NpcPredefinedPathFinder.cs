using System.Collections.Generic;
using UnityEngine;

public class NpcPredefinedPathFinder : NpcPathFinder
{
    [Space(10)] public Transform[] waypoints;
    public float thresholdDistance;

    [SerializeField] private bool inverted;

    public bool Inverted
    {
        get => inverted;
        set => inverted = value;
    }


    private readonly Dictionary<int, bool> _npcId = new Dictionary<int, bool>();
    private int _currentWaypoint;

    public override Vector3 GetNextPoint(Vector3 origin, Vector3 destination, int instanceId)
    {
        if (InSight(origin, destination))
        {
            if(_npcId.ContainsKey(instanceId))
                _npcId.Remove(instanceId);

            return destination;
        }

        if (_npcId.TryGetValue(instanceId, out var value))
        {
            //Get distance between origin and current waypoint
            float distance = Vector3.Distance(origin, waypoints[_currentWaypoint].position);

            //If the distance is less than the threshold distance, move to the next waypoint
            if (distance < thresholdDistance)
            {
                _currentWaypoint = inverted ? _currentWaypoint - 1 : _currentWaypoint + 1;

                if (_currentWaypoint >= waypoints.Length)
                {
                    _currentWaypoint = waypoints.Length - 1;
                    Inverted = !Inverted;
                }
                else if (_currentWaypoint < 0)
                {
                    _currentWaypoint = 0;
                    Inverted = !Inverted;
                }
            }

            return waypoints[_currentWaypoint].position;
        }
        else
        {
            SetInitialWaypoint(origin, instanceId);
            return waypoints[_currentWaypoint].position;
        }

    }

    private void SetInitialWaypoint(Vector3 origin, int instanceId)
    {
        //Set the waypoint index to the closest waypoint to the origin
        float distance = 100;
        int i;

        for (i = 0; i < waypoints.Length; i++)
        {
            bool result = InSight(origin, waypoints[i].position);

            if (result)
            {
                float waypointDistance = Vector3.Distance(origin, waypoints[i].position);

                if (waypointDistance < distance)
                {
                    distance = waypointDistance;
                    _currentWaypoint = i;
                }
            }
        }

        _npcId.Add(instanceId, true);
    }
}