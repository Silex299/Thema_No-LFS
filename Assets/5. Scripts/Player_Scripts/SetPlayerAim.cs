using UnityEngine;

namespace Player_Scripts
{
    public class SetPlayerAim : MonoBehaviour
    {
        private PlayerRigController PlayerRig => PlayerMovementController.Instance.player.RigController;

        public void SetAim(float transitionTime)
        {
            PlayerRig.SetAim(transform, transitionTime);
        }
        
        public void ResetAim(float transitionTime)
        {
            PlayerRig.ResetAim(transitionTime);
        }
        
    }
}
