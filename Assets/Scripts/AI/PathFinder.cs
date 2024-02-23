using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{

    [SerializeField] private List<Transform> chasePoints;
    //TODO: Remove
    [SerializeField] private Transform _target;
    [SerializeField] private LayerMask raycastMask;

    private bool _targteInSight;

    public bool NextPathPoint(Transform target, out Vector3 path)
    {
        var pos = transform.position + 0.5f * Vector3.up;
        var targetPos = target.position + 1f * Vector3.up;


        //TODO remove debug
        //IF player is in sight
        if (!Physics.Linecast(targetPos, pos, out RaycastHit hit, raycastMask))
        {
            path = targetPos;
            Debug.DrawLine(pos, targetPos, Color.cyan);
            return true;
        }

        var distance = Mathf.Infinity;
        int pathPointIndex = 0;

        for (int i = 0; i < chasePoints.Count; i++)
        {
            var dis = Vector3.Distance(targetPos, chasePoints[i].position);
            if (distance > dis)
            {
                if (!Physics.Linecast(pos, chasePoints[i].position, out RaycastHit hit1, raycastMask))
                {
                    if (!Physics.Linecast(targetPos, chasePoints[i].position, out RaycastHit hit2, raycastMask))
                    {
                        distance = dis;
                        pathPointIndex = i;
                    }
                }
            }
        }

        Debug.DrawLine(pos, chasePoints[pathPointIndex].position, Color.blue);
        Debug.DrawLine(targetPos, chasePoints[pathPointIndex].position, Color.green);


        if (distance == Mathf.Infinity)
        {
            path = Vector3.zero;
            return false;
        }


        path = chasePoints[pathPointIndex].position;
        return true;
    }


    private void OnDrawGizmos()
    {
        if (NextPathPoint(_target, out var x))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(x, 1f);
        }
    }

}
