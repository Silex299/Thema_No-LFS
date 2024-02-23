namespace Player_Scripts.Player_States
{

    public abstract class PlayerBaseState
    {

        public abstract void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.5f);

        public abstract void OnStateUpdate(PlayerController controller);

        public abstract void OnStateFixedUpdate(PlayerController controller);

        public abstract void OnStateLateUpdate(PlayerController controller);

        public abstract void OnGizmos(PlayerController controller);

        public abstract void OnStateExit(PlayerController controller);

        public abstract bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0);

        public abstract void SimpleInteract(PlayerController controller, int value = 0);

        #region Custom type

        public enum InteractionType
        {
            Crouch, //1
            Push, //2
            CoverDirection, //3
            Jump,//4
        }

        [System.Serializable]
        public enum PlayerProximity
        {
            Jump,
            Vault,
            VaultLeft,
            VaultRight,
            BlockedLeft,
            BlockedRight,
            GoUnder
        }


        #endregion

    }
}
