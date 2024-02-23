using System.Collections;
using UnityEngine;

public class ThrowableBox : MonoBehaviour
{

    [SerializeField] private float movementSmoothness;

    [SerializeField] private Transform handSocket;
    [SerializeField] private Transform rightHandIKSocket;
    [SerializeField] private Collider coll;
    [SerializeField] private Rigidbody rb;



    private bool _isInTrigger;
    private bool _isInControl;
    private bool _movePlayer;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _isInTrigger = false;
            _movePlayer = false;
        }
    }

    private void Update()
    {
        if (_isInTrigger && !_isInControl)
        {
            if (Input.GetButtonDown("f") && !_movePlayer)
            {

                if (!coll.enabled)
                {
                    StartCoroutine(MovePlayer());
                }


            }
        }
        if (_isInControl)
        {
            if (Input.GetButtonDown("f"))
            {

                StartCoroutine(Drop());

            }
        }
    }

    private void LateUpdate()
    {
        if (!_isInTrigger) return;
        if (!_movePlayer) return;

        var target = Player_Scripts.PlayerController.Instance.transform;

        Vector3 direction = transform.position - target.position;

        var forward = target.forward;
        direction.y = forward.y;

        target.forward = Vector3.MoveTowards(forward, direction, Time.deltaTime * movementSmoothness);

        if (Vector3.Angle(forward, direction) < 2f)
        {
            _movePlayer = false;
        }


    }

    private IEnumerator MovePlayer()
    {
        var playerController = Player_Scripts.PlayerController.Instance;
        _movePlayer = true;
        playerController.Player.canRotate = false;
        playerController.Player.canSprint = false;

        yield return new WaitUntil(() => { return !_movePlayer; });


        playerController.Player.AnimationController.CrossFade("Kneel", 0.2f, 1);

        yield return new WaitForSeconds(2f);

        playerController.Player.AnimationController.CrossFade("Hold Box", 0.1f, 3);
        playerController.Player.Rig.RightHandTarget = rightHandIKSocket;
        playerController.Player.Rig.SetRightHandIK();
        playerController.Player.canRotate = true;

        transform.parent = handSocket;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _isInControl = true;

    }

    private IEnumerator Drop()
    {

        var playerController = Player_Scripts.PlayerController.Instance;

        _isInControl = false;
        playerController.Player.AnimationController.CrossFade("Throw Box", 0.2f, 3);

        yield return new WaitForSeconds(1.5f);


        playerController.Player.canSprint = true;
        transform.parent = Managers.GameManager.instance.transform;
        playerController.Player.Rig.ResetRightHandIK();

        rb.useGravity = true;
        rb.isKinematic = false;
        coll.enabled = true;

        var transform1 = playerController.transform;
        var force = 3 * transform1.forward;
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);
        coll.enabled = true;

        yield return new WaitForSeconds(3f);

        coll.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void ThrowObject()
    {
    }



}
