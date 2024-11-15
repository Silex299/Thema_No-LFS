using Player_Scripts;
using UnityEngine;

namespace Sounds
{
    public class SoundEffectsVolume : MonoBehaviour
    {



        public bool isGlobal; 
        public float playerInteractionVolumeMultiplier = 1;
        public float playerVolumeMultiplier = 1;


        private void Start()
        {
            if (isGlobal)
            {
                SetPlayerVolumeMultiplier();
            }
        }


        public void SetPlayerVolumeMultiplier()
        {
            PlayerMovementController.Instance.player.EffectsManager.PlayerVolumeMultiplier = playerVolumeMultiplier;
            PlayerMovementController.Instance.player.EffectsManager.PlayerInteractionVolumeMultiplier = playerInteractionVolumeMultiplier;
        }
        
    }
}
