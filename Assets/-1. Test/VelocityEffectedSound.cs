using System;
using UnityEngine;

public class VelocityEffectedSound : MonoBehaviour
{

    public AudioSource audioSource;
    public float maximumVelocity = 10;
    private float _velocity;
    private Vector3 _lastPos;

    private void FixedUpdate()
    {
        _velocity = (transform.position - _lastPos).magnitude / Time.fixedDeltaTime;
        _lastPos = transform.position;

    }

    private void Update()
    {
        
        float volume = Mathf.Clamp(_velocity / maximumVelocity, 0, 1);
        audioSource.volume = Mathf.Lerp(audioSource.volume, volume, Time.deltaTime * 10);
    }
}
