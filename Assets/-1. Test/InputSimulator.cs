using UnityEngine;

[ExecuteInEditMode]
public class InputSimulator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButton("Sprint"))
        {
            Debug.Log("Sprint");
        }
    }

}
