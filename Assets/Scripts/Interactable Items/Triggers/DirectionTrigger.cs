using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class DirectionTrigger : MonoBehaviour
{


    [SerializeField] private string triggerTag;
    [SerializeField] private float angleOffset = 10f;

    [SerializeField] private bool isAutomatic = true;

    [SerializeField, HideIf("isAutomatic")] private string Input;


    [SerializeField] private UnityEvent action;


    private bool _playerIsInTrigger;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(triggerTag))
        {
            if (isAutomatic)
            {
                var angle = Quaternion.Angle(transform.rotation, other.transform.rotation);
                if (angle < angleOffset)
                {
                    action?.Invoke();
                }
            }
        }

    }



}
