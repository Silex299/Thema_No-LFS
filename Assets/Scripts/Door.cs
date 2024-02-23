using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Door1 : MonoBehaviour
{

    [SerializeField, BoxGroup("Positions")]private Vector3 closedPosition;
    [SerializeField, BoxGroup("Positions")] private Vector3 openPosition;
    

    /// <summary>
    /// True if door is open, False if door is closed
    /// </summary>
    [SerializeField, BoxGroup("Misc"), InfoBox("True if door is open, false if door is closed")] private bool status;
    [SerializeField, BoxGroup("Misc")] private float smoothness;

#if UNITY_EDITOR

    [Button("Set Closed Position", ButtonSizes.Large), GUIColor(1,0.1f, 0.2f), BoxGroup("Positions")]
    public void SetClosedPosition()
    {
        closedPosition = transform.position;
    }

    [Button("Set Open Position", ButtonSizes.Large), GUIColor(0.2f, 1f, 0.2f), BoxGroup("Positions")]
    public void SetOpenPosition()
    {
        openPosition = transform.position;
    }
    
#endif


    
    public bool ToggleDoor(bool toggle, float delay)
    {
        if(toggle != status) return false;
        
        StartCoroutine(MoveDoor(!toggle, delay));
        return true;
    }


    
    private IEnumerator MoveDoor(bool toggle, float delay)
    {

        yield return new WaitForSeconds(delay);
        
        float lerp = 0;
        
        while (lerp < 1)
        {
            var repos = toggle ? openPosition : closedPosition;
            transform.position = Vector3.Lerp(transform.position, repos, lerp);
            lerp += 0.02f * smoothness;
            yield return new WaitForSeconds(0.02f);
        }

        status = toggle;

    }

}
