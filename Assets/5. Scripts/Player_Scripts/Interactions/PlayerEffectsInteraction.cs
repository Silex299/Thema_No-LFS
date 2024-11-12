using Scriptable;
using UnityEngine;

namespace Player_Scripts.Interactions
{
    public class PlayerEffectsInteraction : MonoBehaviour
    {
        private static PlayerEffectsManager PlayerEffects => PlayerMovementController.Instance.player.EffectsManager;

        public void PlayPlayerInteraction(Object interaction)
        {
            PlayerEffects.PlayPlayerInteraction(interaction);
        }

        public void PlayPlayerInteractionOverrideGround(Object interaction)
        {
            PlayerEffects.PlayPlayerInteractionGroundOverride(interaction);
        }

        public void PlayPlayerSound(string soundKey)
        {
            PlayerEffects.PlayPlayerSound(soundKey);
        }
        
        
        
    }
}
