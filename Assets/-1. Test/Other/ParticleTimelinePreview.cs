using System;
using UnityEditor;
using UnityEngine;

public class ParticleTimelinePreview : MonoBehaviour
{
     
#if UNITY_EDITOR   
    // ReSharper disable once InconsistentNaming
    public ParticleSystem _particleSystem;


    private void OnDrawGizmos()
    {
        if (_particleSystem != null)
        {
            if (_particleSystem.isStopped && !_particleSystem.isPlaying)
            {
                _particleSystem.Play();
            }
        }
    }

        
        
#endif
}