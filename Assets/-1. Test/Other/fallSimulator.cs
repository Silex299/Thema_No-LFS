using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class FallSimulator : MonoBehaviour
{
    [FoldoutGroup("Baked Info")] public bool play;
    [FoldoutGroup("Baked Info")] public List<Vector3> positions = new List<Vector3>();
    [FoldoutGroup("Baked Info")] public List<Quaternion> rotations = new List<Quaternion>();

    [Range(0, 1), FoldoutGroup("Params")]
    public float setTransform;

    #region Editor

#if UNITY_EDITOR
    
    [FoldoutGroup("Editor")] public Rigidbody rb;

    [FoldoutGroup("Editor")] public float time;
    [FoldoutGroup("Editor")] public float count;

    [FoldoutGroup("Editor")] public Vector3 forcePos;
    [FoldoutGroup("Editor")] public float forceMultiplier;
    [FoldoutGroup("Editor")] public Vector3 forceDir;
    [FoldoutGroup("Editor")] public ForceMode forceMode;


    [FoldoutGroup("Editor"), ProgressBar(0, 1)]
    public float simulationProgress;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(forcePos), 1f);


        Gizmos.color = Color.yellow;
        // Draw force direction ray
        Gizmos.DrawRay(transform.TransformPoint(forcePos), (forceDir.x * transform.right) + (forceDir.y * transform.up) + (forceDir.z * transform.forward));

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

        SetTransform();
    }

    EditorApplication.CallbackFunction OnEditorUpdate()
    {
        SetTransform();
        return null;
    }

    [Button]
    public void ClearData()
    {
        positions.Clear();
        rotations.Clear();
    }

    [Button]
    public void BakePhysic()
    {
        StartCoroutine(Simulate());
    }

    IEnumerator Simulate()
    {
        float timeElapsed = 0;
        float deltaTime = time / count;
        bool initEnabled = play;
        play = false;

        Vector3 initialPos = transform.position;
        Quaternion initialRot = transform.rotation;

        Physics.autoSimulation = false;

        // Apply the initial force
        Vector3 lastForce = transform.TransformDirection(forceDir) * forceMultiplier;

        positions.Add(rb.position);
        rotations.Add(rb.rotation);

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


        play = initEnabled;
    }

    [Button]
    public void StopSimulation()
    {
        StopAllCoroutines();
    }

    public void SetTransform()
    {
        if (!play) return;
        if (positions.Count == 0) return;

        int index = Mathf.FloorToInt(setTransform * positions.Count);
        if (index < positions.Count)
        {
            transform.position = positions[index];
            transform.rotation = rotations[index];
        }
    }

#endif

    #endregion

    private void Update()
    {
        if (play)
        {
            if (positions.Count == 0) return;
            int index = Mathf.FloorToInt(setTransform * positions.Count);
            if (index < positions.Count)
            {
                transform.position = positions[index];
                transform.rotation = rotations[index];
            }
        }
    }
}