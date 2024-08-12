using UnityEngine;

namespace Mechanics.Npc
{
    public abstract class NpcStateBase
    {
        public abstract void Enter(Npc npc);
        public abstract void Update();
        public abstract void Exit();


        /// <summary>
        /// Rotates the transform to the desired direction
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="desiredDirection"></param>
        /// <param name="speed">Time dependent, use Time.deltaTime or other param</param>
        public static void Rotate(Transform transform, Vector3 desiredDirection, float speed)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, speed);
        }
    }
}