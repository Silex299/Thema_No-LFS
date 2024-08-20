using Thema_Type;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "StepInfo", menuName = "Scriptable/Thema/StepInfo", order = 1)]
    public class Step : ScriptableObject
    {
        public float volume;
        public WhichStep whichStep;
    }
}
