using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FallSimulator : MonoBehaviour
{
    public Rigidbody rb;

    [BoxGroup("New Force Settings")] public float time;
    [BoxGroup("New Force Settings")] public float count;

    public Vector3 forcePos;
    public Vector3 forceDir;
    public ForceMode forceMode;


    [OnValueChanged(nameof(SetTransform)), Range(0,1)]public float setTransform;
    [ProgressBar(0, 1)] public float simulationProgress;
    
    [FoldoutGroup("Transforms")]public List<Vector3> positions = new List<Vector3>();
    [FoldoutGroup("Transforms")]public List<Quaternion> rotations = new List<Quaternion>();
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(forcePos), 1f);
        
        
        Gizmos.color = Color.yellow;
        // Draw force direction ray
        Gizmos.DrawRay(transform.TransformPoint(forcePos), (forceDir.x  * transform.right) + (forceDir.y * transform.up) + (forceDir.z  * transform.forward));

        Gizmos.color = Color.white;

        // Draw lines connecting the recorded positions
        for (int i = 0; i < positions.Count; i++)
        {
            Gizmos.DrawSphere(positions[i], 0.1f);
            if (i > 0)
            {
                Gizmos.DrawLine(positions[i - 1], positions[i]);
            }
        }
    }

    [Button]
    public void ClearData()
    {
        positions.Clear();
        rotations.Clear();
    }

    [Button]
    public void SimulatePhysics()
    {
        StartCoroutine(Simulate());
    }

    IEnumerator Simulate()
    {
        float timeElapsed = 0;
        float deltaTime = time / count;

        Vector3 initialPos = transform.position;
        Quaternion initialRot = transform.rotation;

        Physics.autoSimulation = false;

        // Apply the initial force
        Vector3 lastForce = transform.TransformDirection(forceDir);
        rb.AddForceAtPosition(lastForce, transform.TransformPoint(forcePos), forceMode);
        
        while (timeElapsed <= time)
        {
            Physics.Simulate(deltaTime);
            timeElapsed += deltaTime;

            positions.Add(rb.position);
            rotations.Add(rb.rotation);

            simulationProgress = timeElapsed / time;
            yield return new WaitForSeconds(deltaTime);
        }

        simulationProgress = 1;
        Physics.autoSimulation = true;

        // Reset position and rotation
        transform.position = initialPos;
        transform.rotation = initialRot;
    }

    public void SetTransform()
    {
        if(positions.Count == 0) return;
        
        int index = Mathf.FloorToInt(setTransform * positions.Count);
        transform.position = positions[index];
        transform.rotation = rotations[index];
    }
    
}
