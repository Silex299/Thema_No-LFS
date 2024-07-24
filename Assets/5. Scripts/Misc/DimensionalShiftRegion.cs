using System.Collections;
using Player_Scripts;
using UnityEngine;

namespace Misc
{
    public class DimensionalShiftRegion : MonoBehaviour
    {


        public SurveillanceVisuals visuals;
    
    
    
        public void ShiftDimensionStart(bool toAlternate)
        {
            StartCoroutine(ShiftCoroutine(toAlternate));
        }

        private IEnumerator ShiftCoroutine(bool toAlternate)
        {
            var playerController = PlayerMovementController.Instance;
        
            playerController.DisablePlayerMovement(true);
            playerController.player.CController.enabled = false;
        
            //power up the visual
            StartCoroutine(visuals.PowerChange(SurveillanceVisuals.ServeillanceVisualState.PowerUp));
            //Play float animation in layer 1
            playerController.PlayAnimation("Float", 1f, 1);
        
            //Call dimensional Shift UI animation
            PlayerSceneAnimatonManager.Instance.PlayPlayerSceneAnimation(toAlternate ? 1 : 0);
        
            //wait for few second
            yield return new WaitForSeconds(4f);
        
            playerController.DisablePlayerMovement(false);
            playerController.player.CController.enabled = true;
            //power down the visuals 
            StartCoroutine(visuals.PowerChange(SurveillanceVisuals.ServeillanceVisualState.PowerDown));
            //PLay default animation
            playerController.PlayAnimation("Default", 1);

        }
    
    
    }
}
