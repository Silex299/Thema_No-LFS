using Managers.Checkpoints;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIExitMenu : MonoBehaviour
    {
        public TextMeshProUGUI lastCheckpointText;
        public TextMeshProUGUI exitToMainMenuText;

        public Color selectedColor;
        public Color deSelectedColor;

        private bool _isRestartLastCheckpointSelected;
        private bool _isCheckpointMenuOpen;


        public void OnEnable()
        {
            UIManager.Instance.onCheckpointMenuLoaded += OnCheckpointMenuLoaded;
        }

        public void OnDisable()
        {
            UIManager.Instance.onCheckpointMenuLoaded -= OnCheckpointMenuLoaded;
        }

        private void Update()
        {
            if(!_isCheckpointMenuOpen) return;


            float input = Input.GetAxis("Vertical");
            bool? up = input switch
            {
                > 0.1f => true,
                < -0.1f => false,
                _ => null
            };
            
            if (up == true)
            {
                SelectLastCheckpoint();
            }
            else if (up == false)
            {
                SelectExitToMainMenu();
            }

            if (Input.GetButtonDown("Submit"))
            {
                if (_isRestartLastCheckpointSelected)
                {
                    CheckpointManager.Instance?.LoadCheckpoint();
                }
                else
                {
                    LocalSceneManager.Instance.LoadMainMenu();
                }
            }

        }


        private void OnCheckpointMenuLoaded(bool menuLoaded)
        {
            _isCheckpointMenuOpen = menuLoaded;

            if (_isCheckpointMenuOpen)
            {
                SelectLastCheckpoint();
            }
        }
        
        private void SelectLastCheckpoint()
        {
            if(_isRestartLastCheckpointSelected) return;
            
            _isRestartLastCheckpointSelected = true;
            lastCheckpointText.color = selectedColor;
            exitToMainMenuText.color = deSelectedColor;
            lastCheckpointText.rectTransform.localScale = 1.2f * Vector3.one;
            exitToMainMenuText.rectTransform.localScale = 1f * Vector3.one;
        }

        private void SelectExitToMainMenu()
        {
            if(!_isRestartLastCheckpointSelected) return;
            
            _isRestartLastCheckpointSelected = false;
            exitToMainMenuText.color = selectedColor;
            lastCheckpointText.color = deSelectedColor;
            exitToMainMenuText.rectTransform.localScale = 1.2f * Vector3.one;
            lastCheckpointText.rectTransform.localScale = 1f * Vector3.one;
        }
        
        
    }
}
