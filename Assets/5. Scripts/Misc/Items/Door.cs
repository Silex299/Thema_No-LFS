 using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField, BoxGroup] private Vector3 closedPosition;
    [SerializeField, BoxGroup] private Vector3 openPosition;
    [SerializeField, BoxGroup] private float transitionTime = 1;
    [SerializeField, BoxGroup] private float delay;
    [SerializeField, BoxGroup] private bool followCurve;
    [ShowIf("followCurve"), BoxGroup] public AnimationCurve openCurve;
    [ShowIf("followCurve"), BoxGroup] public AnimationCurve closeCurve;


    [SerializeField, BoxGroup("Sound")] private AudioClip openSound;
    [SerializeField, BoxGroup("Sound")] private AudioClip closeSound;
    [SerializeField, BoxGroup("Sound")] private AudioSource soundSource;

    [SerializeField, BoxGroup("Sound")] private UnityEvent<bool> action;


#if UNITY_EDITOR
    [SerializeField, Button("Set Closed Position", ButtonSizes.Large)]
    public void SetClosedPosition()
    {
        closedPosition = transform.localPosition;
    }

    [SerializeField, Button("Set Open Position", ButtonSizes.Large)]
    public void SetOpenOpen()
    {
        openPosition = transform.localPosition;
    }

#endif

    [SerializeField, BoxGroup("Misc")] private bool isOpen;
    private Coroutine _toggleDoor;



    public void ToggleDoor(bool open)
    {
        if (_toggleDoor != null)
        {
            StopCoroutine(_toggleDoor);
        }

        _toggleDoor = StartCoroutine(DoorToggle(open));
    }

    private IEnumerator DoorToggle(bool open)
    {
        yield return new WaitForSeconds(delay);


        if (open != isOpen)
        {
            Vector3 destination = open ? openPosition : closedPosition;
            Vector3 initialPos = transform.localPosition;
            
            float timeElapsed = 0;
            
            
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;

                if (!followCurve)
                {
                    transform.localPosition = Vector3.Lerp(initialPos, destination, timeElapsed / transitionTime);
                }
                else
                {
                    float normalisedTime = open? openCurve.Evaluate(timeElapsed / transitionTime) : closeCurve.Evaluate(timeElapsed / transitionTime);
                    transform.localPosition = Vector3.LerpUnclamped(initialPos, destination, normalisedTime);
                }
                
                yield return null;
            }
            
            isOpen = open;
            PlaySound(open);
            action.Invoke(open);

        }
        
    }

    private void PlaySound(bool open)
    {
        if (soundSource)
        {
            soundSource.PlayOneShot(open ? openSound : closeSound);
        }
    }

    public void InstantToggle(bool open)
    {
        print("instant");
        transform.localPosition = open ? openPosition : closedPosition;
        isOpen = open;
    }


}
