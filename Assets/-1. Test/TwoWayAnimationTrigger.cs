using System;
using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using UnityEditor;
using UnityEngine;

public class TwoWayAnimationTrigger : MonoBehaviour
{
    public string forwardAnimation;
    public string backwardAnimation;

    public bool axisGreater = true;
    public string axisInputName;

    private bool _playerInTrigger;
    private Coroutine _resetTriggerCoroutine;
    private bool _played;


    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player_Main")) return;

        _playerInTrigger = true;

        if (_resetTriggerCoroutine != null)
        {
            StopCoroutine(_resetTriggerCoroutine);
        }

        _resetTriggerCoroutine = StartCoroutine(ResetTrigger());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_Main"))
        {
            _playerInTrigger = false;
        }
    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(0.2f);
        _playerInTrigger = false;
    }


    private void Update()
    {
        if (!_playerInTrigger) return;

        if (axisGreater ? Input.GetAxis(axisInputName) > 0 : Input.GetAxis(axisInputName) < 0)
        {
            if (!_played)
            {
                PlayerMovementController.Instance.PlayAnimation(forwardAnimation, 0.4f, 1);
                _played = true;
            }
        }
        else
        {
            if (_played)
            {
                PlayerMovementController.Instance.PlayAnimation(backwardAnimation, 0.4f, 1);
                _played = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (_played && _playerInTrigger)
        {
    
            Transform playerTransform = PlayerMovementController.Instance.transform;
            Vector3 playerPosition = playerTransform.position;
            Quaternion playerRotation = playerTransform.rotation;

            playerTransform.position = Vector3.Lerp(playerPosition, transform.position, Time.deltaTime * 10);
            playerTransform.rotation = Quaternion.Lerp(playerRotation, transform.rotation, Time.deltaTime * 10);

        }
    }
}