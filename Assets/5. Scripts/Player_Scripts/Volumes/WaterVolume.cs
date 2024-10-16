using System.Collections;
using Sirenix.OdinInspector;
using Thema_Camera;
using UnityEngine;

namespace Player_Scripts.Volumes
{
    public class WaterVolume : MonoBehaviour
    {
        [FoldoutGroup("Volume Property")] public float surfaceLevel;
        [FoldoutGroup("Volume Property")] public float bottomLevel;
        [FoldoutGroup("Volume Property")] public float playerX;
        [FoldoutGroup("Volume Property"), Tooltip("Defines after what y position the player state is changed to water")] 
        public float triggerYThreshold;
        [FoldoutGroup("Volume Property")] public float damageSpeed;
        
        [FoldoutGroup("Particle Effects")] public GameObject splashEffect;
        [FoldoutGroup("Particle Effects")] public Vector3 splashEffectOffset;
        [FoldoutGroup("Particle Effects"), Space(10)]
        public GameObject dragEffect;
        [FoldoutGroup("Particle Effects")] public Vector3 dragEffectOffset;
        [FoldoutGroup("Particle Effects"), Space(10)]
        public GameObject underwaterEffect;
        [FoldoutGroup("Particle Effects"), Space(10)]
        public Vector3 underwaterEffectOffset;

        [FoldoutGroup("Camera Offsets")] public ChangeOffset underwaterCameraOffset;
        [FoldoutGroup("Camera Offsets")] public ChangeOffset aboveWaterCameraOffset;
        

        private Coroutine _triggerCoroutine;
        private bool _triggered;
        private bool _playerOnSurface;
        private GameObject _spawnedDragEffect;
        private GameObject _spawnedUnderwaterEffect;
        
        private Player Player => PlayerMovementController.Instance.player;

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player_Main")) return;
            SpawnSplash();
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;

            if (!other.CompareTag("Player_Main")) return;
            if (!_triggered) TriggerWaterVolume();
            
            if (_triggerCoroutine != null)
            {
                StopCoroutine(_triggerCoroutine);
            }
            _triggerCoroutine = StartCoroutine(ResetTrigger());
        }
        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            
            DestroySpawnedEffects();
            Physics.gravity = new Vector3(0, -9.81f, 0);
            _triggered = false;
            _triggerCoroutine = null;
            
        }
        private void TriggerWaterVolume()
        {
            if (!enabled) return;
            
            if(triggerYThreshold < Player.transform.position.y) return;
            
            _triggered = true;
            SpawnEffects(false);
        
            Physics.gravity = new Vector3(0, -0.5f, 0);
            if (PlayerMovementController.Instance.player.currentStateIndex != 2)
            {
                PlayerMovementController.Instance.ChangeState(2);
            }
            
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(playerX, surfaceLevel, transform.position.z),
                new Vector3(playerX, bottomLevel, transform.position.z));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(playerX, surfaceLevel, transform.position.z), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(playerX, bottomLevel, transform.position.z), 0.2f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(new Vector3(playerX, triggerYThreshold, transform.position.z), 0.2f);
        }


        private void Update()
        {
            if (!_triggered) return;

           
            bool onSurface = (Player.transform.position.y + 1.5f) >= surfaceLevel;
            if (onSurface == _playerOnSurface) return;
            
            
            Player.waterMovement.OnSurface = onSurface;
            _playerOnSurface = onSurface;
            SpawnEffects(onSurface);
                
            if (_playerOnSurface)
            {
                aboveWaterCameraOffset.ChangeCameraOffset();
            }
            else
            {
                underwaterCameraOffset.ChangeCameraOffset();
            }
        }

        private void LateUpdate()
        {
            if(!_triggered) return;
            
            #region Player position update

            Vector3 desiredPos = Player.transform.position;
            desiredPos.x = playerX;

            if (_playerOnSurface)
            {
                if (Input.GetAxis("Vertical") >= 0)
                {
                    desiredPos.y = surfaceLevel - 1.3f;
                }
            }

            Player.transform.position = Vector3.Distance(desiredPos, Player.transform.position) > 0.2f ? 
                Vector3.Lerp(Player.transform.position, desiredPos, Time.deltaTime * 7) 
                : desiredPos; //Change the speed maybe

            #endregion
        }


        #region Particle

        private void SpawnSplash()
        {
            if(triggerYThreshold > Player.transform.position.y) return;
            Instantiate(splashEffect, Player.transform.position + splashEffectOffset, Quaternion.identity);
        }

        private void SpawnEffects(bool onSurface)
        {
            if (!_spawnedDragEffect)
            {
                _spawnedDragEffect = Instantiate(dragEffect, parent: Player.transform);
                _spawnedDragEffect.transform.localPosition = dragEffectOffset;
                _spawnedDragEffect.transform.localRotation = Quaternion.identity;
            }

            if (!_spawnedUnderwaterEffect)
            {
                _spawnedUnderwaterEffect = Instantiate(underwaterEffect, parent: Player.transform);
                _spawnedUnderwaterEffect.transform.localPosition = underwaterEffectOffset;
                _spawnedUnderwaterEffect.transform.localRotation = Quaternion.identity;
            }
            
            _spawnedDragEffect.SetActive(onSurface);
            _spawnedUnderwaterEffect.SetActive(!onSurface);
            
        }
        private void DestroySpawnedEffects()
        {
            if (_spawnedDragEffect)
            {
                Destroy(_spawnedDragEffect);
                _spawnedDragEffect = null;
            }
            if (_spawnedUnderwaterEffect)
            {
                Destroy(_spawnedUnderwaterEffect);
                _spawnedUnderwaterEffect = null;
            }
        }

        #endregion
        
    }
}