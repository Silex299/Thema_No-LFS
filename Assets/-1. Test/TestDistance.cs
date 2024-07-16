using Sirenix.OdinInspector;
using UnityEngine;

public class TestDistance : MonoBehaviour
{


    public Animator animator;
    public string animationName;
    [OnValueChanged(nameof(UpdateAnim)), Range(0,1)]
    public  float normalisedTime;


    [Space(10)] public Vector3 startPos;
   [Button("StartPos")] public void SetStartPos()
    {
        startPos = transform.position;
    }

   public Vector3 endPos;
   [Button("EndPos")]public void SetEndPos()
    {
        endPos = transform.position;
    }

    [Space(10)] public float distance;
    [Button("Calculate Distance")]public void CalculateDistance()
    {
        distance = Vector3.Distance(startPos, endPos);
    }
    
    
    public void UpdateAnim()
    {
        animator.Play(animationName, 1, normalisedTime);
        animator.Update(0);
    } 


}
