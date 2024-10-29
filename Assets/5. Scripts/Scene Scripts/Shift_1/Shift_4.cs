using Misc;
using Player_Scripts;
using UnityEngine;

namespace Scene_Scripts
{
    public class Shift_4 : MonoBehaviour
    {


        public void TriggerEnd()
        {
            PlayerMovementController.Instance.DisablePlayerMovement(true);
            CutsceneManager.Instance.PlayClip(1);
        }


    }
}
