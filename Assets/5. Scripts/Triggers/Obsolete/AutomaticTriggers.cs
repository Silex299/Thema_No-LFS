using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class AutomaticTriggers : MonoBehaviour
{

    [SerializeField] private string colliderTag;

    [SerializeField] private float timeDelay;
    [SerializeField] private bool oneTime;


    [SerializeField, Space(10)]
    private UnityEvent action;


    private bool _called;


    private void OnTriggerEnter(Collider other)
    {
        if (oneTime && _called) return;

        if (other.CompareTag(colliderTag))
        {
            Invoke("Action", timeDelay);
        }
    }


    private void Action()
    {
        action?.Invoke();
        _called = true;
    }

}
