using Sirenix.Serialization;
using UnityEngine;

namespace Thema_Type
{
    /// <summary>
    /// Indicated how player should interact with the key, If hold, release, on press key
    /// </summary>
    [System.Serializable]
    public enum KeyInputType
    {
        Key_Press,
        Key_Hold,
        Key_Release
    }

    [System.Serializable]
    public enum WhichStep
    {
        LEFT, RIGHT
    }
    
    public static class Thema
    {
        /// <summary>
        /// This method is intended to simulate a spring mechanism. It takes in parameters for position (x), amplitude, damping factor (damp), and stiffness of the spring.
        /// Currently, the method body is empty and needs to be implemented.
        /// </summary>
        /// <param name="x">The position of the spring.</param>
        /// <param name="amplitude">The amplitude of the spring oscillation.</param>
        /// <param name="damp">The damping factor of the spring.</param>
        /// <param name="stiffness">The stiffness of the spring.</param>
        /// <returns>The new position of the spring.</returns>
        public static float Spring(float x, float amplitude, float damp, float stiffness = 1)
        {
            return ((amplitude * (x - stiffness)) / stiffness) * Mathf.Cos(damp * x);
        }
    }
    
}

