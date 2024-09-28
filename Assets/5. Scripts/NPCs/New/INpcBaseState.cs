using UnityEngine;

namespace NPCs.New
{
    public interface INpcBaseState
    {
        public void Enter(V1Npc v1Npc);
        public void Update(V1Npc v1Npc);
        public void Exit(V1Npc v1Npc);
    }
}
