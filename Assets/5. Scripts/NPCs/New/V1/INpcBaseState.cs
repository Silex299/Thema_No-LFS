namespace NPCs.New.V1
{
    public interface INpcBaseState
    {
        public void Enter(V1Npc npc);
        public void UpdateState(V1Npc npc);
        public void Exit(V1Npc npc);
    }
}
