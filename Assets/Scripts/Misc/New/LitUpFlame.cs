using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LitUpFlame : MonoBehaviour
{

    [SerializeField] private ParticleSystem mainFlame;
    [SerializeField] private Light light_;

    [SerializeField] private string animationName;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private UnityEvent events;

    private bool _movePlayer;

    private bool _litLight;

    private Transform _target;

    private void Update()
    {

        if (_litLight)
        {
            light_.intensity = Mathf.Lerp(light_.intensity, 10, Time.deltaTime * 10);
        }

        if (!_movePlayer) return;

        if (!_target) return;

        Vector3 targetPos = _target.position;
        Vector3 destinationPos = transform.position;

        Quaternion targetRot = _target.rotation;
        Quaternion destinationRot = transform.rotation;

        _target.position = Vector3.MoveTowards(targetPos, destinationPos, Time.deltaTime * movementSpeed);
        _target.rotation = Quaternion.RotateTowards(targetRot, destinationRot, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(targetPos, destinationPos) < 0.01f)
        {
            if (Quaternion.Angle(targetRot, destinationRot) < 5f)
            {
                _movePlayer = false;
            }
        }

        _target.position = Vector3.MoveTowards(targetPos, destinationPos, Time.deltaTime * movementSpeed);
        _target.rotation = Quaternion.RotateTowards(targetRot, destinationRot, Time.deltaTime * rotationSpeed);



    }

    public void LitUp()
    {
        _target = Player_Scripts.PlayerController.Instance.transform;
        StartCoroutine(LitUpFlare());
    }

    public IEnumerator LitUpFlare()
    {
        _movePlayer = true;
        Player_Scripts.PlayerController.Instance.CrossFadeAnimation(animationName, 0.5f);

        yield return new WaitForSeconds(3f);

        _litLight = true;
        mainFlame.Play();

        yield return new WaitForSeconds(2f);

        _litLight = false;
        events?.Invoke();
    }

}
