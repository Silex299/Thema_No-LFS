using UnityEngine;

namespace Mechanics.Npc
{
    public abstract class NpcStateBase
    {
        protected NPCs.New.Npc npc;
        public abstract void Enter(NPCs.New.Npc parentNpc);
        public abstract void Update();
        public abstract void Exit();


        /// <summary>
        /// Rotates the transform to the desired direction
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="desiredPos"></param>
        /// <param name="speed">Time dependent, use Time.deltaTime or other param</param>
        protected static void Rotate(Transform transform, Vector3 desiredPos, float speed)
        {
            Vector3 forward = desiredPos - transform.position;
            forward.y = 0;
            Quaternion desiredRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, speed);
        }
    }
}