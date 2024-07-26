using UnityEditor;
using UnityEngine;

public class fallSimulator : MonoBehaviour
{
    public Rigidbody rb;
    public float deltaTime = 1.0f; // Adjust as needed
    private void Start()
    {
        // Initialize Rigidbody properties (e.g., mass, gravity, etc.)
    }

    public void SimulatePhysics()
    {
        Physics.autoSimulation = false;
        Physics.Simulate(deltaTime);
        Physics.autoSimulation = true;

        Vector3 newPosition = rb.position;
        Quaternion newRotation = rb.rotation;

        // Update the GameObject's Transform with the Rigidbody's new position and rotation
        gameObject.transform.position = newPosition;
        gameObject.transform.rotation = newRotation;

        Debug.Log($"Rigidbody position after {deltaTime} seconds: {newPosition}");
        Debug.Log($"Rigidbody rotation after {deltaTime} seconds: {newRotation}");

    }
}


[CustomEditor(typeof(fallSimulator))] // Replace with your script's class name
public class RigidbodyVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        fallSimulator myScript = (fallSimulator)target; // Replace with your script's class name
        if (GUILayout.Button("Simulate Physics"))
        {
            myScript.SimulatePhysics();
        }
    }
}