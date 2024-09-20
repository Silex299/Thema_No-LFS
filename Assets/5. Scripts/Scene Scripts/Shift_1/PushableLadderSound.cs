using UnityEngine;

namespace Scene_Scripts.Shift_1
{
    public class PushableLadderSound : MonoBehaviour
    {


        public AudioSource source;
        public AudioClip hitSound;

        public void PlaySound()
        {
            source.PlayOneShot(hitSound);
        }
        

    }
}
