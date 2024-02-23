using UnityEngine;
using Thema_Type;

[CreateAssetMenu(fileName = "StepInfo", menuName = "Scriptables/Thema", order = 1)]
public class Step : ScriptableObject
{

    public float volume;
    public WhichStep whichStep;


}
