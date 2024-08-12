using UnityEngine;

namespace Mechanics.Npc
{
    public interface INpcStateBase
    {
       public void Enter(Npc npc);
       public void Update();
       public void Exit();
    }
}
