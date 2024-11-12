using UnityEngine;

namespace Scriptable
{
    
    [CreateAssetMenu(fileName = "NewOverrideInteractionKey", menuName = "Scriptable/Thema/GroundOverrideInteraction", order = 1)]
    public class PlayerInteractionGroundOverride : ScriptableObject
    {
        public string ground;
        public string interactionKey;
        public float volume;
    }
}
