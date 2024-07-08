using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Player_Scripts.Volumes
{

    public class WaterVolume : MonoBehaviour
    {

        [BoxGroup("Movement Params"), Tooltip("distance from bottom at what the player switches from underwater to surface water params")]
        public float surfaceThreshold = 9f;

        [SerializeField, BoxGroup("Movement Params"), Tooltip("distance from bottom at what the player doesn't try to go below")]
        private float bottomThreshold = 0.5f;

        [SerializeField, BoxGroup("Movement Params"), Space(10)] private bool strictRestriction;
        [SerializeField, BoxGroup("Movement Params"), ShowIf("strictRestriction")] private float restrictedY;


        [SerializeField, Space(10), BoxGroup("Movement Params")]
        private float resticetedYOffset = 1f;
        [SerializeField, BoxGroup("Movement Params"), Tooltip("restricted x position")]
        private float restrictedX = -5.76f;


        [SerializeField, BoxGroup("Water Movement Params")] private bool dynamicWater;
        [SerializeField, BoxGroup("Water Movement Params"), ShowIf(nameof(dynamicWater))] private Transform waterSurface;
        [SerializeField, BoxGroup("Water Movement Params"), ShowIf(nameof(dynamicWater))] private float surfaceDistanceOffset;
        [SerializeField, BoxGroup("Water Movement Params"), ShowIf(nameof(dynamicWater))] private float stateThresold;


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


            if (dynamicWater)
            {
                float surfaceDistance;
                WaterSurfaceCheck(out surfaceDistance);

                if (surfaceDistance < stateThresold)
                {
                    if (_player.VerifyState(PlayerMovementState.Water))
                    {
                        _player.ChangeState(0);
                    }

                    return;
                }
                else
                {
                    if (!_player.VerifyState(PlayerMovementState.Water))
                    {
                        _player.ChangeState(2);
                        _player.WaterSurfaceHit(true);
                    }
                }
            }


            //Change state to water 
            //TODO put the verify state inside distance check
            if (!_player.VerifyState(PlayerMovementState.Water))
            {
                if (_distance < surfaceThreshold - 1f)
                {
                    _player.ChangeState(2);
                    underwaterAction?.Invoke();
                }
            }
            else
            {

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

                if (_distance < bottomThreshold)
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



        }

        private void LateUpdate()
        {
            if (!_playerIsInTrigger) return;


            //Restrict
            if (!_player.VerifyState(PlayerMovementState.Water))
            {
                return;
            }

            if (_player.player.DisabledPlayerMovement && !dynamicWater)
            {
                return;
            }



            Vector3 playerPos = _player.transform.position;

            if (_atSurface)
            {

                if (Input.GetAxis("Vertical") >= 0)
                {
                    playerPos.y = transform.position.y + surfaceThreshold + resticetedYOffset;
                }

            }

            if (strictRestriction)
            {
                if (playerPos.y > restrictedY)
                {
                    playerPos.y = restrictedY;
                }
            }

            playerPos.x = restrictedX;


            _player.transform.position = playerPos;
        }

        private void WaterSurfaceCheck(out float surfaceDistance)
        {
            surfaceDistance = waterSurface.position.y - transform.position.y;
            surfaceThreshold = surfaceDistance - surfaceDistanceOffset;
        }

    }



}