using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField, BoxGroup] private Vector3 closedPosition;
    [SerializeField, BoxGroup] private Vector3 openPosition;
    [SerializeField, BoxGroup] private float speed;
    [SerializeField, BoxGroup] private float delay;


    [SerializeField, BoxGroup("Sound")] private AudioClip openSound;
    [SerializeField, BoxGroup("Sound")] private AudioClip closeSound;
    [SerializeField, BoxGroup("Sound")] private AudioSource soundSource;


#if UNITY_EDITOR
    [SerializeField, Button("Set Closed Postion", ButtonSizes.Large)]
    public void SetClosedPosition()
    {
        closedPosition = transform.position;
    }

    [SerializeField, Button("Set Open Postion", ButtonSizes.Large)]
    public void SetOpenOpen()
    {
        openPosition = transform.position;
    }

#endif

    private bool _isOpen;
    private bool _action;
    private Coroutine _toggleDoor;

    private void Update()
    {
        if (!_action) return;

        var pos = transform.position;

        if (_isOpen)
        {
            //Move to open position
            transform.position = Vector3.MoveTowards(pos, openPosition, Time.deltaTime * speed);

            if(Mathf.Abs(Vector3.Distance(pos, openPosition)) < 0.01f)
            {
                _action = false;
            }
        }

        else
        {
            //Move to closed position
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, Time.deltaTime * speed);
            if (Mathf.Abs(Vector3.Distance(pos, closedPosition)) < 0.01f)
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

        _toggleDoor = null;
        _action = true;
        _isOpen = open;
        PlaySound(open);
    }

    public void PlaySound(bool open)
    {
        if (soundSource)
        {
            soundSource.PlayOneShot(open ? openSound : closeSound);
        }
    }



}
