using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Player_Scripts.Interactions
{

    public class WaterVolume : MonoBehaviour
    {

        public float distance;

        [SerializeField] private UnityEvent onSurfaceAction;
        [SerializeField] private UnityEvent underwaterAction;
        [SerializeField] private UnityEvent bottomAction;

        [SerializeField, Space(10)] private float surfaceThreshold = 9f;
        [SerializeField] private float bottomThreshold = 0.5f;


        private bool _playerIsInTrigger;
        private PlayerMovementController _player;
        private Coroutine _resetCoroutine;


        private bool _atSurface;
        private bool _atBottom;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                if (_resetCoroutine != null)
                {
                    StopCoroutine(_resetCoroutine);
                }

                _resetCoroutine = StartCoroutine(Reset());

                if (!_player)
                {
                    _player = PlayerMovementController.Instance;
                }

                if (!_playerIsInTrigger)
                {
                    _playerIsInTrigger = true;
                }


            }
        }

        private IEnumerator Reset()
        {
            yield return new WaitForSeconds(0.3f);
            _playerIsInTrigger = false;
        }


        private void Update()
        {
            if (!_playerIsInTrigger) return;



            distance = Mathf.Abs(transform.position.y - _player.transform.position.y);


            if (!_player.VerifyState(PlayerMovementState.Water))
            {
                if(distance < surfaceThreshold - 1f)
                {
                    _player.ChangeState(PlayerMovementState.Water, 2);
                }
            }



            if (distance > surfaceThreshold)
            {
                if (!_atSurface)
                {
                    _atSurface = true;
                    onSurfaceAction?.Invoke();
                }
            }
            else
            {
                if (_atSurface)
                {
                    _atSurface = false;
                    underwaterAction?.Invoke();
                }
            }

            if(distance < bottomThreshold)
            {
                if (!_atBottom)
                {
                    _atBottom = true;
                    bottomAction?.Invoke();
                }
            }
            else
            {
                if (_atBottom)
                {
                    _atBottom = false;
                    underwaterAction?.Invoke();
                }
            }

        }

        private void LateUpdate()
        {
            if (!_playerIsInTrigger) return;


            //Restrict
            if (_player.player.DisablePlayerMovement || !_player.VerifyState(PlayerMovementState.Water))
            {
                return;
            }

            if (_atSurface)
            {
                Vector3 playerPos = _player.transform.position;
                playerPos.x = -5.76f;

                if (Input.GetAxis("Vertical") >= 0)
                {
                    playerPos.y = 7.2f;
                }

                _player.transform.position = playerPos;
            }

           


        }



    }



}