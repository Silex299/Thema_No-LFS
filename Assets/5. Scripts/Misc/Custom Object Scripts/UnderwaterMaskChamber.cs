using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.Custom_Object_Scripts
{
    public class UnderwaterMaskChamber : MonoBehaviour
    {

        [AssetsOnly] public GameObject playerMaskPrefab;
        
        public Animator animator;
        private GameObject _playerMask;
        private Coroutine _openChamberCoroutine;
        
        public void OpenChamber()
        {
            StartCoroutine(OpenChamberCoroutine());
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator OpenChamberCoroutine()
        {
            animator.Play("OpenChamber");
            PlayerMovementController.Instance.player.DisabledPlayerMovement = true;
            PlayerMovementController.Instance.ResetAnimator();
            
            yield return new WaitForSeconds(1.5f);
            
            _playerMask = Instantiate(playerMaskPrefab);
            _playerMask.GetComponent<UnderwaterPlayerMask>().Equip(true);

            yield return new WaitForSeconds(1f);
            
            PlayerMovementController.Instance.player.DisabledPlayerMovement = false;
        }
        
        public void Reset()
        {
            animator.Play("CloseChamber", 0, 1);
            if (_playerMask)
            {
                _playerMask.GetComponent<UnderwaterPlayerMask>().Equip(false);
                Destroy(_playerMask, 0.5f);
                _playerMask = null;
            }
        }
        public void Set()
        {
            StartCoroutine(DelayedSet());
        }
        IEnumerator DelayedSet()
        {
            animator.Play("OpenChamber", 0, 1);

            yield return new WaitForSeconds(0.5f);
            
            if (!_playerMask)
            {
                _playerMask = Instantiate(playerMaskPrefab);   
            }

            _playerMask.GetComponent<UnderwaterPlayerMask>().Equip(PlayerMovementController.Instance.player.eCurrentState == PlayerMovementState.Water);
        }

    }
}
