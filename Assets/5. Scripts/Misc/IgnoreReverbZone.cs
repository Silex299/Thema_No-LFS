using UnityEngine;

namespace Misc
{
    public class IgnoreReverbZone : MonoBehaviour
    {
        
        private void Start()
        {
            
            
            if (TryGetComponent<AudioSource>(out var source))
            {
                source.reverbZoneMix = 0; 
            }
        }
    }
}
