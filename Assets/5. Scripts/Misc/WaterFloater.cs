using UnityEngine;


namespace Misc
{
    public class WaterFloater : MonoBehaviour
    {
        public float posAmplitude = 0.5f;
        public float rotAmplitude = 0.5f;
        public float frequency = 1f;


        // Initial position and rotation of the object
        private Vector3 startPos;
        private Quaternion startRot;

        // Noise offset to ensure different objects have unique movement patterns
        private float noiseOffset;

        void OnEnable()
        {
            print("Starting");
            startPos = transform.position;
            startRot = transform.rotation;
            noiseOffset = Random.Range(0f, 10000f); // Large range to ensure variety
        }

        void LateUpdate()
        {
            Float();
        }

        void Float()
        {
            // Calculate time since start
            float time = Time.time + noiseOffset;

            // Adjust position to simulate vertical bobbing
            Vector3 newPos = Vector3.zero;

            newPos.y = Mathf.Sin(time * frequency) * posAmplitude;

            Vector3 currentPos = transform.position;
            currentPos.y = startPos.y + newPos.y;

            transform.position = currentPos;

            // Apply noise-based rotation for a more natural, floating effect
            float rotationX =
                Mathf.PerlinNoise(time * frequency, 0f) * 2f - 1f; // Perlin noise returns 0..1, adjust to -1..1
            float rotationZ = Mathf.PerlinNoise(0f, time * frequency) * 2f - 1f; // Different axis for varied effect
            Quaternion newRotation = Quaternion.Euler(rotationX * rotAmplitude * 10f, startRot.eulerAngles.y,
                rotationZ * rotAmplitude * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * frequency);
        }
    }
}