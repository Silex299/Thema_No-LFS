using Player_Scripts;
using UnityEngine;

namespace Sounds
{
    public class SoundEffectsVolume : MonoBehaviour
    {
        
        public float playerInteractionVolumeMultiplier = 1;
        public float playerVolumeMultiplier = 1;
        
        
        public void SetPlayerVolumeMultiplier()
        {
            PlayerMovementController.Instance.player.EffectsManager.PlayerVolumeMultiplier = playerVolumeMultiplier;
            PlayerMovementController.Instance.player.EffectsManager.PlayerInteractionVolumeMultiplier = playerInteractionVolumeMultiplier;
        }
        
    }
}
