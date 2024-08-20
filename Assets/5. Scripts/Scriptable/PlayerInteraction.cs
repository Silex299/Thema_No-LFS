using UnityEngine;

namespace Scriptable
{
    
    [CreateAssetMenu(fileName = "NewInteractionKey", menuName = "Scriptable/Thema/InteractionKey", order = 1)]
    public class PlayerInteraction : ScriptableObject
    {
        public string interactionKey;
        public float volume;
    }
}
