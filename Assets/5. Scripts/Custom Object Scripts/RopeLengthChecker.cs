using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class RopeLengthChecker : MonoBehaviour
{
    public ObiRopeCursor cursor;
    public ObiRope rope;

    public float ropeLength;

    public float currentRopeLength;

    private void Start()
    {
        ropeLength = rope.restLength;
    }

    private bool cut;
    private void Update()
    {
        currentRopeLength = Vector3.Distance(transform.position, rope.transform.position);
        cursor.ChangeLength(ropeLength);
    }
}
