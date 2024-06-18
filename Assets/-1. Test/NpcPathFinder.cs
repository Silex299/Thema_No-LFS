using Sirenix.OdinInspector;
using UnityEngine;

public class NpcPathFinder : MonoBehaviour
{
    public Transform[] wayPoints;
    [InfoBox("raycast has to hit nothing in order to get direct player position")]
    public LayerMask layermask;
    private int closestWayPoint;

    public Vector3 GetNextPoint(Vector3 origin, Vector3 destination)
    {
        //return the point of the destination if raycast from origin to destination hits a collider with tag tag
        if (InSight(origin, destination))
        {
            return destination;
        }

        float distance = 100;
        int i;
        
        for(i=0; i<wayPoints.Length; i++)
        {

            bool result = InSight(origin, wayPoints[i].position) && InSight(destination, wayPoints[i].position);
            
            if (result)
            {
                float waypointDistance = Vector3.Distance(destination, wayPoints[i].position);
                
                if(waypointDistance < distance)
                {
                    distance = waypointDistance;
                    closestWayPoint = i;
                }
            }            
        }

        ///draw line from origin to closest waypoint
        Debug.DrawLine(origin, wayPoints[closestWayPoint].position, Color.red);
        // draw line from destination to closest waypoint
        Debug.DrawLine(destination, wayPoints[closestWayPoint].position, Color.green);

        return (distance == 100) ? destination : wayPoints[closestWayPoint].position;
    }
    
    protected virtual bool InSight(Vector3 obj1, Vector3 obj2)
    {
        return !Physics.Linecast(obj1, obj2, layermask);
    }
}