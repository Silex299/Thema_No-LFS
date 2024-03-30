using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Player_Scripts.Interactions
{

    public class WaterVolume : MonoBehaviour
    {

        [SerializeField, BoxGroup("Movement Params"), Tooltip("distance from bottom at what the player switches from underwater to surface water params")] 
        private float surfaceThreshold = 9f;

        [SerializeField, BoxGroup("Movement Params"), Tooltip("distance from bottom at what the player doesn't try to go below")] 
        private float bottomThreshold = 0.5f;

        [SerializeField, Space(10), BoxGroup("Movement Params"), Tooltip("fixed y position when the player is at the suface of the water")] 
        private float restrictedY = 7.2f;
        [SerializeField, BoxGroup("Movement Params"), Tooltip("restricted x position")] 
        private float restrictedX = -5.76f;


        [SerializeField, FoldoutGroup("Events")] private UnityEvent onSurfaceAction;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent underwaterAction;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent bottomAction;



        private float _distance;
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
            _player = null;
        }


        private void Update()
        {
            if (!_playerIsInTrigger) return;



            _distance = Mathf.Abs(transform.position.y - _player.transform.position.y);


            if (!_player.VerifyState(PlayerMovementState.Water))
            {
                if(_distance < surfaceThreshold - 1f)
                {
                    _player.ChangeState(PlayerMovementState.Water, 2);
                    _player.WaterSurfaceHit(false);
                }
            }



            if (_distance > surfaceThreshold)
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

            if(_distance < bottomThreshold)
            {
                if (!_atBottom)
                {
                    _atBottom = true;
                    bottomAction?.Invoke();
                }
            }
            else
            {

                print("fuck me daddy");
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



            Vector3 playerPos = _player.transform.position;
            if (_atSurface)
            {

                if (Input.GetAxis("Vertical") >= 0)
                {
                    playerPos.y = restrictedY;
                }

            }

            playerPos.x = restrictedX;


            _player.transform.position = playerPos;
        }



    }



}