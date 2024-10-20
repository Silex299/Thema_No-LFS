using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class ChangeLightProperties : MonoBehaviour
    {


        public Light theLight;
        
        [TabGroup("Light Properties", "Default")] public Color defaultColor;
        [TabGroup("Light Properties", "Other")] public Color otherColor;


        public void ChangeLight(bool other)
        {
            theLight.color = other ? otherColor : defaultColor;
        }
        
    }
}
