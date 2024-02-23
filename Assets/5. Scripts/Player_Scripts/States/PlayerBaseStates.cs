namespace Player_Scripts.States
{
    public abstract class PlayerBaseStates
    {
        public abstract void EnterState(Player player);
        public abstract void ExitState(Player player);
        public abstract void UpdateState(Player player);
        public abstract void FixedUpdateState(Player player);
        public abstract void LateUpdateState(Player player);

    }

}
