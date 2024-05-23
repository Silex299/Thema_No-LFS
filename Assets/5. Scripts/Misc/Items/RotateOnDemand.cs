using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

#if UNITY_EDITOR

    [Button("Set Rotation", ButtonSizes.Large), GUIColor(0.3f, 1f, 0.1f), BoxGroup("Rotations")]
    public void SetNewRotation()
    {
        //add new rotation to the list
        eulerRotations.Add(transform.rotation.eulerAngles);
    }

#endif


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
}