using System.Collections;
using UnityEngine;

[System.Serializable]
// ReSharper disable once CheckNamespace
public class GuardNpcSurveillanceState : GuardNpcState
{
    public Transform[] wayPoints;
    public float stopDistance = 1f;
    public float stopTime = 2f;


    private int currentWayPointIndex;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private Coroutine _changeWayPointCoroutine;
    private static readonly int Chase = Animator.StringToHash("Chase");
    private static readonly int Attack = Animator.StringToHash("Attack");

    public override void Enter(GuardNpc npc)
    {
     
        npc.animator.SetBool(Chase, false);
        npc.animator.SetBool(Attack, false);
        
    }

    public override void Update(GuardNpc npc)
    {
        //if there are more than one waypoint
        if (wayPoints.Length > 1)
        {
           
            //rotate the npc towards the current waypoint
            Rotate(npc.transform, wayPoints[currentWayPointIndex].position, npc.rotationSpeed);


            float distance = Vector3.Distance(npc.transform.position, wayPoints[currentWayPointIndex].position);
            
            if(distance < stopDistance)
            {
                _changeWayPointCoroutine ??= npc.StartCoroutine(ChangeWayPoint(npc));
            }
            else
            {
                npc.animator.SetFloat(Speed, 1);
            }
            
        }

    }

    private IEnumerator ChangeWayPoint(GuardNpc npc)
    {

        float timeElapsed = 0;

        while (timeElapsed<1)
        {
            timeElapsed += Time.deltaTime;
            float speed = Mathf.Lerp(1, 0, timeElapsed);
            npc.animator.SetFloat(Speed, speed);
            
            yield return null;
        }
        

        yield return new WaitForSeconds(stopTime);
        
        timeElapsed = 0;
        while (timeElapsed<1)
        {
            timeElapsed += Time.deltaTime;
            float speed = Mathf.Lerp(0, 1, timeElapsed);
            npc.animator.SetFloat(Speed, speed);
            
            yield return null;
        }
        
        currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Length;
        _changeWayPointCoroutine = null;
    }

    public override void Exit(GuardNpc npc)
    {
    }
}