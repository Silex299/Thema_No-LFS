using System.Collections;
using Sirenix.OdinInspector;
using Thema_Camera;
using UnityEngine;

namespace Player_Scripts.Volumes
{
    public class WaterVolume : MonoBehaviour
    {
        [BoxGroup("Volume Property")] public float surfaceLevel;
        [BoxGroup("Volume Property")] public float bottomLevel;
        [BoxGroup("Volume Property")] public float playerX;
        [BoxGroup("Volume Property")] public float damageSpeed;


        [BoxGroup("Effects")] public ChangeOffset underWaterOffset;
        [BoxGroup("Effects")] public ChangeOffset surfaceOffset;

        [BoxGroup("Effects")] public SoundVolumeTrigger underWaterSound;
        [BoxGroup("Effects")] public SoundVolumeTrigger surfaceSound;

        private Coroutine _triggerCoroutine;
        private bool _playerInTrigger;

        private void OnTriggerStay(Collider other)
        {
            if (_triggerCoroutine != null)
            {
                StopCoroutine(_triggerCoroutine);
            }

            _triggerCoroutine = StartCoroutine(ResetTrigger());

            if (_playerInTrigger) return;
            if (other.CompareTag("Player_Main"))
            {
                _playerInTrigger = true;
                if (PlayerMovementController.Instance.player.currentStateIndex != 2)
                {
                    PlayerMovementController.Instance.ChangeState(2);
                }
                else
                {
                    PlayerMovementController.Instance.player.waterMovement.waterVolume = this;
                }
            }
        }

        private IEnumerator ResetTrigger()
        {
            yield return new WaitForSeconds(0.2f);
            _triggerCoroutine = null;
            _playerInTrigger = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(playerX, surfaceLevel, transform.position.z),
                new Vector3(playerX, bottomLevel, transform.position.z));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(playerX, surfaceLevel, transform.position.z), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(playerX, bottomLevel, transform.position.z), 0.2f);
        }

        public void UnderWater()
        {
            underWaterOffset.ChangeCameraOffset();
            underWaterSound.ApplyAudioVolume();
        }

        public void OnSurface()
        {
            surfaceOffset.ChangeCameraOffset();
            surfaceSound.ApplyAudioVolume();
        }
    }
}