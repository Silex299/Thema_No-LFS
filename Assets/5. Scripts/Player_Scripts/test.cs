using UnityEngine;
using UnityEditor;

public class test : MonoBehaviour
{

    [SerializeField, BoxGroup("ESES")] private int testInt;

    IEnumerator Test()
    {
        yield return null;
    }

}