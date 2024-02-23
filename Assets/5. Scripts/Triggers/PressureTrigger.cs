using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class PressureTrigger : MonoBehaviour
{

    [SerializeField] private bool canTrigger = true;
    [SerializeField, Range(1, 30)] private int pressureThresold = 1;
    [SerializeField] private UnityEvent triggerAction;
    [SerializeField] private UnityEvent resetAction;


    [SerializeField, Space(10)] private AudioSource soundSource;
    [SerializeField, Space(10)] private AudioClip triggerSound;

    private int _objectInTirgger;
    private bool _triggerd;

    private void OnTriggerEnter(Collider other)
    {

        if (!canTrigger) return;

        _objectInTirgger = _objectInTirgger + 1;
        if (_triggerd) return;

        if(_objectInTirgger >= pressureThresold)
        {
            _triggerd = true;
            triggerAction?.Invoke();
            soundSource?.PlayOneShot(triggerSound);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!canTrigger) return;

        _objectInTirgger = Mathf.Clamp(_objectInTirgger-1, 0, 30);

        if (!_triggerd) return;

        if(_objectInTirgger < pressureThresold)
        {
            _triggerd = false;
            resetAction?.Invoke();
        }
    }



}
