using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thema;
using UnityEngine;
using UnityEngine.Events;

public class RotateOnDemand : MonoBehaviour
{
    [SerializeField, BoxGroup("Rotations")]
    private int currentIndex;

    [SerializeField, BoxGroup("Rotations")]
    private float transitionTime = 1;

    [SerializeField, BoxGroup("Rotations")]
    private List<Vector3> eulerRotations;

    [SerializeField, BoxGroup("Action")] private UnityEvent onCompleteRotation;

    [FoldoutGroup("Sound")] public AudioSource source;
    [FoldoutGroup("Sound")] public SoundClip[] clips;

    #region Editor
#if UNITY_EDITOR

    [Button("Set Rotation", ButtonSizes.Large), GUIColor(0.3f, 1f, 0.1f), BoxGroup("Rotations")]
    public void SetNewRotation()
    {
        //add new rotation to the list
        eulerRotations.Add(transform.rotation.eulerAngles);
    }

#endif
    #endregion


    private Coroutine _rotateCoroutine;

    public void MoveToNextRotation()
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
        }

        _rotateCoroutine = StartCoroutine(Rotate());
    }


    public void RotateToIndex(int index)
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
        }

        _rotateCoroutine = StartCoroutine(Rotate(index));
        PlaySound(index);
    }


    private IEnumerator Rotate(int index = -1)
    {
        if (index != -1)
        {
            currentIndex = index;
        }
        else
        {
            currentIndex = (currentIndex + 1) % eulerRotations.Count;
        }


        float timeElapsed = 0;
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(eulerRotations[currentIndex]);

        while (timeElapsed < transitionTime)
        {
            timeElapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeElapsed / transitionTime);
            yield return null;
        }

        onCompleteRotation.Invoke();
    }

    private void PlaySound(int index)
    {
        if(!source && !clips[index].clip) return;
        source.PlayOneShot(clips[index].clip, clips[index].volume);
    }
    
}