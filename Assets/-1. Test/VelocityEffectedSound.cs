using System;
using UnityEngine;

public class VelocityEffectedSound : MonoBehaviour
{

    public AudioSource audioSource;
    public float maximumVelocity = 10;
    public float velocityUpdateInterval = 0.5f;
    public float volumeUpdateSpeed = 10;
    
    private float _velocity;
    private Vector3 _lastPos;
    private float _lastUpdateTime;

    //Some odd sounds at start
    private void Start()
    {
        _lastPos = transform.position;
        _lastUpdateTime = Time.time;
    }

    private void Update()
    {
        UpdateVelocity();
        
        float volume = Mathf.Clamp(_velocity / maximumVelocity, 0, 1);
        audioSource.volume = Mathf.Lerp(audioSource.volume, volume, Time.deltaTime * volumeUpdateSpeed);
    }

    private void UpdateVelocity()
    {
        
        if(Time.time < _lastUpdateTime + velocityUpdateInterval) return;
        
        _velocity = (transform.position - _lastPos).magnitude / Time.fixedDeltaTime;
        _lastPos = transform.position;
        _lastUpdateTime = Time.time;
    }
    
}
