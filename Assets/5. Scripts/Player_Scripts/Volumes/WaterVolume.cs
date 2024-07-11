using System;
using System.Collections;
using Thema_Camera;
using UnityEngine;

namespace Player_Scripts.Volumes
{

    public class WaterVolume : MonoBehaviour
    {

        public float surfaceLevel;
        public float bottomLevel;
        public float playerX;


        public ChangeOffset underWaterOffset;
        public ChangeOffset surfaceOffset;


        private Coroutine _triggerCoroutine;
        private bool _triggered;
        private bool _atSurface;
        
        private void OnTriggerStay(Collider other)
        {
            if (_triggerCoroutine != null)
            {
                StopCoroutine(_triggerCoroutine);
            }
            _triggerCoroutine = StartCoroutine(ResetTrigger());
            
            if(_triggered) return;
            if (other.CompareTag("Player_Main"))
            {
                _triggered = true;
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
            _triggered = false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(playerX, surfaceLevel, transform.position.z),
                new Vector3(playerX, bottomLevel, transform.position.z));
            
            Gizmos.color =Color.red;
            Gizmos.DrawWireSphere(new Vector3(playerX, surfaceLevel, transform.position.z), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(playerX, bottomLevel, transform.position.z), 0.2f);
        }


        public void ChangeCameraOffset(bool atSurface)
        {
            if(_atSurface == atSurface) return;
            
            _atSurface = atSurface;
            if (atSurface)
            {
                surfaceOffset.ChangeCameraOffset();
            }
            else
            {
                underWaterOffset.ChangeCameraOffset();
            }
            
        }

    }



}