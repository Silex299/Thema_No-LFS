using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Particle_Scripts
{
    public class WaterDropSound : MonoBehaviour
    {
   
        public ParticleSystem particleSystem;
        public AudioClip[] dropSounds;
        public AudioSource audioSource;


        private void OnParticleCollision(GameObject other)
        {
            print("Hello");
            PlayDropSound();
        }

        void PlayDropSound()
        {
            //play a random sound from the array
            audioSource.PlayOneShot(dropSounds[Random.Range(0, dropSounds.Length)]);
        }
    }
}
