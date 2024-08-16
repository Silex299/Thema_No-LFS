using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

[ExecuteInEditMode]
public class TimelineRecorder : MonoBehaviour
{
    public TimelineAsset timeline;
    public AnimationClip clip;
    public GameObject targetObject;

    [Button]
    public void UpdatePosition(Vector3 newPosition)
    {
        targetObject.transform.position = newPosition;

        if (timeline != null && clip != null)
        {
            
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            float time = (float)TimelineEditor.inspectedDirector.time;
            curveX.AddKey(time, newPosition.x);
            curveY.AddKey(time, newPosition.y);
            curveZ.AddKey(time, newPosition.z);

            clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
            clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }
    }
}