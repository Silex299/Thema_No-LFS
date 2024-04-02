using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField, BoxGroup] private Vector3 closedPosition;
    [SerializeField, BoxGroup] private Vector3 openPosition;
    [SerializeField, BoxGroup] private float transitionTime = 1;
    [SerializeField, BoxGroup] private float delay;


    [SerializeField, BoxGroup("Sound")] private AudioClip openSound;
    [SerializeField, BoxGroup("Sound")] private AudioClip closeSound;
    [SerializeField, BoxGroup("Sound")] private AudioSource soundSource;


#if UNITY_EDITOR
    [SerializeField, Button("Set Closed Postion", ButtonSizes.Large)]
    public void SetClosedPosition()
    {
        closedPosition = transform.localPosition;
    }

    [SerializeField, Button("Set Open Postion", ButtonSizes.Large)]
    public void SetOpenOpen()
    {
        openPosition = transform.localPosition;
    }

#endif

    [SerializeField, BoxGroup("Misc")] private bool isOpen;
    private bool _action;
    private Coroutine _toggleDoor;
    private float _timeElapsed;



    private void Update()
    {
        if (!_action) return;


        if (isOpen)
        {
            _timeElapsed += Time.deltaTime;
            float fraction = _timeElapsed / transitionTime;

            //Move to open position
            transform.localPosition = Vector3.Lerp(closedPosition, openPosition, fraction);

            if (fraction >= 1)
            {
                _action = false;
            }
        }

        else
        {
            _timeElapsed += Time.deltaTime;
            float fraction = _timeElapsed / transitionTime;

            //Move to Close position
            transform.localPosition = Vector3.Lerp(openPosition, closedPosition, fraction);

            if (fraction >= 1)
            {
                _action = false;
            }
        }

    }


    public void ToggleDoor(bool open)
    {
        if (_toggleDoor != null)
        {
            StopCoroutine(_toggleDoor);
        }

        _toggleDoor = StartCoroutine(DoorToggle(open));
    }

    public IEnumerator DoorToggle(bool open)
    {

        yield return new WaitForSeconds(delay);

        if (open != isOpen)
        {
            _toggleDoor = null;
            _action = true;
            isOpen = open;
            _timeElapsed = 0;
            PlaySound(open);
        }
    }

    public void PlaySound(bool open)
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
