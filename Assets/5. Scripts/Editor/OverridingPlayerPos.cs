using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class OverridingPlayerPos : MonoBehaviour
{
    private Vector3 _initialPlayerPos;
    private Quaternion _initialPlayerRot;
    private bool _overriding;

    [Button]
    public void OverridePlayerPos()
    {

        var player = FindObjectOfType<Player>();
        
        if (!_overriding)
        {
            _initialPlayerPos = player.transform.position;
            _initialPlayerRot = player.transform.rotation;
            _overriding = true;
            EditorApplication.update += OverridePlayerPos;
        }


        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;


    }

    [Button]
    private void ResetPlayerPos()
    {
        var player = FindObjectOfType<Player>();
        player.transform.position = _initialPlayerPos;
        player.transform.rotation = _initialPlayerRot;
        _overriding = false;
        EditorApplication.update -= OverridePlayerPos;
    }
}