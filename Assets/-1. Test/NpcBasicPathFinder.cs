using UnityEngine;

public class NpcBasicPathFinder : NpcPathFinder
{
    public Transform[] wayPoints;
    private int _closestWayPoint;


    public override Vector3 GetNextPoint(Vector3 origin, Vector3 destination, int instanceId = 0)
    {
        //return the point of the destination if raycast from origin to destination hits a collider with tag
        if (InSight(origin, destination))
        {
            return destination;
        }

        float distance = 100;
        int i;

        for (i = 0; i < wayPoints.Length; i++)
        {
            bool result = InSight(origin, wayPoints[i].position) && InSight(destination, wayPoints[i].position);

            if (result)
            {
                float waypointDistance = Vector3.Distance(destination, wayPoints[i].position);

                if (waypointDistance < distance)
                {
                    distance = waypointDistance;
                    _closestWayPoint = i;
                }
            }
        }

        Debug.DrawLine(origin, wayPoints[_closestWayPoint].position, Color.red);
        // draw line from destination to the closest waypoint
        Debug.DrawLine(destination, wayPoints[_closestWayPoint].position, Color.green);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return (distance == 100) ? destination : wayPoints[_closestWayPoint].position;
    }
}