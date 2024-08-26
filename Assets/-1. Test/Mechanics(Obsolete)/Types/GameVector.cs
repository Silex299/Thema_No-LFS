using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Mechanics.Types
{
    public static class GameVector
    {
        public static Vector3 GetClosestPointToLine(Vector3 from, Vector3 to, Vector3 point)
        {
            Vector3 line = to - from;
            Vector3 pointDirection = point - from;

            float t = Vector3.Dot(pointDirection, line) / Vector3.Dot(line, line);

            t = float.IsNaN(t) ? 0 : Mathf.Clamp01(t);

            return from + line * t;
        }

        public static Vector3 GetClosestPointToLine(Vector3 line, Vector3 point)
        {
            Vector3 lineDirection = line.normalized;
            float t = Vector3.Dot(point, lineDirection);
            return t * lineDirection;
        }
        
        public static float PlanarDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
        }
    }

    [Serializable, CanBeNull]
    public class TimedAction
    {
        public float time;
        public Action action;

        public TimedAction(float time, Action action)
        {
            this.time = time;
            this.action = action;
        }
    }
}